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
                inter.player.currentInteractable.GetComponent<SThrow>().아_높이정해줘(this.transform);
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
                inter.player.currentInteractable.GetComponent<SThrow>().아_높이정해줘(null);

            }
        }
        catch
        {
            Debug.Log("SThrow is notting");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       // 탄환이 적중하였을 때... 움직임을 정지하고 tag를 변경, 시킬 예정.
        
    }

    private Vector3 getPosition()
    {
        return transform.position;
    }

}
