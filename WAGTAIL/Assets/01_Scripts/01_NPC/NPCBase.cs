 using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;


public class NPCBase : MonoBehaviour
{
    [Header("Public Data")]
    public string Name;     // NPC ��
    public int Hp;          // NPC�� ü��
    public int MoveSpeed;   // NPC�� �̵� �ӵ�.
    //public int 

    public bool IsDead = false;     // ���� ����.

    public string stateName = "None";


    /// <summary>
    /// ��� NPC���� Idle ���¸� ������ ������ �ʴ��� �⺻������ Idle�� ������ ������ ����.
    /// </summary>
    [Header("Idle")]
    public float NextStateTimer;
    public float SearchDistance;


    //public int 

}
