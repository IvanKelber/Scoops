using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQuad : MonoBehaviour
{

    public Color color;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    
    public Grid grid;
    public Material material;
    private void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
    }

    public void SetColor(Color color) {
        this.color = color;
    }

    public void Render(Vector3 position)
    {
        meshRenderer.sharedMaterial = material;
        meshRenderer.material.color = color;
        meshFilter.mesh = new Mesh();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = GetVertices(position);
        mesh.triangles = GetTriangles();
        Vector3[] normals = new Vector3[4]
        {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
        };
        mesh.uv = uv;
    }

    private Vector3[] GetVertices(Vector3 position)
    {
        float laneWidth = grid.laneWidth;
        float rowHeight = grid.rowHeight;
        Vector3[] vertices = new Vector3[4]
        {
                transform.InverseTransformPoint(new Vector3(position.x - laneWidth/2, position.y - rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x + laneWidth/2, position.y - rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x - laneWidth/2, position.y + rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x + laneWidth/2, position.y + rowHeight/2, 0))
        };
        return vertices;
    }

    private int[] GetTriangles()
    {
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        return tris;
    }

    // public void MoveQuad(int lane) {
        
    // }

}
