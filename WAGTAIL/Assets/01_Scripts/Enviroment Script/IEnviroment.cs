using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ� 
/// </summary>
public interface IEnviroment 
{
    public string EnviromentPrompt { get; }
    public bool IsHit { get; set; }

    public bool Interact();
}
