using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 *   모래 이펙트가 점점 퍼지는 기능이 구현된 컴포넌트입니다...
 * ***/
[RequireComponent(typeof(SphereCollider))]
public class SandWave : MonoBehaviour
{
    //===============================================
    //////        Property and Fields           /////
    //===============================================
    [SerializeField] public AnimationCurve waveCurve;
    [SerializeField] public GameObject SandFX;
    [SerializeField] public float WaveDuration;
    [SerializeField] public float WaveMaxRadius;
    [SerializeField] public int   Pricision = 10;


    private GameObject[]   _FXLists;
    private SphereCollider _collider;
    private float          _radianDiv = 0f;

    private float           _timeLeft = -1f;
    private float           _shakeTimeLeft = 0f;
    private float           _shakeTime = .5f;
    private float           _shakeMaxTime = .8f;
    private float           _timeDiv = 1f;
    private float           _delayTime = 0f;


    //========================================
    /////          Magic methods          ////
    //========================================
    private void Awake()
    {
        #region Omit
        if (SandFX != null)
        {
            _FXLists = new GameObject[Pricision];
            for(int i=0; i< Pricision; i++){

                _FXLists[i] = Instantiate(SandFX);
                _FXLists[i].transform.localScale = _FXLists[i].transform.localScale * .3f;
                _FXLists[i].SetActive(false);
            }

            _radianDiv = (Mathf.PI * 2f) / Pricision;
        }
        #endregion
    }

    private void Update()
    {
        if (_timeLeft < 0f || _FXLists==null) return;

        if(_delayTime>0f)
        {
            _delayTime -= Time.deltaTime;
            return;
        }

        _timeLeft -= Time.deltaTime;

        Vector3 center = transform.position;
        float progressRatio = Mathf.Clamp(1f - (_timeLeft * _timeDiv), 0f, 1f);
        float currRadian = 0f;
        float currRadius = WaveMaxRadius * waveCurve.Evaluate(progressRatio);

        if((_shakeTime-=Time.deltaTime)<=0f)
        {
            CameraManager.GetInstance().CameraShake(3f, .5f);
            _shakeTime = Mathf.Clamp(_shakeTime -= 1f, 0f, _shakeMaxTime);
        }

        for (int i = 0; i < Pricision; i++)
        {
            Vector3 dir = new Vector3(

                Mathf.Cos(currRadian),
                0f,
                Mathf.Sin(currRadian)

            );
            Vector3 newPos = center + (dir * currRadius);

            RaycastHit hit;
            if (!Physics.Raycast(

                newPos,
                Vector3.down,
                out hit,
                10f,
                 1 << LayerMask.NameToLayer("Platform")))
            {
            }

            newPos.y -= hit.distance;
            _FXLists[i].transform.position = newPos;

            currRadian += _radianDiv;
        }

        /**마무리 되었을 경우...*/
        if (progressRatio >= 1f)
        {
            for (int i = 0; i < Pricision; i++){

                _FXLists[i].SetActive(false);
            }
        }
    }

    public void StartWave()
    {
        if(_FXLists!=null)
        {
            for (int i = 0; i < Pricision; i++){

                _FXLists[i].transform.position = transform.position;
                _FXLists[i].SetActive(true);
            }

            _radianDiv = (Mathf.PI * 2f) / Pricision;
            _timeLeft = WaveDuration;
            _timeDiv  = 1f/WaveDuration;

            _shakeTime = 0f;
            _shakeTime = _shakeMaxTime;
            _delayTime = .1f;
        }
    }


}
