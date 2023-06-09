using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundKeyType
{
    Boolean = 1,
    Trigger
}

public class SoundHandler : MonoBehaviour
{
    Dictionary<string, (SoundKeyType, bool)> _stateDict = new Dictionary<string, (SoundKeyType, bool)>();
    Dictionary<string, AudioClip> _clipDict = new Dictionary<string, AudioClip>();

    void Start()
    {
        // 1. Register on Manager (�Ŵ����� �ڽ��� ����ϱ�)
    }

    public void RegisterBool(string keyName) => RegisterOnState(keyName, SoundKeyType.Boolean);
    public void RegisterTrigger(string keyName) => RegisterOnState(keyName, SoundKeyType.Trigger);
    private void RegisterOnState(string keyName, SoundKeyType type)
    {
        if (!_stateDict.TryGetValue(keyName, out var value))
        {
            _stateDict.Add(keyName, (SoundKeyType.Boolean, false));
        }
        else
        {
            // If stateDict already have key that same name with keyName
            // ���� stateDict �� keyName �� ���� Ű�� �̹� ���� ���� ���
            Debug.Log("SoundHandler :  Already has Key Error. : " + keyName);
        }
    }

    public void BindAudioClip(string keyName, AudioClip clip)
    {
        if (_stateDict.TryGetValue(keyName, out var value))
        {
            _clipDict.Add(keyName, clip);
        }
        else
        {
            // If stateDict doesn't has a keyName
            // ���� stateDict�� keyName �� ���� ���� ���� ���
            Debug.LogError("SoundHandler : No Key Error. Can't find a key : " + keyName);
        }
    }

    void Update()
    {
        foreach (var _clip in _clipDict)
        {
            SoundKeyType type = _stateDict[_clip.Key].Item1;
            bool state = _stateDict[_clip.Key].Item2;

            if (state == true)
            {

            }
        }
    }
}
