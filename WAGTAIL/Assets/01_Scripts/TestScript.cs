using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    [SerializeField] FModParameterReference paramRef;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name}이 {collision.gameObject.name}과 충돌!!!");
    }

}
