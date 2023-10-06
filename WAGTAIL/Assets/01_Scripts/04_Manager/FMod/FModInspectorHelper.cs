using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************************************
 *   �ν����Ϳ��� FMod Event�� ����� �� �ִ� ����� �����մϴ�...
 * ***/
public sealed class FModInspectorHelper : MonoBehaviour
{
    //===============================================
    /////        Property and Fields             ////
    //===============================================
    [SerializeField] FMODUnity.EventReference EventRef;
    [SerializeField] FModParameterReference   ParamRef;
    [SerializeField] Vector3                  EventPos;

    private int _EventRef = -1;



    //=======================================
    /////        Public methods         /////
    //=======================================
    public void SetEventPosition( Vector3 position )
    {
        EventPos = position;
    }

    public void SetLocalParameterType( FModLocalParamType localParamType)
    {
        ParamRef.SetParameter(localParamType);
    }

    public void SetGlobalParameterType( FModGlobalParamType globalParamType )
    {
        ParamRef.SetParameter(globalParamType);
    }

    public void SetParameterValue( float value )
    {
        ParamRef.SetParameter((FModLocalParamType)ParamRef.ParamType, value);
    }

    public void PlayBGMEvent( FModBGMEventType bgmEvent )
    {
        FModAudioManager.PlayBGM(bgmEvent, ParamRef);
    }

    public void PlaySFXEvent(  FModSFXEventType sfxEvent )
    {
        FModAudioManager.PlayOneShotSFX(sfxEvent, ParamRef, EventPos);
    }

    public void PlayBGMEvent()
    {
        FModAudioManager.PlayBGM(EventRef, ParamRef);
    }

    public void PlaySFXEvent()
    {
        FModAudioManager.PlayOneShotSFX(EventRef, EventPos, ParamRef );
    }
}
