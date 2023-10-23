using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
/***********************************************************
 *   Animation 관련 유틸리티 기능들이 포함된 헬퍼 컴포넌트입니다.
 * ***/
public sealed class AnimatorHelper : MonoBehaviour
{
    public delegate void LateUpdateNotify();

    //======================================
    /////           Propety             ////
    //======================================
    public Animator HelperAnimator { get; private set; }

    public LateUpdateNotify AnimatorLateUpdate;



    //========================================
    /////         Magic methods           ////
    //========================================
    private void Start()
    {
        HelperAnimator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        AnimatorLateUpdate?.Invoke();
    }



    //==========================================
    /////        Public methods           /////
    //==========================================
    public void EndLayer()
    {
        StartCoroutine(WeightLerp());
    }

    public void SetLayer(int index, float weight)
    {
        HelperAnimator.SetLayerWeight(index, weight);
    }



    //==========================================
    /////          Core methods             ////
    //==========================================
    private IEnumerator WeightLerp()
    {
        float time       = 0f;
        float duration   = 0.15f;
        float start      = 1f;
        float end        = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            HelperAnimator.SetLayerWeight(1, Mathf.Lerp(start, end, t));
            yield return null;
        }
    }


}
