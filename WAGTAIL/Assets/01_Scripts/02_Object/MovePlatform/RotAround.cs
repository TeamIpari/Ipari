using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


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
    public int polygon = 3;
    public float size = 1.0f;
    public Vector3 offset = new Vector3(0, 0, 0);
    public GameObject CreateObj1;
    public GameObject CreateObj2;

    [SerializeField] List<GameObject> objs = new List<GameObject>();
    //Mesh mesh;
    Vector3[] vertices;

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

        setMeshData(size, polygon);
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

    private void OnTriggerStay(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Interactor inter = other.GetComponent<Interactor>();
                inter.player.currentInteractable.GetComponent<SThrow>().SetPosHeight(this.transform);
            }
        }
        catch
        {
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag("Player"))
            {

                // Player tag를 가진 GameObject는 interactor를 가지고 있습니다.
            }
        }
        catch
        {
        }
    }


    private void OnTriggerExit(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Interactor inter = other.GetComponent<Interactor>();
                inter.player.currentInteractable.GetComponent<SThrow>().SetPosHeight(null);

            }
        }
        catch
        {
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            // 탄환이 적중하였을 때... 움직임을 정지하고 tag를 변경, 시킬 예정.
            if (collision.gameObject.CompareTag("interactable"))
            {
                collision.gameObject.GetComponent<SThrow>().Throwing();
                collision.gameObject.transform.parent = this.transform;
                if (transform.childCount < 3)
                {
                    Speed += AddGravity;
                }
            }
        }
        catch
        {
        }
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

        vertices[0] = new Vector3(0, 0, 0) + offset;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;

            vertices[i]
                = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * size) + offset;

            if (CreateObj2 != null)
                CreateObj = i % 2 == 1 ? CreateObj1 : CreateObj2;
            else
                CreateObj = CreateObj1;
            GameObject obj = Instantiate(CreateObj);

            createPos = vertices[i] + transform.position;

            obj.transform.position = obj.CompareTag("Coin") == true ? createPos + Vector3.up * 0.5f : createPos;
            
            obj.transform.LookAt(this.transform);
            w_Vec = Reverse == true ? 75 : -75;
            obj.transform.Rotate(0, w_Vec, 0);
            obj.transform.parent = this.transform;
            objs.Add(obj);
        }
    }
}
