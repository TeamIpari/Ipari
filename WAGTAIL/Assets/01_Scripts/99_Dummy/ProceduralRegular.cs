using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralRegular : MonoBehaviour
{
    [Range(3, 30)]
    public int polygon = 3;
    public float size = 1.0f;
    public Vector3 offset = new Vector3(0, 0, 0);
    public GameObject CreateObj1;
    public GameObject CreateObj2;

    List<GameObject> objs = new List<GameObject>();
    //Mesh mesh;
    Vector3[] vertices;
    //int[] triangles;

    void OnValidate()
    {
    }

    void Start()
    {

        setMeshData(size, polygon);
    }

    void setMeshData(float size, int polygon)
    {
        for (int i = 0; i < objs.Count; i++)
            Destroy(objs[i]);
        objs.Clear();

        vertices = new Vector3[polygon + 1];

        vertices[0] = new Vector3(0, 0, 0) + offset;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;

            vertices[i]
                = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * size) + offset;
            GameObject CreateObj;

            if (CreateObj2 !=null)
                CreateObj = i % 2 == 1 ? CreateObj1: CreateObj2 ;
            else
                CreateObj = CreateObj1;
            GameObject obj = Instantiate(CreateObj);

            obj.transform.position = vertices[i] + transform.position;
            obj.transform.LookAt(this.transform);
            obj.transform.Rotate(0, 45f, 0);
            obj.transform.parent = this.transform;
        }
    }

}
