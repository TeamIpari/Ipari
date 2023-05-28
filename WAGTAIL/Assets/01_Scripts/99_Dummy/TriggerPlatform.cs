using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class TriggerPlatform : MonoBehaviour, IEnviroment
{
    public GameObject Target;

    public bool Updown = false;
    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        IsHit = true;
        if (Target != null)
        {
            Target.AddComponent<BrokenPlatform>().HideOnly(Updown);
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
