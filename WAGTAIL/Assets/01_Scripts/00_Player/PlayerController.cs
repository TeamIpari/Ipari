using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerMove를 위한 변수
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
        PlayerMove();
    }

    void PlayerMove()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * Speed * Time.deltaTime;

        transform.LookAt(transform.position + moveVec);
    }

}
