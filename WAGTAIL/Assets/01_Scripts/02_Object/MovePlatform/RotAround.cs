using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotAround : MonoBehaviour, IEnviroment
{
    [Header("Rotate Option")]
    public Transform Center;
    public bool Reverse = false;
    public float Speed = 1.0f;
    public float AddGravity = .5f;

    [Space(10f)]
    [Header("Create Rot Around Objects")]
    [Range(3, 30)]
    public int Polygon = 3;
    public float CircleSize = 1.0f;
    public Vector3 Offset = new Vector3(0, 0, 0);
    public GameObject CreateObj1;
    public GameObject CreateObj2;

    [SerializeField] List<GameObject> objs = new List<GameObject>();
    //Mesh mesh;
    [SerializeField] Vector3[] vertices;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }
    public bool Rot = false;
    public bool Interact()
    {
        Rot = true;
        return false;
    }

    // Start is called before the first frame update
    private void Start() 
    {
        if (Center == null)
            Center = this.transform;
        setMeshData(CircleSize, Polygon);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (objs.Count == 0) return;
        RotatePlatform();
        RotatePlayer();
    }

    public void RotatePlatform()
    {
        float temp = Speed;
        if (Reverse)
            temp *= -1 ;
        else
            temp *= 1;
        this.transform.RotateAround(Center.position, Vector3.up, (temp * Time.deltaTime));
    }

    public void RotatePlayer()
    {
        if (Rot)
        {
            Player.Instance.controller.enabled = false;
            UpdatePlayerRotate();
            Player.Instance.controller.enabled = true;
            if (!Player.Instance.controller.isGrounded)
            {
                Rot = false;
            }
        }
    }

    private void UpdatePlayerRotate()
    {
        float temp = Speed;
        if (Reverse)
            temp *= -1;
        else
            temp *= 1;
        Player.Instance.transform.RotateAround(Center.position, Vector3.up, (temp * Time.deltaTime));
    }

    void setMeshData(float size, int polygon)
    {
        GameObject CreateObj;
        float w_Vec;
        Vector3 createPos;
        for (int i = 0; i < objs.Count; i++)
            Destroy(objs[i]);
        objs.Clear();

        vertices = new Vector3[polygon + 1];

        vertices[0] = new Vector3(0, 0, 0) + Offset;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;

            vertices[i]
                = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * size) + Offset;

            if (CreateObj2 != null)
                CreateObj = i % 2 == 1 ? CreateObj1 : CreateObj2;
            else
                CreateObj = CreateObj1;
            GameObject obj = Instantiate(CreateObj);

            createPos = vertices[i] + transform.position;

            obj.transform.position = obj.CompareTag("Coin") == true ? createPos + Vector3.up * 1.5f : createPos;
            
            obj.transform.LookAt(this.transform);
            w_Vec = Reverse == true ? 75 : -75;
            obj.transform.Rotate(0, w_Vec, 0);
            obj.transform.parent = this.transform;
            objs.Add(obj);
        }
    }

    public void ExecutionFunction(float time)
    {
        Debug.Log("Not Have Function");
    }
}
