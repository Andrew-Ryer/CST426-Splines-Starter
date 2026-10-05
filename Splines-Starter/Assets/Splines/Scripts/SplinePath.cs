using System;
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

    [SerializeField] DistanceRow[] _distanceTable;

    // TODO: Count the cubic segments. The scene's ten points make three.
    public int SegmentCount => 0;
    
    // TODO: Return the total path length, which is the distance on the table's last row.
    public float TotalLength => 0f;

    void Awake() => BuildDistanceTable();

    public Vector3 SamplePoint(float u)
    {
        // TODO: Return the world-space point on the spline at u.
        // u can reach SegmentCount, the very end of the path.
        return Vector3.zero;
    }

    public Vector3 SampleTangent(float u)
    {
        // TODO: Return the tangent at u, using the same segment rules as SamplePoint.
        return Vector3.zero;
    }

    // Walk the path once at equal steps in u and add up the chords.
    public void BuildDistanceTable()
    {
        // TODO: Fill the table with accumulated world distance at equal steps in u.
        // Start at distance 0 and include every segment boundary through the final endpoint.
    }

    // Find the two rows around the distance, then interpolate u between them.
    public float ParameterAtDistance(float distance)
    {
        // TODO: Return the u at a distance along the path. Interpolate u (not position)
        // between the two rows around it.
        return 0f;
    }

    void OnDrawGizmos()
    {
        // TODO: Draw the control points, control polygon, and the whole spline with CurveGizmos.
        // CurveGizmos calls your sampling function with a value from 0 to 1, but SamplePoint
        // expects u from 0 to SegmentCount. Scale the value so 0 to 1 covers the whole
        // path, not just the first segment, and ask for enough samples for every segment.
    }
}
