using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineHit : MonoBehaviour
{
    BossNepenthes targetBoss;

    private void Start()
    {
        try
        {
            targetBoss = GameObject.Find("NewBoss").GetComponent<BossNepenthes>();
            if (targetBoss == null)
                Debug.LogWarning("Boss Nepenthes가 탐색되지 않았습니다.");
        }
        catch
        {
            Debug.LogWarning("Boss Nepenthes가 탐색되지 않았습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") &&  !DieCheck())
            other.GetComponent<Player>().isDead = true;
    }

    private bool DieCheck()
    {

        Debug.Log("Boss Die Check");
        if (targetBoss.AiSM.CurrentState == targetBoss.AiDie)
            return true;

        return false;
    }
}
