using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************************************************
 *   UI관련 인스펙터 헬퍼 함수들을 제공하는 컴포넌트입니다....
 * ****/
public sealed class UIInspectorHelper : MonoBehaviour
{
    //=====================================================
    //////          Property and Fields              //////
    //=====================================================
    [SerializeField] public UnityEngine.UI.Slider TargetSlider;
    [SerializeField] public UnityEngine.UI.Image  TargetImage;
    [SerializeField] public Transform             TargetTransform;
    [SerializeField] public Color[]     ColorSamples;
    [SerializeField] public Vector3[]   VectorSamples;



    //=======================================================
    //////          Magic and Public methods           //////
    //=======================================================
    private void Awake()
    {
        #region Omit
        TargetSlider = GetComponent<UnityEngine.UI.Slider>();
        #endregion
    }

    public void SetTargetImage(UnityEngine.UI.Image targetImg)
    {
        TargetImage = targetImg;
    }

    public void SetTargetSlider(UnityEngine.UI.Slider targetSlider)
    {
        TargetSlider = targetSlider;
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        TargetTransform = targetTransform;
    }

    public void SetImageColor(int sampleIndex)
    {
        #region
        if (ColorSamples == null || TargetImage == null || sampleIndex < 0 || sampleIndex >= ColorSamples.Length)
            return;

        TargetImage.color = ColorSamples[sampleIndex];
        #endregion
    }

    public void SetTransformScale(int sampleIndex)
    {
        #region
        if (VectorSamples == null || TargetTransform == null || sampleIndex < 0 || sampleIndex >= VectorSamples.Length)
            return;

        TargetTransform.localScale = VectorSamples[sampleIndex];
        #endregion
    }




}
