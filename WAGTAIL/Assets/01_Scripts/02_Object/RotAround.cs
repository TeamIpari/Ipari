using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotAround : MonoBehaviour
{
    public Transform tf;
    public float gravity = 1.0f;
    public float addGravity = .5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0 && transform.localRotation.z > 0)
        {
            this.transform.RotateAround(tf.localPosition, Vector3.back, (gravity * Time.deltaTime));

        }
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

                // Player tag를 가진 GameObject는 interactor를 가지고 있습니다.
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
                if (transform.childCount < 3)
                {
                    gravity += addGravity;
                }
                //transform.RotateAround();

            }

        }
        catch
        {
            Debug.Log("AA");
        }
    }

}
