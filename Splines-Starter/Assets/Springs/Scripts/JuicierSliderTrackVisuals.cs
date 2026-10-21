using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class JuicierSliderTrackVisuals : MonoBehaviour
{
    public JuicierSlider slider;
    public Material material;

    [Header("Track Shape")]
    public float yOffset = -0.08f;
    public float slotMarkerHalfDepth = 0.06f;
    public float lineWidth = 0.025f;

    Mesh _mesh;

    int SlotCount => Mathf.Max(1, slider.slotCount);
    float SlotSpacing => SlotCount == 1 ? 0f : slider.travelHalfWidth * 2f / (SlotCount - 1);

    void Awake()
    {
        UpdateVisuals();
    }

    void OnValidate()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (slider == null)
            return;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null || meshRenderer == null)
            return;

        if (material != null)
            meshRenderer.sharedMaterial = material;

        if (_mesh == null)
        {
            _mesh = meshFilter.sharedMesh;
            if (_mesh == null)
            {
                _mesh = new Mesh { name = "Juicier Slider Track" };
                meshFilter.sharedMesh = _mesh;
            }
        }

        int segmentCount = SlotCount + 1;
        var vertices = new Vector3[segmentCount * 4];
        var triangles = new int[segmentCount * 6];

        Vector3 center = transform.InverseTransformPoint(slider.transform.position);
        center.y += yOffset;

        AddLineSegment(vertices, triangles, 0,
            center + new Vector3(-slider.travelHalfWidth, 0f, 0f),
            center + new Vector3(slider.travelHalfWidth, 0f, 0f));

        for (int i = 0; i < SlotCount; i++)
        {
            float x = SlotCount == 1 ? 0f : -slider.travelHalfWidth + SlotSpacing * i;
            Vector3 slotCenter = center + new Vector3(x, 0f, 0f);

            AddLineSegment(vertices, triangles, i + 1,
                slotCenter + new Vector3(0f, 0f, -slotMarkerHalfDepth),
                slotCenter + new Vector3(0f, 0f, slotMarkerHalfDepth));
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    void AddLineSegment(Vector3[] vertices, int[] triangles, int segmentIndex, Vector3 start, Vector3 end)
    {
        int vertexIndex = segmentIndex * 4;
        int triangleIndex = segmentIndex * 6;

        Vector3 direction = (end - start).normalized;
        Vector3 side = new Vector3(-direction.z, 0f, direction.x) * lineWidth * 0.5f;

        vertices[vertexIndex] = start - side;
        vertices[vertexIndex + 1] = start + side;
        vertices[vertexIndex + 2] = end + side;
        vertices[vertexIndex + 3] = end - side;

        triangles[triangleIndex] = vertexIndex;
        triangles[triangleIndex + 1] = vertexIndex + 1;
        triangles[triangleIndex + 2] = vertexIndex + 2;
        triangles[triangleIndex + 3] = vertexIndex;
        triangles[triangleIndex + 4] = vertexIndex + 2;
        triangles[triangleIndex + 5] = vertexIndex + 3;
    }
}
