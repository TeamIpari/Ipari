using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 *   �÷��̾��� �ִϸ��̼� �̺�Ʈ�� �����ִ� ������Ʈ�Դϴ�.
 * *****/
public sealed class PlayerAnimationHelper : MonoBehaviour
{
    //==============================================
    //////          Public methods            //////
    //==============================================
    public void PlayWalkSound()
    {
        #region Omit
        /**���� ���� �κ��� ȯ�� �Ķ���͸� �����´�...*/
        FModParameterReference paramRef = IpariUtility.GetFloorSFXType( transform.position, 
                                                                        ~(1 << LayerMask.NameToLayer("Player") ));

        /**�÷��̾ �ȴ� �Ҹ��� ����Ѵ�...*/
        FModAudioManager.PlayOneShotSFX( FModSFXEventType.Player_Walk, 
                                         paramRef, 
                                         transform.position, 
                                         5f, 
                                         200 );
        #endregion
    }


}
