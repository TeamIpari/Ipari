using System;
using System.Collections;
using UnityEngine;

public class Throw : MonoBehaviour, IInteractable
{
    [Header("Interactable Setting")]
    [SerializeField] private string _promt;
    private GameObject _playerEquipPoint;
    private GameObject _playerInteractionPoint;

    [Header("Throw Setting")]
    [SerializeField] float _force = 1.0f;
    [SerializeField] float _yForce = 1.0f;
    [Range(0, 1)]
    [SerializeField] float _yAngle = 0.45f;
    [SerializeField]
    float _value = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;
    public string InteractionPrompt => _promt;
    private Animator _animator;

    private void Start()
    {
        //_animator = GetComponent<Animator>();
        _playerEquipPoint = Player.Instance.ThrowEquipPoint.gameObject;
        _playerInteractionPoint = Player.Instance.InteractionPoint.gameObject;
    }

    public bool AnimEvent()
    {
        return false;
    }

    public bool Interact(Interactor interactor)
    {
        if(_playerEquipPoint.transform.childCount == 0 &&
            interactor.player.movementSM.currentState == interactor.player.idle)
        {
            StartCoroutine(Pickup());
            // isCarry를 isThrow로 바꿔줘야함
            interactor.player.isCarry = true;
            
            return true;
        }

        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            //Throwing(interactor);
            StartCoroutine(Throwing(interactor));
            interactor.player.isCarry = false;
            return true;
        }

        return false;
    }

    /*
    public void Pickup()
    {
        // Object가 Player의 머리 위에서 움직이는걸 방지
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().isKinematic = true;

        // Object를 Player의 머리 위로 옮김
        transform.SetParent(_playerEquipPoint.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        // interactionPoint를 머리 위로 옮김
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
    }
    */

    /*
    public void Throwing(Interactor interactor)
    {
        // interactionPoint의 Position을 초기상태로 되돌림
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;

        // Object 종속을 풀어줌
        _playerEquipPoint.transform.DetachChildren();

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        // 정한 방식대로 날라감
        _playerForwardTransform = interactor.player.transform.forward;
        _playerForwardTransform.x *= _force;
        _playerForwardTransform.y = _yForce * _yAngle;
        _playerForwardTransform.z *= _force;

        GetComponent<Rigidbody>().AddForce(_playerForwardTransform );
    }*/
    
    IEnumerator Pickup()
    {
        //_animator.enabled = true;
        //_animator.SetTrigger("pickup");
        // 여기에 Object 움직이는거 구현
        _value = 0;
        // Object가 Player의 머리 위에서 움직이는걸 방지
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(0.55f);

        while (_value <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            transform.transform.position = (BeziurCurve(_value));
            _value += 0.01f;
        }
        //yield return new WaitForSeconds(1.8f);

        // Object를 Player의 머리 위로 옮김
        transform.SetParent(_playerEquipPoint.transform);
        ////_animator.SetTrigger("idle");
        //transform.localPosition = Vector3.zero;
        //transform.rotation = new Quaternion(0, 0, 0, 0);

        // interactionPoint를 머리 위로 옮김
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
    }

    Vector3 BeziurCurve(float _value)
    {
        Vector3 height = new Vector3(_playerInteractionPoint.transform.position.x, _playerEquipPoint.transform.position.y , _playerInteractionPoint.transform.position.z );
        Vector3 top = new Vector3(_playerEquipPoint.transform.position.x, _playerEquipPoint.transform.position.y -0.5f, _playerEquipPoint.transform.position.z);
        //float _value = 0.5f;
        Vector3 A = Vector3.Lerp(_playerInteractionPoint.transform.position, height, _value);

        Vector3 B = Vector3.Lerp(height, top, _value);

        Vector3 C = Vector3.Lerp(A, B, _value);

        return C;
    }

    IEnumerator Throwing(Interactor interactor)
    {
        //_animator.enabled = false;
        yield return new WaitForSeconds(0.2f);
        // interactionPoint의 Position을 초기상태로 되돌림
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;

        // Object 종속을 풀어줌
        _playerEquipPoint.transform.DetachChildren();

        // 머리 위에서 움직이는걸 방지하기 위한 것들 해제
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        // 정한 방식대로 날라감
        _playerForwardTransform = interactor.player.transform.forward;
        _playerForwardTransform.x *= _force;
        _playerForwardTransform.y = _yForce * _yAngle;
        _playerForwardTransform.z *= _force;

        GetComponent<Rigidbody>().AddForce(_playerForwardTransform );
    }

}
