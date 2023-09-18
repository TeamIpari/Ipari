using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaySpeaker : MonoBehaviour/*, IInteractable*/
{
    [SerializeField] private bool IsSaying = false;
    // ���� �Ͽ��°�? ���� �ʾ��� ��� !�� �����.
    public GameObject QuestIcon;
    public GameObject SpeakBalloon;
    public bool IsSay = false;
    public bool isUsing = false;
    public int SayType = 1;     // � ���� ���� ������ ��󸶴� �ٸ�.
    public TextMeshProUGUI TextViewer;      // �ʼ�
    public CutScene CutScenePlayer;         // ������ ���.
    public Dialogue Dialogue;


    private void Start()
    {
        if(TextViewer != null)
        {
            Debug.LogWarning(this.gameObject.name + ": Not Have A TextViewer at SeapkerController");
        }
        SpeakBalloon =
            TextViewer.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        SpeakBalloon.SetActive(false);
        isUsing = false;
    }

    //private void Update()
    //{
        
    //}

    public void AnimEvent()
    {
        if (!IsSaying && !isUsing)
            StartCoroutine(AnimEvents());
        else
            PlaySay();
    }

    IEnumerator AnimEvents()
    {
        Player.Instance.playerInput.enabled = false;
        isUsing = true;
        Animator anim = QuestIcon.GetComponent<Animator>();
        if(anim != null)
            QuestIcon.GetComponent<Animator>().SetTrigger("Interactable");
        yield return new WaitForSeconds(1.0f);
        IsSaying = true;
        Dialogue = LoadManager.GetInstance().IO_GetScriptable(SayType);
        IsSay = true;
        SpeakBalloon.SetActive(true);
        LoadManager.GetInstance().TmpSet(TextViewer);
        LoadManager.GetInstance().StartDialogue(Dialogue);
    }

    public void PlaySay()
    {
        if (IsSay && !IsSaying)
        {
            return;
        }
        else if(IsSaying)
        {
            if (!LoadManager.GetInstance().EndDialogue())
            {
                LoadManager.GetInstance().DisplayNextSentence();
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
