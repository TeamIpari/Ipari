using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolinePlatformBehaviour : PlatformBehaviorBase
{


    //=======================================
    //////          Properties          /////
    //=======================================
    public float JumpHeight;
    public float SaveHieght;

    public Mushroom parentMushroom;

    //=======================================
    //////     Override Methods          ////
    //=======================================
    public override void BehaviorEnd(PlatformObject changedTarget)
    {

    }

    public override void BehaviorStart(PlatformObject affectedPlatform)
    {

    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody)
    {
        if (parentMushroom != null)
        {
            parentMushroom.ChangeMushroom();
            parentMushroom.isMush = true;
        }
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit
        // 여기서 시작
        // 밟았을 때 튀어 오르기.

        //Player.Instance.jumpHeight = JumpHeight;
        //#region 협의 후 변경할 예정.
        //if (Player.Instance.movementSM.currentState == Player.Instance.idle)
        //{
        //    Player.Instance.idle.Jumping();
        //}
        //else if (Player.Instance.movementSM.currentState == Player.Instance.flight)
        //{
        //    Player.Instance.flight.Jumping();
        //}
        //else if (Player.Instance.movementSM.currentState == Player.Instance.jump)
        //{
        //    Player.Instance.jump.Jumping();
        //}
        //#endregion
        Debug.Log($"Hit");
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Mushroom_Jump);
        Player.Instance.animator.SetTrigger("flight");
        Player.Instance.movementSM.currentState.gravityVelocity.y = 0;
        Player.Instance.movementSM.currentState.gravityVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * Player.Instance.gravityValue);

        #endregion
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {

    }
}
