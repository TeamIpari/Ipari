using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Profiling;
using UnityEngine;

public class PickUpState : State
{
    private float _timePassed;
    private const float PickUpTime = 0.1f;
    private const float PickUpDelayTime = 0.3f;
    
    private GameObject _currentInteractable;
    private Transform _currentInteractableTransform;
    private Collider _currentInteractableCollider;
    private Rigidbody _currentInteractableRigidbody;

    // Properties for Bezier Curve
    private Vector3 _startPos;
    private Vector3 _height;
    private Vector3 _endPos;
    

    public PickUpState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        
        _currentInteractable = player.currentInteractable;
        _currentInteractableTransform = _currentInteractable.GetComponent<Transform>();
        _currentInteractableCollider = _currentInteractable.GetComponent<Collider>();
        _currentInteractableRigidbody = _currentInteractable.GetComponent<Rigidbody>();
        
        _timePassed = 0f;
        player.animator.SetFloat(Speed, 0);
        player.animator.SetTrigger(PickUp);
        player.StartCoroutine(PickUpObject(0.1f, PickUpTime));
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // 물건을 들고 일정 시간 후 현재 상태를 carry(들고 움직이기)로 바꿔줌.
        if(player.isCarry)
        {
            stateMachine.ChangeState(player.carry);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        player.isPickup = false;
    }

    private IEnumerator PickUpObject(float lerpTime, float pickUpTime)
    {
        var pos = _currentInteractableTransform.position;
        var currentTime = 0.0f;

        while (currentTime < lerpTime)
        {
            _currentInteractableTransform.position = Vector3.Lerp(pos, player.InteractionPoint.position, currentTime / lerpTime);
            currentTime += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        yield return new WaitForSecondsRealtime(PickUpDelayTime);
        
        // BezierCurve를 위한 3개의 점 구하기 StartPos, Height, EndPos
        _startPos = _currentInteractableTransform.position;
        var lookVec = (player.transform.position - _currentInteractableTransform.position).normalized;
        _height = new Vector3(_startPos.x, player.ThrowEquipPoint.position.y - 0.5f, _startPos.z) + lookVec * 0.5f;
        _endPos = player.ThrowEquipPoint.position;
        _endPos.y -= 0.25f;
        
        // 머리 위로 드는 곡선을 그리는 코루틴
        currentTime = 0.0f;
        while (currentTime < pickUpTime)
        {
            _currentInteractableTransform.position = BezierCurve(_startPos, _endPos, _height, currentTime / pickUpTime);
            currentTime += Time.deltaTime;
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        _currentInteractableTransform.SetParent(player.Head.transform);
        player.isCarry = true;
    }
    
    private Vector3 BezierCurve(Vector3 startPos, Vector3 endPos, Vector3 height, float value)
    {
        var a = Vector3.Lerp(startPos, height, value);

        var b = Vector3.Lerp(height, endPos, value);

        var c = Vector3.Lerp(a, b, value);
 
        return c;
    }
}
