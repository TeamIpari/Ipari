using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] Vector3 search_range = new Vector3(0, 0, 0);
    BoxCollider[] col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponents<BoxCollider>();
        col[0].size = col[0].size;
        col[1].size = search_range;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Vector3 Dummy = new Vector3(search_range.x,
            search_range.y,
            search_range.z );

        //Gizmos.DrawWireCube(this.transform.position, Dummy);
        Gizmos.DrawWireCube(Vector3.zero, Dummy);

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

        }
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
            Debug.Log("D");
        }
    }

    IEnumerator GravityCall()
    {
        yield return new WaitForSeconds(1f);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;

    }

}
