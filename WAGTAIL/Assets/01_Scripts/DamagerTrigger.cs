using MagicaCloth2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.iOS;

/// <summary>
/// 작성자 : 성지훈
/// 추가 작성
/// </summary>
public class DamagerTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ;
        }
    }
}
