using DG.Tweening;
using IPariUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name}�� {collision.gameObject.name}�� �浹!!!");
    }

}
