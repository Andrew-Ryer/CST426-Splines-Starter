using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * SplinePath joins cubic Bezier segments end to end. 3n + 1 points make n
 * segments: each segment's last point is the next segment's first point.
 * u = segment index + local t, so u runs from 0 to SegmentCount.
 *
 * The distance table approximates arc length with short chords at equal steps in u.
 * It is built once, in Awake. SamplePoint keeps reading the Transforms, so moving
 * a point updates the curve before the table is rebuilt.
 */

public class SplinePath : MonoBehaviour
{
    public Transform[] points;
    
    [Min(1)]
    public int samplesPerSegment = 64;
    
    [Serializable]
    struct DistanceRow
    {
        public float u;
        public float distance;
    }

    [SerializeField] List<DistanceRow> _distanceTable = new();

    // TODO: Count the cubic segments. The scene's ten points make three.
    public int SegmentCount => (points.Length - 1) / 3;
    
    // TODO: Return the total path length, which is the distance on the table's last row.
    public float TotalLength => _distanceTable.Last().distance;

    void Awake() => BuildDistanceTable();

    public Vector3 SamplePoint(float u)
    {
        // TODO: Return the world-space point on the spline at u.
        // u can reach SegmentCount, the very end of the path.
        int segment = Math.Min((int)u, SegmentCount - 1);
        float t = u - segment;
        int pointIndex = segment * 3;
        Vector3 p0 = points[pointIndex].position;
        Vector3 p1 = points[pointIndex + 1].position;
        Vector3 p2 = points[pointIndex + 2].position;
        Vector3 p3 = points[pointIndex + 3].position;
        
        return CubicBezierMath.SamplePoint(p0, p1, p2, p3, t);
    }

    public Vector3 SampleTangent(float u)
    {
        // TODO: Return the tangent at u, using the same segment rules as SamplePoint.
        int segment = Math.Min((int)u, SegmentCount - 1);
        float t = u - segment;
        int pointIndex = segment * 3;
        Vector3 p0 = points[pointIndex].position;
        Vector3 p1 = points[pointIndex + 1].position;
        Vector3 p2 = points[pointIndex + 2].position;
        Vector3 p3 = points[pointIndex + 3].position;
        
        return CubicBezierMath.SampleTangent(p0, p1, p2, p3, t);
    }

    // Walk the path once at equal steps in u and add up the chords.
    public void BuildDistanceTable()
    {
        // TODO: Fill the table with accumulated world distance at equal steps in u.
        // Start at distance 0 and include every segment boundary through the final endpoint.
        _distanceTable.Clear();
        
        float divisor = 1f / samplesPerSegment * SegmentCount;

        Vector3 lastPoint = points[0].position;
        float cumulitiveDistance = 0f;
        
        float du = 1f / (samplesPerSegment);
        float u = 0f;
        _distanceTable.Add(new DistanceRow() { u = 0, distance = 0f});

        for (int i = 0; i < samplesPerSegment * SegmentCount; i++)
        {
            u += du;
            Vector3 newPoint = SamplePoint(u);
            float distanceToLastPoint = (newPoint - lastPoint).magnitude;
            lastPoint = newPoint;
            cumulitiveDistance += distanceToLastPoint;
            _distanceTable.Add(new DistanceRow() { u = u, distance = cumulitiveDistance });
        }
        
    }

    // Find the two rows around the distance, then interpolate u between them.
    public float ParameterAtDistance(float distance)
    {
        // TODO: Return the u at a distance along the path. Interpolate u (not position)
        // between the two rows around it.
        
        distance = Mathf.Clamp(distance, 0f, TotalLength);
        
        // Binary Search Version
        if (distance <= 0f)
            return 0f;
        
        if (distance >= TotalLength)
            return SegmentCount;
        
        // Start with the entire distance table.
        int low = 0;
        int high = _distanceTable.Count - 1;
        
        // Narrow the search until two neighboring rows remain.
        while (high - low > 1)
        {
            int mid = low + (high - low) / 2;
        
            if (_distanceTable[mid].distance < distance)
            {
                low = mid;
            }
            else
            {
                high = mid;
            }
        }
        
        // Interpolate between the two surrounding rows.
        DistanceRow before = _distanceTable[low];
        DistanceRow after = _distanceTable[high];
        
        float t = Mathf.InverseLerp(before.distance, after.distance, distance);
        
        return Mathf.Lerp(before.u, after.u, t);
        
        // Old Version
        
        // for (int i = 1; i < _distanceTable.Count; i++)
        // {
        //     DistanceRow before = _distanceTable[i - 1];
        //     DistanceRow after = _distanceTable[i];
        //
        //     if (distance <= after.distance)
        //     {
        //         float t = Mathf.InverseLerp(
        //             before.distance,
        //             after.distance,
        //             distance
        //         );
        //
        //         return Mathf.Lerp(before.u, after.u, t);
        //     }
        // }
        //
        // return SegmentCount;
    }

    void OnDrawGizmos()
    {
        // TODO: Draw the control points, control polygon, and the whole spline with CurveGizmos.
        // CurveGizmos calls your sampling function with a value from 0 to 1, but SamplePoint
        // expects u from 0 to SegmentCount. Scale the value so 0 to 1 covers the whole
        // path, not just the first segment, and ask for enough samples for every segment.
        int totalSamples = samplesPerSegment * SegmentCount;
        float uRange = SegmentCount;
        CurveGizmos.Draw(totalSamples, t=>SamplePoint(t * uRange), points);
    }
}
