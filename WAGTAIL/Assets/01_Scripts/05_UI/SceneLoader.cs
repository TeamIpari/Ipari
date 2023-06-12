using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    [SerializeField] private CanvasGroup _canvasGruop;
    [SerializeField] private Image _loadingBar;
    private string _loadSceneName;
    
    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        _loadSceneName = sceneName;
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        _loadingBar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while(!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                _loadingBar.fillAmount = Mathf.Lerp(_loadingBar.fillAmount, op.progress, timer);
                if(_loadingBar.fillAmount > op.progress)
                {
                    timer = 0.0f;
                }
            }
            else
            {
                _loadingBar.fillAmount = Mathf.Lerp(_loadingBar.fillAmount, 1f, timer);

                if(_loadingBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == _loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;

        while(timer <=1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            _canvasGruop.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }
        UIManager.GetInstance().GetActiveCanvas().gameObject.SetActive(false);

        if(!isFadeIn)
        {
            if (_loadSceneName == "Chapter01")
            {
                GameManager.GetInstance().StartChapter(ChapterType.Chapter01);
                CameraManager.GetInstance().CameraSetting();
            }
            gameObject.SetActive(false);
        }
    }
}
