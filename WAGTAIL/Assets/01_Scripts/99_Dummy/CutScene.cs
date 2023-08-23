using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class CutScene : MonoBehaviour
{
    public GameObject CutSceneBackGround;
    public PlayableDirector[] cutScenes;
    public TextMeshProUGUI TextViewer;
    public Transform[] cuts;

    public bool IsIntro;
    public int SayType;
    public bool ISText;
    public bool isSpeedUp = false;

    [Header("")]
    public float LastTimeLineTime = 5f;
    public float WaitTime = 0;

    private float colorValue = 255;

    private bool IsCutScene;
    private int sceneCount;

    private void Awake()
    {
        cutScenes = CutSceneBackGround.GetComponentsInChildren<PlayableDirector>();
        HideCutScenes();
        IsCutScene = true;
        sceneCount = 0;
        colorValue = 255;
        colorCurve = -51f;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsIntro)
        {
            PlayCutScene();
            LoadManager.GetInstance().IO_GetScriptable(SayType);
            LoadManager.GetInstance().TmpSet(TextViewer);
            LoadManager.GetInstance().PlayTyping();
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

    private void SceneChange()
    {
        bool bInputNextKey = Input.GetKeyDown(KeyCode.F);
        bool bSceneState = sceneCount > 0 && cutScenes[sceneCount - 1].state == PlayState.Paused;

        if(bInputNextKey || bSceneState)
        {
            if (sceneCount >= cutScenes.Length && cutScenes[sceneCount - 1].state == PlayState.Paused)
            {
                Player.Instance.playerInput.enabled = true;
                HideCutScenes();
                UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
                SoundTest.GetInstance().PlayBGM("isTitle", false);
                //CameraManager.GetInstance().SwitchCamera(CameraType.Village);
                SoundTest.GetInstance().PlayBGM("isInGame", true);
            }
            else if (sceneCount < cutScenes.Length)
            {
                Player.Instance.playerInput.enabled = false;
                cutScenes[sceneCount++].gameObject.SetActive(true);
                if (TextViewer != null)
                    LoadManager.GetInstance().PlayTyping();
            }
        }

        // 글자를 점점 사라지게 하는 기능.
        if (sceneCount >= cutScenes.Length)
        {
            if (WaitTime > WaitTimer)
                WaitTimer  += Time.deltaTime;
            else
            {
                colorValue -= Time.deltaTime * (255f / LastTimeLineTime);
                TextViewer.color = new Color32((byte)(colorValue), (byte)(colorValue), (byte)(colorValue), (byte)colorValue);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsCutScene)
        {
            DoubleSpeed();
            SceneChange();
        }
    }

    public void PlayCutScene()
    {
        IsCutScene = true;
        CutSceneBackGround.gameObject.SetActive(true);
        cutScenes[sceneCount++].gameObject.SetActive(true);
        Player.Instance.playerInput.enabled = false;
        if (TextViewer != null)
            TextViewer.gameObject.SetActive(true);
    }

    private void HideCutScenes()
    {
        for (int i = 0; i < cutScenes.Length; i++)
        {
            cutScenes[i].gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        IsCutScene = false;
        sceneCount = 0;
        CutSceneBackGround.gameObject.SetActive(false);
        if (TextViewer != null)
            TextViewer.gameObject.SetActive(false);
    }
}
