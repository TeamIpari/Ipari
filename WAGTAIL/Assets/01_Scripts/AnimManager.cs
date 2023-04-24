

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

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    #region AnimProperty
    //public void AnimTrigger( string _tName)
    //{
    //    anim.SetTrigger(_tName);
    //}

    //public void AnimBoolean(string _bName, bool _b = true)
    //{
    //    anim.SetBool(_bName, _b);
    //}

    //public void AnimFloat(string _fName, float _f =1.0f)
    //{
    //    anim.SetFloat(_fName, _f);
    //}

    //public void AnimInt(string _iName, int _i = 0)
    //{
    //    anim.SetInteger(_iName, _i);
    //}
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

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
