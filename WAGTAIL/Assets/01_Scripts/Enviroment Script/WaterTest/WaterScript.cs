using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaterScript : MonoBehaviour
{
    public LayerMask PlayerLayer;
    [Header("수치값들")]
    [Tooltip("가해지는 힘")]
    public float ForceX;
    public float ForceZ;
    [Range(00.0f, 1f)]
    [Tooltip("")]
    public float WaterForce;
    private float defualtVal = 0.0f;
    

    [Tooltip("")]
    public float R;


    // Start is called before the first frame update
    private void Start()
    {
        defualtVal = Player.Instance.jumpHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Player>().jumpHeight = 0.2f;
            SoundTest.GetInstance().PlaySound("isThrowWater");
        }
        //else if(/*other.gameObject.layer == LayerMask.GetMask("Interactable"*/))
        else if (LayerMask.NameToLayer("Interactable") == other.gameObject.layer)
        {
            //Debug.Log(other.gameObject.layer);
            //other.gameObject.layer = LayerMask.NameToLayer("Default");
            //other.GetComponent<IInteractable>() = false;
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            SoundTest.GetInstance().PlaySound("isThrowWater");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.gameObject.layer);
        // PlayerMask만 체크하여 이동을 시킴.
        if(other.gameObject.CompareTag( "Player"))
        {
            //Debug.Log("ABC");
            
            other.GetComponent<CharacterController>()?.Move(new Vector3(ForceX, 0, ForceZ) * WaterForce);
 
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Player>().jumpHeight = defualtVal;
        }
    }

}
