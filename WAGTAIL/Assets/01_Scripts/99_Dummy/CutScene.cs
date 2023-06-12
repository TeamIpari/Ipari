using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    public GameObject CutSceneBackGround;
    public PlayableDirector[] cutScenes;
    public Transform[] cuts;

    public bool IsCutScene;

    public int sceneCount;

    private void Awake()
    {
        Debug.Log(CutSceneBackGround.GetComponentsInChildren<PlayableDirector>().Length);
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
        if(Input.GetKey(KeyCode.F) && (sceneCount > 0 && sceneCount < cutScenes.Length) && cutScenes[sceneCount - 1].state == PlayState.Playing)
        {
            cutScenes[sceneCount - 1].playableGraph.GetRootPlayable(0).SetSpeed(1f);
        }
        if((IsCutScene
            && Input.GetKeyDown(KeyCode.F))
            || (sceneCount > 0 && cutScenes[sceneCount - 1].state == PlayState.Paused))
        {
            if (sceneCount >= cutScenes.Length && cutScenes[sceneCount - 1].state == PlayState.Paused)
            {
                Player.Instance.playerInput.enabled = true;
                HideCutScenes();
                CutSceneBackGround.gameObject.SetActive(false);
            }
            else if (sceneCount < cutScenes.Length)
            {
                Player.Instance.playerInput.enabled = false;
                cutScenes[sceneCount++].gameObject.SetActive(true);
            }
        }
    }

    public void PlayCutScene()
    {
        cutScenes[sceneCount++].gameObject.SetActive(true);
        Player.Instance.playerInput.enabled = false;
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
