using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성 
/// </summary>
public interface IEnviroment 
{
    public string EnviromentPrompt { get; }
    public bool IsHit { get; set; }

    public bool Interact();
}
