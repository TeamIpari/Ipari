 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour
{
    [Header("Public Data")]
    public string Name;     // NPC 명
    public int Hp;          // NPC의 체력
    public int MoveSpeed;   // NPC의 이동 속도.
    //public int 

    public bool IsDead = false;     // 생존 여부.
    


    /// <summary>
    /// 모든 NPC들은 Idle 상태를 가지며 가지지 않더라도 기본적으로 Idle의 정보를 가지고 있음.
    /// </summary>
    [Header("Idle")]
    public float NextStateTimer;
    public float SearchDistance;


    //public int 

}
