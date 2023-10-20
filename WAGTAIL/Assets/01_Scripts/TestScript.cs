using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Player.Instance == null) return;

        Vector3 playerPos = Player.Instance.transform.position;
        Gizmos.color = new Color(1f, 0f, 0f, .2f);
        Gizmos.DrawSphere(playerPos, 1.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 10f));
    }

}
