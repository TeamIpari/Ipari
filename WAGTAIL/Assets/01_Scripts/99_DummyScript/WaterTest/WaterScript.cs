using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    
    public LayerMask _playerMask;
    [Header("수치값들")]
    [Tooltip("가해지는 힘")]
    public float _ForceX;
    public float _ForceZ;
    [Range(00.0f, 1f)]
    [Tooltip("")]
    public float _vals;
    

    [Tooltip("")]
    public float R; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_playerMask.value);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        // PlayerMask만 체크하여 이동을 시킴.
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("ABC");
            other.GetComponent<CharacterController>().Move(new Vector3(_ForceX, 0, _ForceZ) * _vals);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Stone") // 임시 스크립트
        {
            // 들 수 없게 만들어야함 <- 보류

        }
    }
}
