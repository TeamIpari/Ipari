using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 *   �÷��̾��� �ִϸ��̼� �̺�Ʈ�� �����ִ� ������Ʈ�Դϴ�.
 * *****/
public sealed class PlayerAnimationHelper : MonoBehaviour
{
    //===============================================
    ///////              Fields                //////
    //===============================================
    private Animator _anim;



    //================================================
    ///////            Magic methods           ///////
    //================================================
    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }




    //==============================================
    //////          Public methods            //////
    //==============================================
    public void PlayWalkSound()
    {
        #region Omit
        if (_anim.GetFloat("speed") < .15f) return;

        /**���� ���� �κ��� ȯ�� �Ķ���͸� �����´�...*/
        FModParameterReference paramRef = IpariUtility.GetFloorSFXType( transform.position, 
                                                                        ~(1 << LayerMask.NameToLayer("Player") ));

        /**�÷��̾ �ȴ� �Ҹ��� ����Ѵ�...*/
        FModAudioManager.PlayOneShotSFX( FModSFXEventType.Player_Walk, 
                                         paramRef, 
                                         transform.position, 
                                         8f, 
                                         400 );
        #endregion
    }

    public void PlayerLanding()
    {
        var player = Player.Instance;
        if (player.isCarry) player.movementSM.ChangeState(player.carry);
        else if (player.isIdle) player.movementSM.ChangeState(player.idle);
    }


}
