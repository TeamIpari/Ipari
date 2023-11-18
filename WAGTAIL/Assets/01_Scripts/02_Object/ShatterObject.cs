using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/********************************************************
 *   폭발하여 조각이 흩어지는 효과가 구현된 컴포넌트입니다...
 * ******/
public sealed class ShatterObject : MonoBehaviour
{
    //======================================================
    ////////                Property                ////////
    //======================================================
    [SerializeField] private GameObject _originalObject;
    [SerializeField] private GameObject _fracturedObject;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private float _explosionMinForce = 5;
    [SerializeField] private float _explosionMaxForce = 100;
    [SerializeField] private float _explosionForceRadius = 10;
    [SerializeField] private float _fragScaleFactor = 0.01f;



    //====================================================
    /////////                Fields                 //////
    //====================================================
    private Material   _sharedMat;
    private GameObject _fractObj;

    private void Update()
    {
        // JiHun 추가
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    Explode();
        //}
    }



    public void Explode()
    {
        Collider col = GetComponent<Collider>();
        if(col != null )
        {
            col.enabled = false;
        }
        if (_originalObject != null)
        {
            _originalObject.SetActive(false);

            if (_fracturedObject != null)
            {
                _fractObj = Instantiate(_fracturedObject, _originalObject.transform.position, transform.rotation);
                _fractObj.transform.parent = transform;

                _sharedMat = _fractObj.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial;
                _sharedMat.SetFloat("_Dissolve", 0f);

                FModAudioManager.PlayOneShotSFX(
                    FModSFXEventType.Broken,
                    FModLocalParamType.BrokenType,
                    FModParamLabel.BrokenType.Stone,
                    transform.position
                );

                foreach (Transform t in _fractObj.transform)
                {
                    var rb = t.GetComponent<Rigidbody>();

                    if(rb != null )
                    {
                        rb.AddExplosionForce(Random.Range(_explosionMinForce, _explosionMaxForce), 
                            _originalObject.transform.position, _explosionForceRadius); 
                    }

                }

                if (_explosionVFX != null)
                {
                    GameObject exploVFX = Instantiate(_explosionVFX, _originalObject.transform.position, _explosionVFX.transform.rotation);
                    Destroy(exploVFX, 7);
                }
                StartCoroutine(Shrink(2f, 2f));
            }
        }
    }
    
    // Debug용 함수
    private void Reset()
    {
        Destroy(_fractObj);
        _originalObject.SetActive(true);
    }

    IEnumerator Shrink (float delay, float time)
    {
        #region Omit
        while ((delay -= Time.deltaTime) > 0f) yield return null;

        float timeDiv = (1f / time);
        do
        {
            float progressRatio = (1f - Mathf.Clamp01((time -= Time.deltaTime)*timeDiv));
            _sharedMat.SetFloat("_Dissolve", 1.02f * progressRatio);
            yield return null;
        }
        while (time > 0f);

        Destroy(gameObject);
        #endregion
    }
}