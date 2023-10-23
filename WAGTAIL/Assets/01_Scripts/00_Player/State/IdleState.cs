using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**********************************************************************
 *   플레이어의 기본, 이동 상태에 대한 로직이 구현된 클래스입니다..
 * *****/
public class IdleState : State
{ 
    private struct InteractDesc
    {
       public IInteractable Interactable;
       public Collider      Collider;
    }

    //=================================================
    /////                 Fields                   ////
    //================================================
    private Vector3    _slopeSlideVelocity;
    private GameObject _FXMove;


    /**상호작용 관련 필드....*/
    private InteractDesc  _lastInteract;
    private float         _interactViewRadian;
    private LayerMask     _interactLayer;

    private static readonly Collider[] _colliders = new Collider[3];



    //=====================================================
    //////             Override methods              //////
    //====================================================

    public IdleState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        #region Omit
        player = _player;
        stateMachine = _stateMachine;

        _interactViewRadian = (Mathf.Deg2Rad * 60f);
        _interactLayer = LayerMask.GetMask("Interactable");
        #endregion
    }

    public override void Enter()
    {
        #region Omit
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
        #endregion
    }

    public override void HandleInput()
    {
        #region Omit
        base.HandleInput();

        if (jumpAction.triggered) player.isJump = true;
        
        InteractNearestObject();

        GetMoveInput();

        _FXMove.SetActive(input != Vector2.zero);
        #endregion
    }

    public override void LogicUpdate()
    {
        #region Omit
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
        #endregion
    }

    public override void PhysicsUpdate()
    { 
        base.PhysicsUpdate();
        
        Movement(player.playerSpeed);
    }

    public override void Exit()
    {
        #region Omit
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
        #endregion
    }



    //=====================================================
    //////              Core methdos                 //////
    //=====================================================

    public void Jumping()
    {
        player.isJump = true;
    }
    
    // 슬라이딩 로직
    private void SetSlopeSlideVelocity()
    {
        #region Omit
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
        #endregion
    }

    private bool InteractNearestObject()
    {
        #region Omit

        /*************************************************************
         *   플레이어와 접촉한 상호작용이 가능한 객체를 탐지한다...
         * ******/
        Transform playerTr = player.transform;

        Vector3 playerDir = playerTr.forward;
        Vector3 playerPos = playerTr.position;

        /**지정한 범위안의 객체들을 가져온다...*/
        int hitCount = Physics.OverlapSphereNonAlloc(

            playerPos,
            1.5f,
            _colliders,
            _interactLayer
        );

        Collider hitCollider = null;
        if (hitCount > 0)
        {
            /**플레이어 시야 안으로 들어온 것들을 탐색한다...*/
            for (int i = 0; i < hitCount; i++){

                Vector3 hitPos     = _colliders[i].bounds.center;
                Vector3 player2Hit = (hitPos - playerPos-(playerTr.forward*-1f)).normalized;

                float acos = Mathf.Abs(Mathf.Acos(Vector3.Dot(playerDir, player2Hit)));
                if (acos <= _interactViewRadian && hitPos.y>=playerPos.y)
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
            bool isChange = false;
            if(_lastInteract.Collider!=hitCollider)
            {
                _lastInteract.Collider     = hitCollider;
                _lastInteract.Interactable = hitCollider.GetComponent<IInteractable>();
                isChange = true;
            }

            #region Null_Test
#if UNITY_EDITOR
            if (_lastInteract.Interactable==null)
            {
                Debug.LogWarning($"경고!!! ({_lastInteract.Collider.name})의 interactable이 null임!!!!!");
                return false;
            }
#endif
            #endregion

            /**상호작용한 물체에 대한 UI를 표시한다....*/
            Vector3 popupPos = (hitCollider.bounds.center + _lastInteract.Interactable.InteractPopupOffset);
            InterativeUI.PopupUI(

                InterativeUI.ShowType.Visible,
                _lastInteract.Interactable.InteractionPrompt,
                popupPos,
                isChange
            );


            /**상호작용 키를 입력했을 경우....*/
            if (interactAction.triggered)
            {
                /**상호작용 UI를 비표시한다...*/
                InterativeUI.PopupUI(

                    InterativeUI.ShowType.InVisible,
                    InterativeUI.Text,
                    popupPos
                );

                /**상호작용에 성공시 상호작용 대상을 갱신한다...*/
                if(_lastInteract.Interactable.Interact(player.gameObject)){

                    player.currentInteractable = hitCollider.gameObject;
                }
            }

            return true;
        }


        /************************************
         *   탐지된 물체가 업을 경우....
         * *****/
        if(_lastInteract.Collider!=null)
        {
            /**상호작용 UI를 종료시킨다...*/
            InterativeUI.PopupUI(

                InterativeUI.ShowType.InVisible,
                InterativeUI.Text,
                InterativeUI.WorldPosition
            );

            _lastInteract.Collider     = null;
            _lastInteract.Interactable = null;
        }

        return false;
        #endregion
    }
}
