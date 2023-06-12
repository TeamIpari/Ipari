using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SayerCocosh : MonoBehaviour, IInteractable
{
    [SerializeField] private bool IsSaying = false;
    // 말을 하였는가? 하지 않았을 경우 !를 띄워줌.
    public bool IsSay = false;
    public int SayType = 1;     // 말해야하는 챕터마다 다름.
    public TextMeshProUGUI TextVeiwer;      // 필수
    public CutScene CutScenePlayer;         // 있으면 재생.

    public string InteractionPrompt => throw new System.NotImplementedException();

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }

    public bool Interact(Interactor interactor)
    {
        if(IsSay && !IsSaying)
        {
            Debug.Log("이미 한 번 실행함");
            return false;
        }
        else if(!IsSaying)
        {
            Player.Instance.playerInput.enabled = false;
            Debug.Log("Add Dic");
            LoadManager.GetInstance().SearchTypePoint(SayType);
            IsSay = true;
            IsSaying = true;
        }
        if (!LoadManager.GetInstance().IsSayEnding())
        {
            LoadManager.GetInstance().TmpSet(TextVeiwer);
            LoadManager.GetInstance().PlayTyping();
        }
        else
        {
            Player.Instance.playerInput.enabled = true;
            IsSaying = false;
            TextVeiwer.gameObject.SetActive(false);
            CutScenePlayer.gameObject.SetActive(true);
            CutScenePlayer.PlayCutScene();
        }

        return false;
    }


}
