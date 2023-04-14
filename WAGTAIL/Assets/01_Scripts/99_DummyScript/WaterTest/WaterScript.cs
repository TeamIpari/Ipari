using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    
    public LayerMask _playerMask;
    [Header("��ġ����")]
    [Tooltip("�������� ��")]
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
        // PlayerMask�� üũ�Ͽ� �̵��� ��Ŵ.
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("ABC");
            other.GetComponent<CharacterController>().Move(new Vector3(_ForceX, 0, _ForceZ) * _vals);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Stone") // �ӽ� ��ũ��Ʈ
        {
            // �� �� ���� �������� <- ����

        }
    }
}
