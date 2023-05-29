using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class respawnManager : MonoBehaviour
{

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    //// Update is called once per frame
    //private void Update()
    //{
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag( "Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = Vector3.zero;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
