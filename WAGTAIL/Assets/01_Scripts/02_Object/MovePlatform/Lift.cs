using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>

public class Lift : MonoBehaviour
{

    [SerializeField] private float gravity;
    [SerializeField] private float addGravity;
    [SerializeField] private GameObject door;
    [Header("탐색 범위")]
    [SerializeField] private Vector3 search_range = new Vector3(6, 6, 6);

    private BoxCollider[] cols;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        ////Vector3 Dummy = new Vector3(search_range.x * this.transform.lossyScale.x,
        ////    search_range.y * this.transform.lossyScale.y,
        ////    search_range.z * this.transform.lossyScale.z);
        Vector3 Dummy = new Vector3(search_range.x,
            search_range.y,
            search_range.z);
        Gizmos.DrawWireCube(Vector3.zero, Dummy);
    }

    private void Start()
    {
        cols = GetComponents<BoxCollider>();
        cols[0].size = new Vector3(1, 1, 1);
        cols[1].size = search_range;

    }


    private void Update()
    {
        LifePlayer();
    }

    private void LifePlayer()
    {
        if (transform.childCount > 0 && !isGround())
        {
            transform.position += Vector3.down * ((gravity * Time.deltaTime) / 2);
            door.transform.position += Vector3.up * ((gravity * Time.deltaTime) / 2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag( "Player"))
            {
                
                // Player tag를 가진 GameObject는 interactor를 가지고 있습니다.
            }

        }
        catch
        {
        }
    }

    private void OnTriggerStay(Collider other)
    {
        try
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Interactor inter = other.GetComponent<Interactor>();
                inter.player.currentInteractable.GetComponent<SThrow>().SetPosHeight(this.transform);
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
            if (collision.gameObject.CompareTag( "interactable"))
            {
                collision.gameObject.GetComponent<SThrow>().Throwing();
                collision.gameObject.transform.parent = this.transform;

                if(transform.childCount < 3)
                {
                    gravity += addGravity;
                }

            }

        }
        catch
        {
        }
    }

    private bool isGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {

            if ((this.transform.position.y - (transform.localScale.y * .5f) - hit.transform.position.y ) <= 0)
            {
                return true;
            }

            //Debug.DrawRay(transform.position, (transform.rotation * Vector3.down * (transform.localScale.y )) * hit.distance, Color.red);
        }
        else
        {
            //Debug.DrawRay(transform.position, (transform.rotation * Vector3.down * (transform.localScale.y / 2)) * 1000f, Color.red);
        }
        return false;
    }
}




