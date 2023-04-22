using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] List<Transform> _hitPoint = new List<Transform>();
    [SerializeField] float gravity;
    [SerializeField] float addGravity;
    [SerializeField] private Vector3 targetPos;

    private float speed = 1.0f;
    private void Start()
    {
        targetPos = transform.position;    
    }


    private void Update()
    {
        if(transform.childCount > 0)
            transform.position += Vector3.down * ((gravity * Time.deltaTime) / 2);

    }
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.gameObject.tag == "Player")
            {
                
                // Player tag를 가진 GameObject는 interactor를 가지고 있습니다.
            }

        }
        catch
        {
            Debug.Log("SThrow is notting");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        try
        {
            if(other.gameObject.tag == "Player")
            {
                Interactor inter = other.GetComponent<Interactor>();
                inter.player.currentInteractable.GetComponent<SThrow>().SetPosHeight(this.transform);
            }
        }
        catch
        {
            Debug.Log("SThrow is notting");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        try
        { 
            if (other.gameObject.tag == "Player")
            {
                Interactor inter = other.GetComponent<Interactor>();
                inter.player.currentInteractable.GetComponent<SThrow>().SetPosHeight(null);

            }
        }
        catch
        {
            Debug.Log("SThrow is notting");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            // 탄환이 적중하였을 때... 움직임을 정지하고 tag를 변경, 시킬 예정.
            if (collision.gameObject.tag == "interactable")
            {
                collision.gameObject.GetComponent<SThrow>().Throwing();
                collision.gameObject.transform.parent = this.transform;
                //targetPos += Vector3.down * addGravity;
                if(transform.childCount < 3)
                {
                    gravity += addGravity;
                }

            }

        }
        catch
        {
            Debug.Log("AA");
        }
    }


    private Vector3 getPosition()
    {
        return transform.position;
    }

}
