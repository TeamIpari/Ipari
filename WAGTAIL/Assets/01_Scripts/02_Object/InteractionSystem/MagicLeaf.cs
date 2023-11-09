using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

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
        SceneLoader.GetInstance().LoadScene("Boss_Crap_FINAL_Front");
        return true;
        #endregion
    }
}
