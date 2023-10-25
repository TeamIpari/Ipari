using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CutSceneArea : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        setMeshData();
        createProceduralMesh();
    }

    void setMeshData()
    {
        vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0 , 1f, 0),
            new Vector3(1, 0, 0),

            new Vector3(1,0 , 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };

        triangles = new int[] { 0, 1, 2 };
    }

    void createProceduralMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
