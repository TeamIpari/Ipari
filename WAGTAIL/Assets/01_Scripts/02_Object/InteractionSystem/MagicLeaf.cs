using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class MagicLeaf : MonoBehaviour, IInteractable
{
    #region Define
    [System.Serializable]
    public sealed class MagicLeafEvent : UnityEvent
    {
    }
    #endregion

    //===============================================
    //////               Property              //////
    //===============================================
    public string  InteractionPrompt   { get; set; } = "¾É±â";
    public Vector3 InteractPopupOffset { get; set; } = (Vector3.up*1.5f);
    public TimelineController timelineController;


    //=================================================
    //////            Override methods           //////
    //=================================================
    public bool AnimEvent()
    {
        /**None Implements.*/
        return false;
    }

    public bool Interact(GameObject interactor)
    {
        #region Omit
        if (!interactor.CompareTag("Player") || InteractPopupOffset!=(Vector3.up * 1.5f)) 
            return false;

        InteractPopupOffset = (Vector3.up*999999f);
        Player.Instance.stiffen.StiffenTime = -1f;
        Player.Instance.movementSM.ChangeState(Player.Instance.stiffen);
        timelineController.StartTimeline();
        return true;
        #endregion
    }
}
