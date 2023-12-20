using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossroomBackground : MonoBehaviour
{
    Material mat;

    [SerializeField] float timeScale = 1f;
    [SerializeField] float currTime  = 0f;
    [SerializeField] float maxTime   = 0f;

    float timeDiv = 0f;

    void Start()
    {
        mat     = GetComponent<MeshRenderer>().sharedMaterial;
        timeDiv = (1f / maxTime);
    }

    void Update()
    {
        currTime = Mathf.Clamp01((currTime += (Time.deltaTime * timeScale))* timeDiv);
        mat.SetFloat("_NormalizedTime", currTime);

        if(currTime>=1f){

            currTime = 0f;
        }
    }
}
