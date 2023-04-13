using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTest : MonoBehaviour
{
    SkinnedMeshRenderer _sktelecom;
    public float value = 0f;
    [Range (1f, 10f)]
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _sktelecom = GetComponent<SkinnedMeshRenderer>();
        value = 0;
        _sktelecom.SetBlendShapeWeight(0, value);
    }

    // Update is called once per frame
    void Update()
    {

        if (_sktelecom.GetBlendShapeWeight(0) < 100)
        {
            value += speed * Time.deltaTime;
            _sktelecom.SetBlendShapeWeight(0, value); 
        }
    }

}
