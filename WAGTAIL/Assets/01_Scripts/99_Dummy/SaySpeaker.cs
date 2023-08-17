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
    public int SayType = 1;     // � ���� ���� ������ ��󸶴� �ٸ�.
    public TextMeshProUGUI TextViewer;      // �ʼ�
    public CutScene CutScenePlayer;         // ������ ���.

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
            // Ÿ�̸� ����.
            StartCoroutine(AnimEvents());
            // Ÿ�̸� ���� �� Tmp ���.
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
