Shader "Custom/WireframeGrid"
{
    Properties
    {
        // Never sampled, but UI CanvasRenderer expects every Image material to have it.
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        [Header(Grid)]
        _GridColor       ("Grid Color", Color) = (0.55, 0.15, 0.85, 1)
        _BackgroundColor ("Background Color", Color) = (0.09, 0.08, 0.11, 1)
        _Density         ("Grid Density (cells)", Float) = 12
        _Rotation        ("Rotation (deg)", Range(-180, 180)) = 0
        _Emission        ("Emission (drives Bloom)", Range(0, 8)) = 2.5

        [Header(Graph Paper)]
        _MajorEvery      ("Major Line Every N Cells", Float) = 5
        _MajorWidth      ("Major Line Width (px)", Range(0.5, 8)) = 2.2
        _MajorBrightness ("Major Line Brightness", Range(0, 1)) = 1
        _MinorWidth      ("Minor Line Width (px)", Range(0.5, 4)) = 1.2
        _MinorBrightness ("Minor Line Brightness", Range(0, 1)) = 0.35

        [Header(Vignette)]
        _VignettePower   ("Vignette Falloff", Range(0.5, 4)) = 1.6
        _VignetteStrength("Vignette Strength", Range(0, 1)) = 0.85

        [Header(Animation)]
        _Scroll          ("Scroll Speed (XY)", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                float4 _GridColor;
                float4 _BackgroundColor;
                float  _Density;
                float  _Rotation;
                float  _Emission;
                float  _MajorEvery;
                float  _MajorWidth;
                float  _MajorBrightness;
                float  _MinorWidth;
                float  _MinorBrightness;
                float  _VignettePower;
                float  _VignetteStrength;
                float4 _Scroll;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // Line intensity along one axis of the grid.
            //   coord: grid-space coordinate (1 unit = 1 cell; lines sit on integers)
            //   deriv: fwidth of coord, for anti-aliasing
            // Every _MajorEvery-th line is drawn thicker and at full brightness;
            // the lines in between are thinner and dimmed. Both come from the same
            // coordinate, so majors always sit exactly on top of the minor grid.
            float AxisLine (float coord, float deriv)
            {
                float nearest = round(coord);            // index of the nearest line
                float dist    = abs(coord - nearest);    // distance to it, in cells

                // Is that line a multiple of _MajorEvery? (robust for negative indices)
                float k       = nearest / max(_MajorEvery, 1.0);
                float toInt   = abs(k - round(k));
                float isMajor = step(toInt, 0.001);

                float widthPx     = lerp(_MinorWidth, _MajorWidth, isMajor);
                float brightness  = lerp(_MinorBrightness, _MajorBrightness, isMajor);

                // Distance normalized to pixels; 1 at the line center, 0 off the line.
                float mask = 1.0 - saturate(dist / max(deriv * widthPx, 1e-5));
                return mask * brightness;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Work from the center so rotation + vignette pivot on the middle.
                float2 centered = IN.uv - 0.5;
                float  r2 = dot(centered, centered);

                // Rotate the grid about the center.
                float a = radians(_Rotation);
                float s = sin(a), c = cos(a);
                float2 p = float2(c * centered.x - s * centered.y,
                                  s * centered.x + c * centered.y);

                // Optional drift.
                p += _Scroll.xy * _Time.y;

                // Grid space: 1 unit per cell, lines on integer coordinates.
                float2 grid  = p * _Density;
                float2 deriv = fwidth(grid);
                float lineMask = max(AxisLine(grid.x, deriv.x),
                                     AxisLine(grid.y, deriv.y));

                // Radial vignette.
                float vignette = pow(saturate(1.0 - r2 * 2.0), _VignettePower);
                vignette = lerp(1.0, vignette, _VignetteStrength);

                // Compose. _Emission > 1 pushes lines into HDR so URP Bloom catches them.
                half3 col = lerp(_BackgroundColor.rgb, _GridColor.rgb * _Emission, lineMask);
                col *= vignette;

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
