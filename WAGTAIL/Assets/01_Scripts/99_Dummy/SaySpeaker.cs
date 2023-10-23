using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**************************************************
*   상호작용 할 경우, 대화를 실행하는 컴포넌트입니다...
*******/
public sealed class SaySpeaker : MonoBehaviour, IInteractable
{
    //=================================================
    //////              Property                 //////
    //================================================
    public Vector3  InteractPopupOffset { get { return (!_IsTalkable ? _unPossibleOffset : _possibleOffset); } set{ Debug.LogWarning("SaySpeaker: 임의로 상호작용UI 오프셋을 수정할 수 없습니다."); } }
    public string   InteractionPrompt   { get; set; } = "대화한다";
    public bool     IsSaying            { get { return _IsSaying; } }
    public bool     IsTalkable          
    { 
        get { return _IsTalkable; } 
        set
        {
            Animator qiAnim;
            if ((_IsTalkable = value) && QuestIcon && (qiAnim = QuestIcon.GetComponent<Animator>())){

                qiAnim.SetTrigger("Interactable");
            }
        } 
    }     

    [SerializeField] private bool           _IsSaying   = false;
    [SerializeField] private bool           _IsTalkable  = true;
    [SerializeField] public  int            SayType     = 1;

    [SerializeField] public TextMeshProUGUI TextViewer;
    [SerializeField] public TextMeshProUGUI NameTag;
    [SerializeField] public GameObject      TextBoxPrefab;

    [SerializeField] public GameObject      QuestIcon;
    [SerializeField] public GameObject      SpeakBalloon;

    [SerializeField] public CutScene        CutScenePlayer;
    [SerializeField] public Dialogue        Dialogue;



    //===================================================
    ///////               Fields                   //////
    //===================================================
    private static readonly Vector3 _possibleOffset    = (Vector3.up*1.5f);
    private static readonly Vector3 _unPossibleOffset  = (Vector3.up * 9999f);



    //======================================================
    //////               Magic methods                //////
    //======================================================
    private void Start()
    {
        #region Omit

        /**대화창이 없다면, UIManager의 전역 대화창을 택한다....*/
        if (TextViewer == null)
        {
            TextBoxPrefab = UIManager.GetInstance().GetGameUI(GameUIType.TextBox).gameObject;
            TextViewer    = TextBoxPrefab.GetComponent<TextBoxUI>().textBox;
            NameTag       = TextBoxPrefab.GetComponent<TextBoxUI>().nameTag;
            Debug.LogWarning(gameObject.name + ": Not Have A TextViewer at SeapkerController");
        }

        #endregion
    }



    //======================================================
    ///////             Core methods                ////////
    //======================================================
    private IEnumerator TalkProgress()
    {
        #region Omit
        /*****************************************************
         *   플레이어의 상호작용 키와, 퀘스트 아이콘에 대한 처리...
         * ****/
        InputAction interact = Player.Instance.playerInput.actions["Interaction"];
        Player      player   = Player.Instance;
        Animator    qiAnim   = null;
        Animator    boxAnim  = null;

        /**퀘스트 아이콘을 종료시킨다....*/
        if (QuestIcon!=null && (qiAnim=QuestIcon.GetComponent<Animator>())){

            qiAnim.SetTrigger("Interactable");
        }

        /**텍스트 박스가 나타나는 애니메이션을 재생한다...*/
        if(TextBoxPrefab!=null && (boxAnim=TextBoxPrefab.GetComponent<Animator>())){

            TextBoxPrefab.SetActive(true);
            boxAnim.Play("TextBox_FadeIn");
        }



        /**************************************************
         *   대화내역 및 대화를 출력할 텍스트를 갱신한다....
         * ****/
        try{

            Dialogue = LoadManager.GetInstance().IO_GetScriptable(SayType);
            LoadManager.GetInstance().NameTagSet(NameTag);
            LoadManager.GetInstance().TmpSet(TextViewer);
            LoadManager.GetInstance().StartDialogue(Dialogue);
        }
        catch 
        {
            if (boxAnim != null)
            {
                boxAnim.Play("TextBox_FadeOut");
            }
            player.movementSM.ChangeState(player.idle);
            player.currentInteractable = null;
            Debug.LogWarning("SaySpeaker -> LoadManager: 출력할 대화내역이 존재하지않습니다..");
            yield break;
        }

        ///**컷씬이 존재한다면 활성화시킨다....*/
        //if (CutScenePlayer != null){

        //    CutScenePlayer.gameObject.SetActive(true);
        //}



        /**********************************************************
         *   대화내역이 존재하지 않을 때까지 출력한다....
         * ****/
        bool isPressed = true;
        while (true)
        {
            /**상호작용키를 눌렀다가 뗏을 때...*/
            if (!interact.triggered || isPressed){
                if (!interact.triggered && isPressed) isPressed = false;
                yield return null;
                continue;
            }
            /**더 이상 진행할 수 있는 대화내역이 존재하지 않을경우 대화를 마친다...*/
            if (LoadManager.GetInstance().EndDialogue()) break;


            isPressed = true;


            /**다음 대화내역을 출력한다...*/
            LoadManager.GetInstance().DisplayNextSentence();
        }

        /*******************************************
         *    대화창을 닫고 마무리 짓는다....
         * ****/
        Debug.Log($"{boxAnim.name}");
        if (boxAnim!=null){

            TextBoxPrefab.SetActive(false);
            boxAnim.Play("TextBox_FadeOut");
        }

        while (CutScenePlayer)
        {
            /**컷씬이 존재할 경우, 다음 컷씬으로 이동한다..*/
            if (CutScenePlayer != null)
            {
                CutScenePlayer.gameObject.SetActive(true);
                CutScenePlayer.PlayCutScene();
            }
            if (CutScenePlayer.GetisCutScene)
                break;
        }


        player.movementSM.ChangeState(player.idle);
        player.currentInteractable = null;
        #endregion
    }

    public void PlaySay()
    {
        #region Omit
        if (!_IsTalkable) return;

        /*****************************************
         *   대화가 가능하다면 대화를 시작한다.....
         * ***/
        _IsSaying = true;
        _IsTalkable  = false;

        /**플레이어의 조작을 제한한다....*/
        Player player = Player.Instance;
        player.stiffen.StiffenTime = -1;
        player.movementSM.ChangeState(player.stiffen);

        StopAllCoroutines();
        StartCoroutine(TalkProgress());
        #endregion
    }

    public bool Interact(GameObject interactor)
    {
        #region Omit
        /********************************************************************
         *   플레이어가 아니거나, 대화가 불가능한 상태라면 상호작용은 불가능하다...
         * ***/
        if (!interactor.CompareTag("Player")) return false;

        PlaySay();
        return IsTalkable;
        #endregion
    }

    public bool AnimEvent()
    {
        return false;
    }
}
