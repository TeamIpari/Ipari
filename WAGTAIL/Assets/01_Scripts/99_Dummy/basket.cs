using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class basket : MonoBehaviour
{
    public int Countmax;
    private int CurCount;

    public GameObject target;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag( "interactable"))
        {
            if (CurCount < Countmax)
                CurCount++;
            else
                target.SetActive(true);

        }
    }
}
