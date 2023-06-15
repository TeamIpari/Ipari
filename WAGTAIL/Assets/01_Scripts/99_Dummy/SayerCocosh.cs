using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SayerCocosh : MonoBehaviour, IInteractable
{
    [SerializeField] private bool IsSaying = false;
    // ���� �Ͽ��°�? ���� �ʾ��� ��� !�� �����.
    public bool IsSay = false;
    public int SayType = 1;     // ���ؾ��ϴ� é�͸��� �ٸ�.
    public TextMeshProUGUI TextVeiwer;      // �ʼ�
    public CutScene CutScenePlayer;         // ������ ���.

    public string InteractionPrompt => throw new System.NotImplementedException();

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }

    public bool Interact(Interactor interactor)
    {
        if(IsSay && !IsSaying)
        {
            return false;
        }
        else if(!IsSaying)
        {
            Player.Instance.playerInput.enabled = false;
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
            TextVeiwer.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false); ;
            //TextVeiwer.gameObject.SetActive(false);
            if (CutScenePlayer != null)
            {
                CutScenePlayer.gameObject.SetActive(true);
                CutScenePlayer.PlayCutScene();
            }
        }

        return false;
    }


}
