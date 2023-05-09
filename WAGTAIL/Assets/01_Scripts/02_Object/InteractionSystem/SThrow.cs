using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class SThrow : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => throw new System.NotImplementedException();
    GameObject _playerEquipPoint;
    GameObject _playerInteractionPoint;

    GameObject _playerLeftHand;
    GameObject _playerRightHand;

    // ���� ����. (�������ؼ� ��.)
    Transform startPos;
    // �� ����. (Ÿ��)
    // A to B �������� �̸� ������ �� ���� ���ΰ�?
    Transform endPos;
    // ���̴� ����. 
    GameObject heightPos;

    public bool flying = false;
    float _value = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;

    [Header("���� ��ġ")]
    [Range(1f, 5f)]
    [Tooltip("���̴� ��ġ�� ��������")]
    public float _pointHeight = 3.5f;
    [Range(0.0f,1f )]
    public float _pointPersent = 0.8f;
    [Range(5, 10)]
    public float speed = 1.0f;

    public float _force;
    public float _yForce = 1.0f;

    private void Start()
    {
        //_playerForwardTransform = Player.Instance
        startPos = Player.Instance.transform;
        flying = false;
        _playerEquipPoint = Player.Instance.RightHandPoint.gameObject;
        _playerInteractionPoint = Player.Instance.InteractionPoint.gameObject;
        _playerRightHand = Player.Instance.RightHand.gameObject;
    }

    private void Update()
    {
        if (flying)
        {
            //���ư���();
            transform.position = BezierCurve();
            _value += speed * 0.001f;
            flying = _value < 1.0f ? true : false;
            //Debug.Log(GetAngle());
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
        GetComponent<Rigidbody>().isKinematic = true;
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
        Player.Instance.currentInteractable = null;

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;


        Debug.Log(GetAngle());
        //Debug.Log(interactor.player.transform.forward);
        Vector3 nV3 = new Vector3(endPos.position.x, Player.Instance.transform.position.y, endPos.position.z);

        Player.Instance.gameObject.transform.LookAt(nV3);

        _playerForwardTransform = interactor.player.transform.forward;
        _playerForwardTransform.x *= _force;
        _playerForwardTransform.y = _yForce * (GetAngle()) ;
        _playerForwardTransform.z *= _force;

        GetComponent<Rigidbody>().AddForce(_playerForwardTransform);
        // ��¼�ٺ��� �����ε� ��.
        //Throwing();
        // ������ ��ŸƮ
        // Object ������ Ǯ����

    }

    IEnumerator SetPoint()
    {
        yield return new WaitForSeconds(1.5f);

        // interactionPoint�� ������ �ű�.
        _nomalInteractionPoint = _playerInteractionPoint.transform.localPosition;
        _playerInteractionPoint.transform.localPosition = _playerEquipPoint.transform.localPosition;
        
    }

    Vector3 BezierCurve()
    {
        Vector3 A = Vector3.Lerp(startPos.position, heightPos.transform.position, _value);

        Vector3 B = Vector3.Lerp(heightPos.transform.position, endPos.position, _value);

        Vector3 C = Vector3.Lerp(A, B, _value);

        return C;
    }

    float GetAngle()
    {
        try
        {
            Vector3 v3 = heightPos.transform.position - startPos.position;

            return 180 / (180 - Mathf.Atan2(v3.y, v3.x) * Mathf.Rad2Deg);
        }
        catch
        {
            return 0;
        }
    }

    public void Throwing()
    {
        if(heightPos != null && endPos != null)
            flying = flying == true ? false : true;
        //Destroy(heightPos.gameObject);
    }

    public void SetPosHeight(Transform tf)
    {
        endPos = tf;
        if(endPos != null && !flying)
        {
            GetHieght();
            //Debug.Log(GetAngle());
        }
    }

    public Transform GetHieght()
    {
        Vector3 dir = endPos.position - startPos.position;

        Quaternion q = Quaternion.LookRotation(dir);

        transform.rotation = q;
        
        Vector3 _vec = transform.position + (transform.forward * (Vector3.Distance(endPos.position, startPos.position) * _pointPersent));    // �ۼ�Ƽ���� ���.

        if(heightPos == null)
        {
            heightPos = new GameObject("name");
        }

        heightPos.transform.rotation = q;
        heightPos.transform.position = _vec + Vector3.up * _pointHeight;

        return heightPos.transform;
    }

    public bool AnimEvent()
    {
        Throwing();
        Player.Instance.isCarry = false;
        return false;
    }
}
