using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        try
        {
            if (other.gameObject.tag == "Player")
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

                StartCoroutine(GravityCall());
                //targetPos += Vector3.down * addGravity;
                //if (transform.childCount < 3)
                //{
                //    gravity += addGravity;
                //collision.gameObject.GetComponent<Rigidbody>().velocity;
                //}
                //transform.RotateAround();

            }

        }
        catch
        {
            Debug.Log("AA");
        }
    }

    IEnumerator GravityCall()
    {
        yield return new WaitForSeconds(1f);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;

    }

}
