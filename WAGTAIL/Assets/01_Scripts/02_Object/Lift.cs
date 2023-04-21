using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] List<Transform> _hitPoint = new List<Transform>();
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
                inter.player.currentInteractable.GetComponent<SThrow>().��_����������(this.transform);
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
                inter.player.currentInteractable.GetComponent<SThrow>().��_����������(null);

            }
        }
        catch
        {
            Debug.Log("SThrow is notting");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       // źȯ�� �����Ͽ��� ��... �������� �����ϰ� tag�� ����, ��ų ����.
        
    }

    private Vector3 getPosition()
    {
        return transform.position;
    }

}
