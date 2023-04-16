using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Throw_Test : MonoBehaviour, IInteractable
{

    [Header("Interactable Setting")]
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoiont;
    [SerializeField] GameObject _playerInteractionPoint;


    public string InteractionPrompt => throw new System.NotImplementedException();

    [Header("A_to_B 이동 범위")]
    public Vector3 _startPos = Vector3.zero;
    public Vector3 _targetPos = Vector3.zero;
    public Vector3 _heightPos = Vector3.zero;

    [Header("Throw_Inspector")]
    [Tooltip("")]
    public float _height = 0f;
    public float _speed = 0f;
    [Range(0.0f, 1.0f)]
    public float _value = 0; 
    

    


    public bool Interact(Interactor interactor)
    {
        if (_playerEquipPoiont.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle)
        {
            PickUp();
            interactor.player.isCarry = true;
            return true;
        }
        else if(interactor.player.isCarry &&
            interactor.player.movementSM.currentState == interactor.player.carry )
        {

            return false;

        }
        return false;

    }

    public void PickUp()
    {

    }

    public void Throwing()
    {

    }
}
