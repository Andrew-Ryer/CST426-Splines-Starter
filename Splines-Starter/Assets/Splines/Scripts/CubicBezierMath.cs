using UnityEngine;

/*
 * CubicBezierMath holds De Casteljau point and tangent sampling for four
 * control points. SplinePath supplies the chosen segment's current world positions.
 */

public static class CubicBezierMath
{
    public static Vector3 SamplePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // 1st layer
        Vector3 segment1 = p0 + (p1 - p0) * t;
        Vector3 segment2 = p1 + (p2 - p1) * t; 
        Vector3 segment3 = p2 + (p3 - p2) * t; 
        
        // 2nd layer
        Vector3 segment4 = segment1 + (segment2 - segment1) * t;
        Vector3 segment5 = segment2 + (segment3 - segment2) * t;
        
        return segment4 + (segment5 - segment4) * t;
    }

    public static Vector3 SampleTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // 1st layer
        Vector3 segment1 = p0 + (p1 - p0) * t;
        Vector3 segment2 = p1 + (p2 - p1) * t; 
        Vector3 segment3 = p2 + (p3 - p2) * t; 
        
        // 2nd layer
        Vector3 segment4 = segment1 + (segment2 - segment1) * t;
        Vector3 segment5 = segment2 + (segment3 - segment2) * t;
        
        return 3f * (segment5 - segment4);
    }
}
