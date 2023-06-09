using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    public GameObject CutSceneBackGround;
    public PlayableDirector[] cutScenes;

    public bool IsCutScene;

    private int sceneCount;

    private void Awake()
    {
        cutScenes = CutSceneBackGround.GetComponentsInChildren<PlayableDirector>();
        HideCutScenes();
        IsCutScene = true;
        sceneCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if((IsCutScene
            && Player.Instance.playerInput.actions["jump"].triggered) 
            || cutScenes[sceneCount - 1].GetComponent<PlayableDirector>().state == PlayState.Paused)
        {
            if(sceneCount >= cutScenes.Length)
                HideCutScenes();
            else
                cutScenes[sceneCount++].gameObject.SetActive(true);
        }

    }

    private void HideCutScenes()
    {
        for (int i = 0; i < cutScenes.Length; i++)
        {
            cutScenes[i].gameObject.SetActive(false);
        }
        IsCutScene = false;
    }
}
