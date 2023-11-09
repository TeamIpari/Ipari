using IPariUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 *   플레이어의 애니메이션 이벤트를 도와주는 컴포넌트입니다.
 * *****/
public sealed class PlayerAnimationHelper : MonoBehaviour
{
    //==============================================
    //////          Public methods            //////
    //==============================================
    public void PlayWalkSound()
    {
        #region Omit
        /**현재 밟은 부분의 환경 파라미터를 가져온다...*/
        FModParameterReference paramRef = IpariUtility.GetFloorSFXType( transform.position, 
                                                                        ~(1 << LayerMask.NameToLayer("Player") ));

        /**플레이어가 걷는 소리를 재생한다...*/
        FModAudioManager.PlayOneShotSFX( FModSFXEventType.Player_Walk, 
                                         paramRef, 
                                         transform.position, 
                                         5f, 
                                         200 );
        #endregion
    }


}
