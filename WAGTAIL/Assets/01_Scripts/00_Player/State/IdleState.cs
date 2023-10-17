using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private Vector3    _slopeSlideVelocity;
    private GameObject _FXMove;

    private Collider   _lastCollider, _lastCollider2;
    private float      _liftViewRadian;
        
    private static readonly Collider[] _colliders = new Collider[3];


    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player          = _player;
        stateMachine    = _stateMachine;
        _liftViewRadian = (Mathf.Deg2Rad*60f);
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
        if( !HoldNearestObject() && !LiftNearestObject())
        {
            /**상호작용 UI를 종료시킨다...*/
            InterativeUIPopup.PopupUI(

                InterativeUIPopup.ShowType.InVisible,
                InterativeUIPopup.Text,
                InterativeUIPopup.WorldPosition
            );
        }

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

    private bool LiftNearestObject()
    {
        #region Omit
        Transform playerTr = player.transform;

        /********************************************
         *    계산에 필요한 요소들을 모두 구한다...
         * *******/
        Vector3 playerDir = playerTr.forward;
        Vector3 playerPos = playerTr.position;



        /**************************************************
         *   플레이어와 접촉한 Pullable collider를 탐지한다...
         * ******/
        int hitCount = Physics.OverlapSphereNonAlloc(

            playerPos, 
            1.5f, 
            _colliders,
            (1<<LayerMask.NameToLayer("Interactable"))
        );

        Collider hitCollider = null;
        if (hitCount > 0)
        {
            /**플레이어 시야 안으로 들어온 것들을 탐색한다...*/
            for (int i = 0; i < hitCount; i++){

                Vector3 hitPos = _colliders[i].transform.position;
                Vector3 player2Hit = (hitPos - playerPos).normalized;

                float acos = Mathf.Abs(Mathf.Acos(Vector3.Dot(playerDir, player2Hit)));
                if (acos <= _liftViewRadian)
                {
                    hitCollider = _colliders[i];
                    break;
                }

            }
        }


        /************************************
         *   탐지된 물체가 존재할 경우... 
         * *****/
        if (hitCollider != null)
        {
            Vector3 popupPos = hitCollider.bounds.center + (Vector3.up * 1.5f);
            InterativeUIPopup.PopupUI(

                InterativeUIPopup.ShowType.Visible,
                "들어올린다.",
                popupPos,
                (_lastCollider2 != hitCollider)
            );

            _lastCollider2 = hitCollider;


            /**상호작용 키를 입력했을 경우....*/
            if (interactAction.triggered)
            {
                InterativeUIPopup.PopupUI(

                    InterativeUIPopup.ShowType.InVisible,
                    "들어올린다.",
                    popupPos
                );

                /**상호작용이 가능할 경우 상호작용을 한다....*/
                IInteractable target = hitCollider.GetComponent<IInteractable>();
                if(target!=null && target.Interact(player.gameObject))
                {
                    player.currentInteractable = hitCollider.gameObject;
                }
            }

            return true;
        }


        /************************************
         *   탐지된 물체가 업을 경우....
         * *****/
        _lastCollider2 = null;

        return false;
        #endregion
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
            (1<<LayerMask.NameToLayer("Pullable"))
        );


        /************************************
         *   탐지된 물체가 존재할 경우... 
         * *****/
        if (hitCount>0 )
        {
            Vector3 popupPos = _colliders[0].bounds.center + (Vector3.up*2f);
            InterativeUIPopup.PopupUI(

                InterativeUIPopup.ShowType.Visible, 
                "당긴다.",
                popupPos,
                (_lastCollider != _colliders[0])
            );

            _lastCollider = _colliders[0];


            /**상호작용 키를 입력했을 경우....*/
            if(interactAction.triggered)
            {
                InterativeUIPopup.PopupUI(
                    
                    InterativeUIPopup.ShowType.InVisible, 
                    "당긴다.",
                    popupPos
                );

                player.movementSM.ChangeState(player.pullInout);
                player.pullInout.HoldTarget(_colliders[0].gameObject);
            }
            return true;
        }


        /************************************
         *   탐지된 물체가 업을 경우....
         * *****/
        _lastCollider = null;

        return false;
        #endregion
    }
}
