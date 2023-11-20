using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IPariUtility;
using UnityEngine.UI;

/*********************************************************
 *   UI���� �ν����� ���� �Լ����� �����ϴ� ������Ʈ�Դϴ�....
 * ****/
public sealed class UIInspectorHelper : MonoBehaviour
{
    #region Define
    [System.Serializable]
    public struct FadeDesc
    {
        public IpariUtility.FadeOutType FadeType;
        public float      FadeDuration;
        public float      FadeSecondDelay;
        public float      FadeStartDelay;
        public UnityEvent OnEndOfFirstFade;
    }
    #endregion

    //=====================================================
    //////          Property and Fields              //////
    //=====================================================
    [SerializeField] public UnityEngine.UI.Slider TargetSlider;
    [SerializeField] public UnityEngine.UI.Image  TargetImage;
    [SerializeField] public Transform             TargetTransform;
    [SerializeField] public Color[]     ColorSamples;
    [SerializeField] public Vector3[]   VectorSamples;
    [SerializeField] public FadeDesc[]  FadeSamples;


    private Image _screenImage;


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

    public void ApplyScreenFade(int sampleIndex)
    {
        #region Omit
        if (FadeSamples == null || sampleIndex < 0 || sampleIndex >= FadeSamples.Length)
            return;

        /**���̵带 ������ Image�� �����´�....*/
        if(_screenImage==null){

            if(TargetImage!=null) _screenImage = TargetImage;
            _screenImage = UIManager.GetInstance().transform.Find("FadeCanvas").GetChild(0).GetComponent<Image>();
        }

        /*******************************************
         *   ���̵带 �����Ѵ�.....
         * ****/
        ref FadeDesc desc = ref FadeSamples[sampleIndex];

        IpariUtility.ApplyImageFade(

            desc.FadeType,
            _screenImage,
            desc.FadeDuration,
            desc.FadeSecondDelay,
            -99999,
            default,
            0f,
            1f,
            desc.FadeStartDelay,
            default,
            desc.OnEndOfFirstFade
        );

        #endregion
    }




}
