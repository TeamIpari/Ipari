using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>

public class lendslide : MonoBehaviour, IEnviroment
{
    public BrokenDoorManager Door;
    public float BrokenTime = 2.0f;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        IsHit = true;
        StartCoroutine(Boom());
        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !IsHit)
        {
            IsHit = true;
            StartCoroutine(Boom());
        }
    }


    private IEnumerator Boom ()
    {
        yield return new WaitForSeconds(BrokenTime);
        Door.Boom();

    }
}
