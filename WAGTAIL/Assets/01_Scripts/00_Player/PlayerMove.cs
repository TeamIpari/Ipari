using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
//!!!!!!!!!!!!!!!!! ¥ı¿ÃªÛ æ»æ∏
public class PlayerMove : MonoBehaviour
{

    [SerializeField] public float Speed;

    private float vAxis;
    private float hAxis;

    Vector3 moveVec;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * Speed * Time.deltaTime;

        transform.LookAt(transform.position + moveVec);
    }
}
