

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class AnimManager
    : MonoBehaviour
{
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
    }
}
