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

    // 시작 지점. (어지간해선 손.)
    Transform startPos;
    // 끝 지점. (타겟)
    // A to B 지점으로 미리 지정을 해 놓을 것인가?
    Transform endPos;
    // 꺾이는 지점. 
    GameObject heightPos;

    bool flying = false;
    float _value = 0.0f;

    Vector3 _playerForwardTransform;
    Vector3 _nomalInteractionPoint;

    [Header("정점 위치")]
    [Range(1f, 5f)]
    [Tooltip("꺾이는 위치를 지정해줌")]
    public float _pointHeight = 3.5f;
    [Range(0.0f,1f )]
    public float _pointPersent = 0.8f;
    [Range(5, 10)]
    public float speed = 1.0f;

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
            //날아간다();
            transform.position = BezierCurve();
            _value += speed * 0.001f;
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
        // interactionPoint의 Position을 초기상태로 되돌림
        _playerInteractionPoint.transform.localPosition = _nomalInteractionPoint;
        _playerRightHand.transform.DetachChildren();
        Player.Instance.currentInteractable = null;

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().freezeRotation = true;

        // 던지기 스타트
        // Object 종속을 풀어줌

    }

    IEnumerator SetPoint()
    {
        yield return new WaitForSeconds(1.5f);

        // interactionPoint를 손으로 옮김.
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

    public void Throwing()
    {
        flying = flying == true ? false : true;
        //Destroy(heightPos.gameObject);
    }

    public void SetPosHeight(Transform tf)
    {
        endPos = tf;
        if(endPos != null && !flying)
        {
            GetHieght();
        }
    }

    public Transform GetHieght()
    {
        Vector3 dir = endPos.position - startPos.position;

        Quaternion q = Quaternion.LookRotation(dir);

        transform.rotation = q;
        
        Vector3 _vec = transform.position + (transform.forward * (Vector3.Distance(endPos.position, startPos.position) * _pointPersent));    // 퍼센티지로 계산.

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
