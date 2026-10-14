using UnityEngine;

/*
 * ProceduralQuadStarter builds the smallest piece of road: four vertices and two
 * triangles. A mesh is four parallel lists: vertices, triangles, UVs and
 * normals. RoadMeshGenerator fills the first three, one rung at a time, and
 * has Unity work out the normals from its triangles.
 */

public class ProceduralQuadStarter : MonoBehaviour
{
    public float width = 2f;
    public Material material;
    
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    
    void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        
        float halfWidth = width / 2f;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-halfWidth, -halfWidth, 0f);
        vertices[1] = new Vector3(halfWidth, -halfWidth, 0f);
        vertices[2] = new Vector3(-halfWidth, halfWidth, 0f);
        vertices[3] = new Vector3(halfWidth, halfWidth, 0f);

        int[] triangles = { 0, 2, 1, 1, 2, 3 };
        
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(1f, 0f);
        uvs[2] = new Vector2(0f, 1f);
        uvs[3] = new Vector2(1f, 1f);
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        _meshFilter.mesh = mesh;
    }
}
