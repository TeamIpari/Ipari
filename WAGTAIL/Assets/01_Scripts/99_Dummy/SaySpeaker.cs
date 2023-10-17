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
    public bool isSay = false;
    public bool isUsing = false;
    public bool isInteract = false;
    public int SayType = 1;     // 어떤 말을 뱉을 것인지 대상마다 다름.
    public TextMeshProUGUI TextViewer;      // 필수
    public TextMeshProUGUI NameTag;
    public GameObject TextBoxPrefab;
    public CutScene CutScenePlayer;         // 있으면 재생.
    public Dialogue Dialogue;


    private void Start()
    {
        if(TextViewer == null)
        {
            Debug.LogWarning(this.gameObject.name + ": Not Have A TextViewer at SeapkerController");
            TextBoxPrefab = UIManager.GetInstance().GetGameUI(GameUIType.TextBox).gameObject;
            TextViewer = TextBoxPrefab.GetComponent<TextBoxUI>().textBox;
            NameTag = TextBoxPrefab.GetComponent<TextBoxUI>().nameTag;
        }
        //SpeakBalloon =
        //    TextViewer.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
        //SpeakBalloon.SetActive(false);
        isUsing = false;
        isInteract = false;
    }

    private void Update()
    {
        if (isUsing && isInteract && Input.GetKeyDown(KeyCode.F)) 
        {
            AnimEvent();
        }
    }

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

        isInteract = true;
        Animator anim = QuestIcon.GetComponent<Animator>();
        if(anim != null)
            QuestIcon.GetComponent<Animator>().SetTrigger("Interactable");
        yield return new WaitForSeconds(0.0f);
        IsSaying = true;
        Dialogue = LoadManager.GetInstance().IO_GetScriptable(SayType);
        isSay = true;
        //SpeakBalloon.SetActive(true);
        LoadManager.GetInstance().NameTagSet(NameTag);
        LoadManager.GetInstance().TmpSet(TextViewer);
        LoadManager.GetInstance().StartDialogue(Dialogue);
    }

    public void PlaySay()
    {
        if (isSay && !IsSaying)
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
                Player.Instance.currentInteractable = null;
                Player.Instance.playerInput.enabled = true;
                IsSaying = false;
                TextBoxPrefab.SetActive(false);
                //SpeakBalloon.SetActive(false);
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
            TextBoxPrefab.SetActive(true);
        }
    }

    public bool Interact(Interactor interactor)
    {


        return false;
    }


}
