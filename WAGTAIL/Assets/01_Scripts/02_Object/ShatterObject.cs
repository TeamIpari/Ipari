using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterObject : MonoBehaviour
{
    [SerializeField] private GameObject _originalObject;
    [SerializeField] private GameObject _fracturedObject;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private float _explosionMinForce = 5;
    [SerializeField] private float _explosionMaxForce = 100;
    [SerializeField] private float _explosionForceRadius = 10;
    [SerializeField] private float _fragScaleFactor = 0.01f;

    
    
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
                //_fractObj.transform.localScale = _originalObject.transform.lossyScale;
                foreach (Transform t in _fractObj.transform)
                {
                    var rb = t.GetComponent<Rigidbody>();

                    if(rb != null )
                    {
                        rb.AddExplosionForce(Random.Range(_explosionMinForce, _explosionMaxForce), 
                            _originalObject.transform.position, _explosionForceRadius);

                        StartCoroutine(Shrink(t, 2));
                    }

                    Destroy(_fractObj, 3);

                    if (_explosionVFX != null )         
                    {           
                        GameObject exploVFX = Instantiate(_explosionVFX, _originalObject.transform.position, transform.rotation);
                        Destroy(exploVFX, 7);
                    }
                }
            }
        }
    }
    
    // Debug용 함수
    private void Reset()
    {
        Destroy(_fractObj);
        _originalObject.SetActive(true);
    }

    IEnumerator Shrink (Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newScale = t.localScale;

        while(newScale.x >= 0)
        {
            newScale -= new Vector3(_fragScaleFactor, _fragScaleFactor, _fragScaleFactor);

            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }
    }
}