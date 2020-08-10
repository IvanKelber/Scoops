using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderQuad : MonoBehaviour
{

    public Color color;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    
    public LaneHelper laneHelper;
    public Material material;
    private void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer.sharedMaterial = material;
        meshRenderer.material.color = color;
        meshFilter.mesh = new Mesh();
    }

    public void Render(Vector3 position)
    {
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

        // Color[] colors = new Color[mesh.vertices.Length];
        // for(int i = 0; i < mesh.vertices.Length; i++) {
        //     colors[i] = this.color;
        // }
        // mesh.colors = colors;
    }

    private Vector3[] GetVertices(Vector3 position)
    {
        Debug.Log(position);
        float laneWidth = laneHelper.laneWidth;
        float rowHeight = laneHelper.rowHeight;
        Vector3[] vertices = new Vector3[4]
        {
                transform.InverseTransformPoint(new Vector3(position.x - laneWidth/2, position.y - rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x + laneWidth/2, position.y - rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x - laneWidth/2, position.y + rowHeight/2, 0)),
                transform.InverseTransformPoint(new Vector3(position.x + laneWidth/2, position.y + rowHeight/2, 0))
        };
        foreach(Vector3 v in vertices) {
            Debug.Log(v);
        }
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
