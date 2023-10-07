using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    private bool           isTrigger = false;
    private PullableObject pullable;
    private PullInOutState pullInOutState;

    private void Start()
    {
        pullable       = GetComponent<PullableObject>();
        pullInOutState = new PullInOutState(Player.Instance, Player.Instance.movementSM, Player.Instance.transform.Find("HoldingPoint").gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isTrigger = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && isTrigger)
        {
            Player.Instance.movementSM.ChangeState(pullInOutState);
            pullInOutState.HoldTarget(gameObject);
            isTrigger = false;
        }
    }
}
