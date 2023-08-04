using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaySpeaker : MonoBehaviour/*, IInteractable*/
{
    [SerializeField] private bool IsSaying = false;
    // 말을 하였는가? 하지 않았을 경우 !를 띄워줌.
    public GameObject QuestIcon;
    public GameObject SpeakBalloon;
    public bool IsSay = false;
    public int SayType = 1;     // 어떤 말을 뱉을 것인지 대상마다 다름.
    public TextMeshProUGUI TextViewer;      // 필수
    public CutScene CutScenePlayer;         // 있으면 재생.

    public string InteractionPrompt => throw new System.NotImplementedException();

    private void Start()
    {
        if(TextViewer != null)
        {
            Debug.LogWarning(this.gameObject.name + ": Not Have A TextViewer at SeapkerController");
        }
        SpeakBalloon =
            TextViewer.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        SpeakBalloon.SetActive(false);
    }

    private void Update()
    {
        
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator AnimEvents()
    {
        Animator anim = QuestIcon.GetComponent<Animator>();
        if(anim != null)
            QuestIcon.GetComponent<Animator>().SetTrigger("Interactable");
        yield return new WaitForSeconds(1.0f);
        IsSaying = true;
        SpeakBalloon.SetActive(true);
        PlaySay();
    }

    public void PlaySay()
    {
        if (IsSay && !IsSaying)
        {
            return;
        }
        else if (!IsSaying)
        {
            Player.Instance.playerInput.enabled = false;
            LoadManager.GetInstance().SearchTypePoint(SayType);
            IsSay = true;
            // 타이머 시작.
            StartCoroutine(AnimEvents());
            // 타이머 끝난 후 Tmp 출력.
        }
        else if(IsSaying)
        {
            if (!LoadManager.GetInstance().IsSayEnding())
            {
                LoadManager.GetInstance().TmpSet(TextViewer);
                LoadManager.GetInstance().PlayTyping();
            }
            else
            {
                Player.Instance.playerInput.enabled = true;
                IsSaying = false;
                SpeakBalloon.SetActive(false);
                if (CutScenePlayer != null)
                {
                    CutScenePlayer.gameObject.SetActive(true);
                    CutScenePlayer.PlayCutScene();
                }
            }
        }
        else
        {
            Debug.Log("Waiting");
        }
    }

    public bool Interact(Interactor interactor)
    {


        return false;
    }


}
