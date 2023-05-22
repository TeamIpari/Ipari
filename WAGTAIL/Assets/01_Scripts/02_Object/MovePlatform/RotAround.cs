using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotAround : MonoBehaviour, IEnviroment
{
    public Transform tf;
    public float gravity = 1.0f;
    public float addGravity = .5f;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    bool _ishit = false;

    public bool _hit { get {return _ishit; } set { _ishit = value; } }
    public bool a = false;
    public bool Interact()
    {
        a = true;
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _hit = _ishit;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(tf.position, Vector3.up , (gravity * Time.deltaTime));
        if(a)
        {
            Player.Instance.controller.enabled = false;
            Player.Instance.transform.RotateAround(tf.position, Vector3.up,(gravity * Time.deltaTime));
            Player.Instance.controller.enabled = true;
            if (!Player.Instance.controller.isGrounded)
            {
                a = false;
                //Player.Instance.transform.SetParent(null);
            }
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
        }
    }

}
