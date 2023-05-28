using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class RotAround : MonoBehaviour, IEnviroment
{
    public Transform Tf;
    public bool Reverse = false;
    public float Speed = 1.0f;
    public float AddGravity = .5f;

    public string EnviromentPrompt => throw new System.NotImplementedException();


    public bool IsHit { get; set; }
    public bool Rot = false;
    public bool Interact()
    {
        Rot = true;
        return false;
    }
    // Start is called before the first frame update
    private void Start() 
    { 

    }

    // Update is called once per frame
    private void Update()
    {
        RotatePlatform();
        RotatePlayer();
    }

    public void RotatePlatform()
    {
        float temp = Speed;
        if (Reverse)
            temp *= -1 ;
        else
            temp *= 1;
        this.transform.RotateAround(Tf.position, Vector3.up, (temp * Time.deltaTime));

    }

    public void RotatePlayer()
    {
        if (Rot)
        {
            Player.Instance.controller.enabled = false;
            UpdatePlayerRotate();
            Player.Instance.controller.enabled = true;
            if (!Player.Instance.controller.isGrounded)
            {
                Rot = false;
            }
        }
    }

    private void UpdatePlayerRotate()
    {
        float temp = Speed;
        if (Reverse)
            temp *= -1;
        else
            temp *= 1;
        Player.Instance.transform.RotateAround(Tf.position, Vector3.up, (temp * Time.deltaTime));
    }

    private void OnTriggerStay(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag("Player"))
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
            if (other.gameObject.CompareTag("Player"))
            {

                // Player tag�� ���� GameObject�� interactor�� ������ �ֽ��ϴ�.
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
            // źȯ�� �����Ͽ��� ��... �������� �����ϰ� tag�� ����, ��ų ����.
            if (collision.gameObject.CompareTag("interactable"))
            {
                collision.gameObject.GetComponent<SThrow>().Throwing();
                collision.gameObject.transform.parent = this.transform;
                if (transform.childCount < 3)
                {
                    Speed += AddGravity;
                }
            }
        }
        catch
        {
        }
    }

}
