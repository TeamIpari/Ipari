using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AnimManager : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void EndLayer()
    {
        StartCoroutine(WeightLerp());
    }

    public void SetLayer()
    {
        _animator.SetLayerWeight(1, 0f);
    }

    private IEnumerator WeightLerp()
    {
        float time = 0f;
        float duration = 0.15f;
        float start = 1f;
        float end = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            _animator.SetLayerWeight(1, Mathf.Lerp(start, end, t));
            yield return null;
        }
    }
    
    
    /*
    private static AnimManager instance;
    public static AnimManager Instance { get { return instance; } }

    public Animator Anim;
    // Start is called before the first frame update
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        Anim = GetComponent<Animator>();
    }


    //// Update is called once per frame
    //private void Update()
    //{

    //}

    private void OnAnimatorIK(int layerIndex)
    {
    }

    private void AnimFunc()
    {
        try
        {
            //Player.Instance.currentInteractable.GetComponent<SThrow>().Throwing();
            Player.Instance.currentInteractable.GetComponent<IInteractable>().AnimEvent();
        }
        catch
        {
        }
    }*/
    
}
