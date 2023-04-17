using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // PlayerMove를 위한 변수
    [SerializeField] public float _moveSpeed;

    public Vector2 _input = Vector2.zero;
    public Vector3 _velocity = Vector3.zero;

    public InputAction moveAction;

    private float _vAxis;
    private float _hAxis;

    Vector3 _moveVec;

    // Start is called before the first frame update
    void Start()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
    }

    private void Update()
    {
        _input = moveAction.ReadValue<Vector2>();
        _velocity = new Vector3(_input.x, 0, _input.y);
        Debug.Log(_velocity.normalized);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");

        _moveVec = new Vector3(_hAxis, 0, _vAxis).normalized;

        transform.position += _moveVec * _moveSpeed * Time.deltaTime;

        transform.LookAt(transform.position + _moveVec);

    }

    void Idle()
    {

    }

    void Push()
    {

    }
}
