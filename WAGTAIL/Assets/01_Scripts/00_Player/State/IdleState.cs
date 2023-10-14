using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private Vector3    _slopeSlideVelocity;
    private GameObject _FXMove;

    private static readonly Collider[] _colliders = new Collider[3];


    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        player.isIdle = true;
        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;
        
        player.animator.SetTrigger(Move);
        // FX
        // 임시로 넣어둔것이니 FX Manager가 완성되면 필히 수정해야함
        _FXMove = player.MoveFX;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered) player.isJump = true;
        if (interactAction.triggered && !HoldNearestObject()) player.Interaction();

        GetMoveInput();

        _FXMove.SetActive(input != Vector2.zero);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        player.animator.SetFloat(Speed, input.magnitude, player.speedDampTime, Time.deltaTime);

        if (player.isJump)
        {
            stateMachine.ChangeState(player.jump);
        }

        if (player.isPickup)
        {
            stateMachine.ChangeState(player.pickup);
        }

        if (player.isFlight)
        {
            stateMachine.ChangeState(player.flight);
        }

        if (player.isDead)
        {
            stateMachine.ChangeState(player.death);
        }

        // TODO : Idle 상태일때 추락 후 땅에 닿았을 때 landingState호출 해주기
    }

    public override void PhysicsUpdate()
    { 
        base.PhysicsUpdate();
        
        Movement(player.playerSpeed);
    }

    public override void Exit()
    {
        base.Exit();
        
        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
        
        _FXMove.SetActive(false);
        
        FModAudioManager.PlayOneShotSFX(
            FModSFXEventType.Player_Walk,
            player.transform.position
        );

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    public void Jumping()
    {
        player.isJump = true;
    }
    
    // 슬라이딩 로직
    private void SetSlopeSlideVelocity()
    {
        if (Physics.Raycast(player.transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 5f))
        {
            var angle = Vector3.Angle(hitInfo.normal, Vector3.up);
            
            if(angle >= player.controller.slopeLimit)
            {
                _slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, player.gravityValue * Time.deltaTime,0), hitInfo.normal);
                return;
            }
        }
        
        _slopeSlideVelocity = Vector3.zero;
    }

    private bool HoldNearestObject()
    {
        #region Omit

        CharacterController playerCon = player.controller;

        /***************************************
         *   계산에 필요한 요소들을 모두 구한다....
         * ******/
        float height            = playerCon.height;
        float radius            = playerCon.radius;
        float heightHalfOffset  = (height * .5f) - (radius);
        Vector3 playerPos       = playerCon.transform.position;
        Vector3 center          = (playerPos + playerCon.center);
        Vector3 point1          = center + (Vector3.up * heightHalfOffset);
        Vector3 point2          = center + (Vector3.down * heightHalfOffset);

        
        /**************************************************
         *   플레이어와 접촉한 Pullable collider를 탐지한다...
         * ******/

        /**주변을 탐색한다....*/
        int hitCount = Physics.OverlapCapsuleNonAlloc(
            point1,
            point2,
            radius,
            _colliders,
            (1 << LayerMask.NameToLayer("Pullable"))
        );

        /**탐지되었을 경우, 최상위 객체를 고른다....*/
        if (hitCount>0){

            player.movementSM.ChangeState(player.pullInout);
            player.pullInout.HoldTarget(_colliders[0].gameObject);
            return true;
        }

        return false;
        #endregion
    }
}
