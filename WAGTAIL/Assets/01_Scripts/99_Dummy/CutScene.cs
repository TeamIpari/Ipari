using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.InputSystem;

public class CutScene : MonoBehaviour
{
    public GameObject CutSceneBackGround;
    public PlayableDirector[] CutScenes;
    public TextMeshProUGUI TextViewer;
    public Transform[] Cuts;

    public bool IsIntro;
    public int SayType;
    public bool isText;
    public bool isSpeedUp = false;
    public bool isSkip;

    [Header("Timer")]
    public float LastTimeLineTime = 5f;
    public float WaitTime = 0;
    public float FKeyWaitTime = 0;
    private float colorValue = 255;

    private bool isCutScene;
    public bool GetisCutScene { get { return isCutScene; } }
    private int sceneCount;
    private float FKeyWaitTimer = 0;
   
    [SerializeField] private Dialogue dialogue = new Dialogue();

    public GameObject ChapterCanvas;

    private void Awake()
    {
        //CutScenes = CutSceneBackGround.GetComponentsInChildren<PlayableDirector>();
        HideCutScenes();
        isCutScene = true;
        sceneCount = 0;
        colorValue = 255;
        colorCurve = -51f;
        FKeyWaitTimer = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsIntro)
        {
            PlayCutScene();
            dialogue = LoadManager.GetInstance().IO_GetScriptable(SayType);
            LoadManager.GetInstance().TmpSet(TextViewer);
            LoadManager.GetInstance().StartDialogue(dialogue);
            FModAudioManager.SetBusMute(FModBusType.Environment, true);
            ChapterCanvas.SetActive(false);
            //LoadManager.GetInstance().PlayTyping();
        }
    }

    private void DoubleSpeed()
    {
        Time.timeScale = Input.GetKey("space") ? 3.0f : 1f;
        LoadManager.GetInstance().isSpeedUp = Input.GetKey("space") ? true : false;
    }

    private float colorTimer = 0f;
    private float colorCurve = -51f;
    private float WaitTimer = 0;

    private bool FKeyCooldown()
    {
        if (FKeyWaitTime > FKeyWaitTimer)
            return false;
        else
            return true;
    }

    private void SceneChange()
    {
        bool bInputNextKey = (Input.GetKeyDown(KeyCode.F) || (Gamepad.current!=null && Gamepad.current.buttonWest.value!=0)) && isSkip && FKeyCooldown();
        bool bSceneState   = sceneCount > 0 && CutScenes[sceneCount - 1].state == PlayState.Paused;
        if(bInputNextKey || bSceneState)
        {
            FKeyWaitTimer = 0;
            if (sceneCount >= CutScenes.Length && CutScenes[sceneCount - 1].state == PlayState.Paused)
            {
                Player.Instance.playerInput.enabled = true;
                HideCutScenes();
                if (IsIntro)
                {
                    try
                    {
                        UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).gameObject.GetComponent<CollectionCocosiUI>()
                            .SetCanvas(0, true);
                        FModAudioManager.PlayBGM(FModBGMEventType.tavuti_ingame1);
                        ChapterCanvas.SetActive(true);
                        ChapterCanvas.GetComponent<ChapterUI>().ChapterCanvasStart();
                    }
                    catch
                    {

                    }
                    this.gameObject.SetActive(false);
                }
            }
            else if (sceneCount < CutScenes.Length)
            {
                Player.Instance.playerInput.enabled = false;
                CutScenes[sceneCount - 1].gameObject.SetActive(false);
                CutScenes[sceneCount++].gameObject.SetActive(true);
                if (TextViewer != null)
                    LoadManager.GetInstance().DisplayNextSentence();
            }

        }

        //if (sceneCount >= CutScenes.Length && TextViewer != null)
        //{
        //    if (WaitTime > WaitTimer)
        //        WaitTimer  += Time.deltaTime;
        //    else
        //    {
        //        colorValue -= Time.deltaTime * (255f / LastTimeLineTime);
        //        TextViewer.color = new Color32((byte)(colorValue), (byte)(colorValue), (byte)(colorValue), (byte)colorValue);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(isCutScene)
        {
            FKeyWaitTimer += Time.deltaTime;
            DoubleSpeed();
            SceneChange();
        }
    }

    public void PlayCutScene()
    {
        isCutScene = true;
        CutSceneBackGround.gameObject.SetActive(true);
        CutScenes[sceneCount++].gameObject.SetActive(true);
        Player.Instance.playerInput.enabled = false;
        if (TextViewer != null)
            TextViewer.gameObject.SetActive(true);
    }

    private void HideCutScenes()
    {
        Debug.Log($"{CutScenes.Length}");
        for (int i = 0; i < CutScenes.Length; i++)
        {
            CutScenes[i].gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        isCutScene = false;
        sceneCount = 0;
        if(!IsIntro)
            CutSceneBackGround.gameObject.SetActive(false);
        if (TextViewer != null)
            TextViewer.gameObject.SetActive(false);
    }
}
