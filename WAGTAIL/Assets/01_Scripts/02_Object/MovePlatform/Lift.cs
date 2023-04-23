using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lift : MonoBehaviour
{

    [SerializeField] float gravity;
    [SerializeField] float addGravity;
    [SerializeField] GameObject door;



    private void Start()
    {   
    }


    private void Update()
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
            if (other.gameObject.tag == "Player")
            {
                
                // Player tag�� ���� GameObject�� interactor�� ������ �ֽ��ϴ�.
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
            // źȯ�� �����Ͽ��� ��... �������� �����ϰ� tag�� ����, ��ų ����.
            if (collision.gameObject.tag == "interactable")
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
            Debug.Log("AA");
        }
    }

    bool isGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Debug.Log((this.transform.position.y - (transform.localScale.y) - hit.transform.position.y));
            if ((this.transform.position.y - (transform.localScale.y * .5f) - hit.transform.position.y ) <= 0)
            {
                return true;
            }

            Debug.DrawRay(transform.position, (transform.rotation * Vector3.down * (transform.localScale.y )) * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, (transform.rotation * Vector3.down * (transform.localScale.y / 2)) * 1000f, Color.red);
        }
        return false;
    }
}

    //private Vector3 getPosition()
    //{
    //    return transform.position;
    //}

