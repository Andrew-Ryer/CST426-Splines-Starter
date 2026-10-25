using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadMeshGeneratorStarter : MonoBehaviour
{
    public SplinePath splinePath;
    
    public int rungs = 10;
    public float width = 3f;
    [Min(0f)] public float thickness = 0.5f;
    
    public Material material;
    public float textureRepeatLength = 2f;
    public bool rebuildEveryFrame = false;
    public float[] bankAngles;
    
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    
    Mesh _roadMesh;
        

    void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        _roadMesh = new Mesh { name = "Solid Road" };
        _roadMesh.MarkDynamic();
        _meshFilter.sharedMesh = _roadMesh;
        
        BuildRoadMesh();
    }

    void Update()
    {
        if (rebuildEveryFrame)
        {
            BuildRoadMesh();
        }
    }

    /*
     * Commented out to attempt Solid Road Polish
     */
    // void BuildRoadMesh()
    // {
    //     List<Vector3> verts = new();
    //     List<Vector2> uvs = new();
    //     Array.Resize(ref bankAngles, splinePath.SegmentCount + 1);
    //     
    //     for (int i = 0; i < rungs; i++)
    //     {
    //         // Get the distance to the rung
    //         float fraction = i / (float)(rungs - 1);
    //         float distance = splinePath.TotalLength * fraction;
    //
    //         // Use distance to look up u
    //         float u = splinePath.ParameterAtDistance(distance);
    //         
    //         // Use u to get the center point
    //         Vector3 centerPoint = splinePath.SamplePoint(u);
    //         Vector3 tangent = splinePath.SampleTangent(u);
    //         
    //         int segment = (int)Mathf.Min(u, splinePath.SegmentCount - 1);
    //         float t = u - segment;
    //         float bankAngle = Mathf.SmoothStep(bankAngles[segment], bankAngles[segment + 1], t);
    //         
    //         // Calculate the "right" direction
    //         Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
    //         right = Quaternion.AngleAxis(bankAngle, tangent) * right;
    //         
    //         float halfWidth = width * 0.5f;
    //         float lift = Mathf.Sin(bankAngle * Mathf.Deg2Rad) * halfWidth;
    //
    //         centerPoint += Vector3.up * Mathf.Abs(lift);
    //         
    //         // Create the left and right verts
    //         Vector3 leftVert = centerPoint - right * halfWidth;
    //         Vector3 rightVert = centerPoint + right * halfWidth;
    //         
    //         verts.Add(leftVert);
    //         verts.Add(rightVert);
    //         
    //         uvs.Add(new Vector2(0f, distance / textureRepeatLength));
    //         uvs.Add(new Vector2(1f, distance / textureRepeatLength));
    //     }
    //     
    //     List<int> triangles = new();
    //     for (int i = 0; i < rungs - 1; i++)
    //     {
    //         int bottomLeft = i * 2;
    //         int bottomRight = bottomLeft + 1;
    //         int topLeft = (i + 1) * 2;
    //         int topRight = topLeft + 1;
    //         
    //         triangles.Add(bottomLeft);
    //         triangles.Add(topLeft);
    //         triangles.Add(bottomRight);
    //         
    //         triangles.Add(topLeft);
    //         triangles.Add(topRight);
    //         triangles.Add(bottomRight);
    //     }
    //
    //     Mesh mesh = new();
    //     mesh.vertices = verts.ToArray();
    //     mesh.triangles = triangles.ToArray();
    //     mesh.uv = uvs.ToArray();
    //     
    //     mesh.RecalculateNormals();
    //     mesh.RecalculateBounds();
    //
    //     _meshFilter.mesh = mesh;
    //     _meshRenderer.material = material;
    // }
    //-----------------------------------------------------------------
    /*
     * For Polish: Solid Road
     */
    void OnDestroy()
    {
        if (_roadMesh != null) Destroy(_roadMesh);
    }

    // Each face gets its own vertices, keeping the slab's edges sharp.
    static void AddQuad(List<Vector3> verts, List<Vector2> uvs, List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c, Vector3 d,
        Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD)
    {
        int start = verts.Count;
        verts.Add(a);
        verts.Add(b);
        verts.Add(c);
        verts.Add(d);
        uvs.Add(uvA);
        uvs.Add(uvB);
        uvs.Add(uvC);
        uvs.Add(uvD);

        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
        triangles.Add(start + 2);
        triangles.Add(start + 1);
        triangles.Add(start + 3);
    }

    public void BuildRoadMesh()
    {
        if (splinePath == null || _meshFilter == null || _roadMesh == null) return;
        int count = Mathf.Max(2, rungs);
        int segments = splinePath.SegmentCount;
        if (segments < 1) return;

        if (bankAngles == null) bankAngles = new float[segments + 1];
        else if (bankAngles.Length != segments + 1)
            Array.Resize(ref bankAngles, segments + 1);

        float halfWidth = Mathf.Max(0.01f, width) * 0.5f;
        float depth = Mathf.Max(0f, thickness);
        float repeat = Mathf.Max(0.01f, textureRepeatLength);
        float length = splinePath.TotalLength;

        Vector3[] leftTop = new Vector3[count];
        Vector3[] rightTop = new Vector3[count];
        Vector3[] leftBottom = new Vector3[count];
        Vector3[] rightBottom = new Vector3[count];
        float[] v = new float[count];

        // Sample by arc distance, not evenly spaced spline parameters.
        for (int i = 0; i < count; i++)
        {
            float distance = length * i / (count - 1f);
            float u = splinePath.ParameterAtDistance(distance);
            Vector3 centerWorld = splinePath.SamplePoint(u);
            Vector3 tangent = splinePath.SampleTangent(u).normalized;

            int segment = Mathf.Clamp(Mathf.FloorToInt(u), 0, segments - 1);
            float t = Mathf.Clamp01(u - segment);
            float bankAngle = Mathf.SmoothStep(bankAngles[segment], bankAngles[segment + 1], t);

            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
            if (right.sqrMagnitude < 0.000001f) right = Vector3.right;
            right = Quaternion.AngleAxis(bankAngle, tangent) * right;

            // The slab's lowest BOTTOM edge rests at the sampled spline height.
            // For depth 0 this is the same height as the original flat ribbon.
            centerWorld += Vector3.up * (Mathf.Abs(right.y) * halfWidth + depth);

            Vector3 lt = centerWorld - right * halfWidth;
            Vector3 rt = centerWorld + right * halfWidth;
            Vector3 down = Vector3.down * depth;

            // Mesh vertices must be in the road object's local coordinate space.
            leftTop[i] = transform.InverseTransformPoint(lt);
            rightTop[i] = transform.InverseTransformPoint(rt);
            leftBottom[i] = transform.InverseTransformPoint(lt + down);
            rightBottom[i] = transform.InverseTransformPoint(rt + down);
            v[i] = distance / repeat;
        }

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        float widthU = 1f;
        float depthU = depth / repeat;

        for (int i = 0; i < count - 1; i++)
        {
            int j = i + 1;
            // Top: width across U, traveled distance along V.
            AddQuad(verts, uvs, tris,
                leftTop[i], leftTop[j], rightTop[i], rightTop[j],
                new Vector2(0f, v[i]), new Vector2(0f, v[j]),
                new Vector2(widthU, v[i]), new Vector2(widthU, v[j]));

            if (depth <= 0f) continue;

            // Left side: the V coordinate repeats by traveled distance.
            AddQuad(verts, uvs, tris,
                leftBottom[i], leftBottom[j], leftTop[i], leftTop[j],
                new Vector2(0f, v[i]), new Vector2(0f, v[j]),
                new Vector2(depthU, v[i]), new Vector2(depthU, v[j]));

            // Right side (outward-facing winding).
            AddQuad(verts, uvs, tris,
                rightTop[i], rightTop[j], rightBottom[i], rightBottom[j],
                new Vector2(0f, v[i]), new Vector2(0f, v[j]),
                new Vector2(depthU, v[i]), new Vector2(depthU, v[j]));

            // Underneath the road.
            AddQuad(verts, uvs, tris,
                rightBottom[i], rightBottom[j], leftBottom[i], leftBottom[j],
                new Vector2(0f, v[i]), new Vector2(0f, v[j]),
                new Vector2(widthU, v[i]), new Vector2(widthU, v[j]));
        }

        if (depth > 0f)
        {
            // Start cap: faces backward along the road.
            AddQuad(verts, uvs, tris,
                leftBottom[0], leftTop[0], rightBottom[0], rightTop[0],
                new Vector2(0f, 0f), new Vector2(0f, depthU),
                new Vector2(widthU, 0f), new Vector2(widthU, depthU));

            int last = count - 1;
            // End cap: faces forward along the road.
            AddQuad(verts, uvs, tris,
                leftTop[last], leftBottom[last], rightTop[last], rightBottom[last],
                new Vector2(0f, depthU), new Vector2(0f, 0f),
                new Vector2(widthU, depthU), new Vector2(widthU, 0f));
        }

        _roadMesh.Clear();
        if (verts.Count > 65535)
            _roadMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _roadMesh.SetVertices(verts);
        _roadMesh.SetTriangles(tris, 0);
        _roadMesh.SetUVs(0, uvs);
        _roadMesh.RecalculateNormals();
        _roadMesh.RecalculateBounds();
        _meshRenderer.sharedMaterial = material;
    }
}