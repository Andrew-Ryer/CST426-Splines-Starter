using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GeneratedSpiral : MonoBehaviour
{
    //Rocket position
    public Transform rocketBase;

    //Spiral size
    public float radius = 28f;
    public float height = 115f;
    public float turns = 2f;
    public float startingAngleDegrees = 0f;
    public float baseHeight = 3f;

    //Curve resolution
    public int segmentsPerTurn = 4;

    [SerializeField]
    private List<Transform> generatedPoints = new List<Transform>();

    private bool rebuildQueued;

    private void Awake()
    {
        // Build before SplinePath.Awake reads its points in Play Mode.
        if (Application.isPlaying)
            Rebuild();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            QueueEditorRebuild();
    }

    private void OnValidate()
    {
        radius = Mathf.Max(0.1f, radius);
        height = Mathf.Max(0.1f, height);
        turns = Mathf.Max(0.25f, turns);
        segmentsPerTurn = Mathf.Max(4, segmentsPerTurn);

        if (!Application.isPlaying)
            QueueEditorRebuild();
    }
    
    
    private void OnDisable()
    {
        EditorApplication.delayCall -= RebuildInEditor;
        rebuildQueued = false;
    }

    private void QueueEditorRebuild()
    {
        if (rebuildQueued) return;
        rebuildQueued = true;
        EditorApplication.delayCall += RebuildInEditor;
    }

    private void RebuildInEditor()
    {
        rebuildQueued = false;
        if (this == null || !isActiveAndEnabled || Application.isPlaying)
            return;

        Rebuild();
    }

    [ContextMenu("Rebuild Spiral")]
    public void Rebuild()
    {
        SplinePath path = GetComponent<SplinePath>();
        int count = Mathf.CeilToInt(turns * segmentsPerTurn);
        int pointCount = 3 * count + 1;

        // Reuse existing generated point objects when settings change.
        generatedPoints.RemoveAll(point => point == null);
        while (generatedPoints.Count < pointCount)
        {
            GameObject marker = new GameObject("Spiral Point");
            marker.transform.SetParent(transform, false);
            generatedPoints.Add(marker.transform);
        }
        while (generatedPoints.Count > pointCount)
        {
            int lastIndex = generatedPoints.Count - 1;
            Transform extra = generatedPoints[lastIndex];
            generatedPoints.RemoveAt(lastIndex);
            if (Application.isPlaying) Destroy(extra.gameObject);
            else DestroyImmediate(extra.gameObject);
        }

        Vector3 center = rocketBase != null ? rocketBase.position : transform.position;
        center.y += baseHeight;

        float start = startingAngleDegrees * Mathf.Deg2Rad;
        float anglePerSegment = turns * Mathf.PI * 2f / count;
        float heightPerSegment = height / count;
        float handleFactor = (4f / 3f) * Mathf.Tan(anglePerSegment / 4f);

        for (int segment = 0; segment < count; segment++)
        {
            float a0 = start + segment * anglePerSegment;
            float a1 = a0 + anglePerSegment;
            float y0 = segment * heightPerSegment;
            float y1 = y0 + heightPerSegment;

            Vector3 p0 = center + new Vector3(radius * Mathf.Cos(a0), y0, radius * Mathf.Sin(a0));
            Vector3 p3 = center + new Vector3(radius * Mathf.Cos(a1), y1, radius * Mathf.Sin(a1));
            Vector3 tangent0 = new Vector3(-Mathf.Sin(a0), 0f, Mathf.Cos(a0));
            Vector3 tangent1 = new Vector3(-Mathf.Sin(a1), 0f, Mathf.Cos(a1));
            Vector3 p1 = p0 + radius * handleFactor * tangent0 + Vector3.up * (heightPerSegment / 3f);
            Vector3 p2 = p3 - radius * handleFactor * tangent1 - Vector3.up * (heightPerSegment / 3f);

            // Each segment shares its first point with the previous segment's last point.
            if (segment == 0) SetPoint(0, p0);
            int i = segment * 3;
            SetPoint(i + 1, p1);
            SetPoint(i + 2, p2);
            SetPoint(i + 3, p3);
        }

        path.points = generatedPoints.ToArray();
        path.BuildDistanceTable();
    }

    private void SetPoint(int index, Vector3 worldPosition)
    {
        Transform point = generatedPoints[index];
        point.name = "Spiral Point " + index.ToString("00");
        point.position = worldPosition;
    }
}
