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
                Debug.LogWarning("Boss Nepenthes�� Ž������ �ʾҽ��ϴ�.");
        }
        catch
        {
            Debug.LogWarning("Boss Nepenthes�� Ž������ �ʾҽ��ϴ�.");
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
