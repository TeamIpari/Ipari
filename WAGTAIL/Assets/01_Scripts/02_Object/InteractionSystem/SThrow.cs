using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class SThrow : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => throw new System.NotImplementedException();
    [SerializeField] GameObject _playerEquipPoint;
    [SerializeField] GameObject _playerInteractionPoint;

    [SerializeField] GameObject _playerLeftHand;
    [SerializeField] GameObject _playerRightHand;

    // ���� ����. (�������ؼ� ��.)
    Transform startPos;
    // �� ����. (Ÿ��)
    // A to B �������� �̸� ������ �� ���� ���ΰ�?
    public Transform endPos;
    // ���̴� ����. 
    GameObject heightPos;

    bool flying = false;
    float _value = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;

    private void Start()
    {
        //_playerForwardTransform = Player.Instance
        startPos = Player.Instance.transform;
        flying = false;
    }

    private void Update()
    {
        if (flying)
        {
            //���ư���();
            transform.position = ���ư���();
            _value += 0.005f;
            flying = _value < 1.0f ? true : false;
        }
    }

    public bool Interact(Interactor interactor)
    {
        if(interactor.player.movementSM.currentState == interactor.player.idle)
        {
            //interactor.player.isSmallThrow = true;
            interactor.player.isCarry = true;
            Pickup();

            return true;
        }
        else if (interactor.player.isCarry && interactor.player.movementSM.currentState == interactor.player.carry)
        {
            //if(interactor.)
            Throwing(interactor);
            //interactor.player.isSmallThrow = false;
            interactor.player.isCarry = false;
        }


        return false;
    }

    public void Pickup()
    {
        // 
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;

        // 
        transform.SetParent(_playerRightHand.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        StartCoroutine(SetPoint());
    }


    public void Throwing(Interactor interactor)
    {
        // interactionPoint�� Position�� �ʱ���·� �ǵ���
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;
        _playerRightHand.transform.DetachChildren();
        //Player.Instance.currentInteractable = null;
        
        // ������ ��ŸƮ
        //Debug.Log("��, ������!");
        // Object ������ Ǯ����

    }

    IEnumerator SetPoint()
    {
        yield return new WaitForSeconds(1.5f);

        // interactionPoint�� ������ �ű�.
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
        
    }

    Vector3 ���ư���()
    {
        Vector3 A = Vector3.Lerp(startPos.position, heightPos.transform.position, _value);

        Vector3 B = Vector3.Lerp(heightPos.transform.position, endPos.position, _value);

        Vector3 C = Vector3.Lerp(A, B, _value);

        return C;
    }

    public void ������ʹٽ���()
    {
        flying = flying == true ? false : true;
    }

    public void ��_����������(Transform tf)
    {
        endPos = tf;
        if(endPos != null && !flying)
        {
            ���̸�_�����ٰ�();
        }
    }

    public Transform ���̸�_�����ٰ�()
    {
        Vector3 dir = endPos.position - startPos.position;
        //Debug.Log(dir);

        Quaternion q = Quaternion.LookRotation(dir);

        transform.rotation = q;
        
        //Vector3.Distance(endPos.position, startPos.position)

        Vector3 _vec = transform.position + (transform.forward * (Vector3.Distance(endPos.position, startPos.position) * .8f));    // �ۼ�Ƽ���� ���.

        if(heightPos == null)
        {
            heightPos = new GameObject("name");
        }
        //heightPos.transform.position = ;
        heightPos.transform.rotation = q;
        heightPos.transform.position = _vec + heightPos.transform.up * 3.5f;

        return heightPos.transform;
    }
}
