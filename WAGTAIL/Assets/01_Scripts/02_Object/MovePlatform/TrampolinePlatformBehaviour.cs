using IPariUtility;
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
        //Debug.Log($"{exitTarget.name}");
        if (parentMushroom != null) 
        {
            parentMushroom.ChangeMushroom();
            parentMushroom.isMush = true;
        }
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit

        if (standingBody == null)
        {
            FModAudioManager.PlayOneShotSFX(FModSFXEventType.Mushroom_Jump);
            Player.Instance.animator.SetTrigger("flight");
            Player.Instance.movementSM.currentState.gravityVelocity.y = 0;
            Player.Instance.movementSM.currentState.gravityVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * Player.Instance.gravityValue);
        }
        else
        {
            Debug.Log(affectedPlatform.name);
            standingBody.velocity = IpariUtility.CaculateVelocity(affectedPlatform.transform.position + IpariUtility.RandomDirection() * 4f, standingPoint, 2f);
        }

        #endregion
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {

    }
}
