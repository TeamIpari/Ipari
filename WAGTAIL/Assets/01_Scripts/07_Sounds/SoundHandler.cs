using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundKeyType
{
    Boolean = 1,
    Trigger
}

public class SoundSetting
{
    public float volume = 1f;
    public bool forceStop = false;
}

public class SoundHandler : MonoBehaviour
{
    // Dictionarys
    Dictionary<string, (SoundKeyType, bool)> _stateDict = new Dictionary<string, (SoundKeyType, bool)>();

    Dictionary<string, List<AudioClip>> _bindDict = new Dictionary< string, List<AudioClip> >();

    Dictionary<string, AudioSource> _srcDict = new Dictionary<string, AudioSource>();

    Dictionary<string, SoundSetting> _settingDict = new Dictionary<string, SoundSetting>();

    // Variable
    float globalVolume { get => SoundManager.GetInstance().volume; }

    void Start()
    {
        // 1. Register on Manager (�Ŵ����� �ڽ��� ����ϱ�)
        SoundManager.GetInstance().RegisterHandler(this);
    }

    public void RegisterBool(string keyName) => RegisterState(keyName, SoundKeyType.Boolean);
    public void RegisterTrigger(string keyName) => RegisterState(keyName, SoundKeyType.Trigger);
    private void RegisterState(string keyName, SoundKeyType type)
    {
        // If stateDict already have key that same name with keyName
        // ���� stateDict �� keyName �� ���� Ű�� �̹� ���� ���� ���
        if (_stateDict.ContainsKey(keyName))
            Debug.Log("SoundHandler :  RegisterState() Already has Key Error. : " + keyName);
        else
            _stateDict.Add(keyName, (type, false));
    }
    public void Bind(string stateName, AudioClip clip, SoundSetting setting)
    {
        if (Bind(stateName, clip) == true)
        {
            _settingDict[stateName+"/"+clip.ToString()] = setting;
        }
    }

    public bool Bind(string stateName, AudioClip clip)
    {
        // ���� stateDict�� keyName �� ���� ���� ���� ��� ����
        if (!_stateDict.ContainsKey(stateName))
        {
            Debug.LogError("SoundHandler : Bind() No Key Error. Can't find a key : " + stateName);
            return false;
        }

        // �� �� �ִٸ� bind �Ϸ�
        else if (!_bindDict.ContainsKey(stateName))
            _bindDict.Add(stateName, new List<AudioClip>());

        // �ٵ� �̹� �� ���ǿ� ���� Ŭ���� �ִٸ� ��ŵ
        else if (_bindDict[stateName].Contains(clip))
            return false;

        // ���ε� �Ϸ�
        _bindDict[stateName].Add(clip);

        // �̸� AudioSource ���� ����ϱ�

        // bool ���� ���� Ŭ�� �ϳ��� �ҽ� �ϳ��� �ʿ��ؼ� bool �϶� �ҽ� �̸��� �������ϰ� ���ϱ�
        string srcName = stateName;
        if (_stateDict[stateName].Item1 == SoundKeyType.Boolean)
            srcName += "/" + clip.ToString();

        //Debug.Log(srcName);
        GameObject obj = new GameObject();
        AudioSource src = obj.AddComponent<AudioSource>();
        obj.name = "AudioSource_" + srcName;
        obj.transform.parent = this.transform;
        obj.transform.localPosition = new Vector3(0,0,0);
        src.clip = clip;
        _srcDict.Add(srcName, src);

        // ������ �����ϴ� ��ųʸ��� �⺻���� ���� ���
        _settingDict.Add(stateName+"/"+clip.ToString(), new SoundSetting());

        return true;
    }
    public void SetBool(string stateName, bool value)
    {
        // ���� stateDict�� stateName �� ���� ���� ���� ��� ����
        if (!_stateDict.ContainsKey(stateName))
            Debug.LogError("SoundHandler : SetBool() No State Key Error. Can't find a key : " + stateName);

        // ���� ���� �ٲ��� �ʾҴٸ� ��ŵ
        if (_stateDict[stateName].Item2 == value)
            return;

        _stateDict[stateName] = (_stateDict[stateName].Item1, value);

        // play sounds
        if (_bindDict.ContainsKey(stateName))
        {
            foreach (AudioClip clip in _bindDict[stateName])
            {
                string key = stateName + "/" + clip.ToString();
                float volume = _settingDict[key].volume * globalVolume;
                if (value == true)
                {
                    _srcDict[key].volume = volume;
                    _srcDict[key].loop = true;
                    _srcDict[key].Play();
                }
                else
                {
                    if (_settingDict[key].forceStop == false)
                    {
                        _srcDict[key].loop = false;
                    }
                    else
                        _srcDict[key].Stop();
                }
            }
        }
    }
    public void SetTrigger(string stateName)
    {
        if (!_stateDict.ContainsKey(stateName))
            Debug.LogError("SoundHandler : SetTrigger() No State Key Error. Can't find a key : " + stateName);

        // play sounds
        if ( _bindDict.ContainsKey(stateName))
        {
            foreach(AudioClip clip in _bindDict[stateName])
            {
                _srcDict[stateName].volume = globalVolume;
                _srcDict[stateName].PlayOneShot(clip, _settingDict[stateName + "/" + clip.ToString()].volume);
            }
        }
    }

    // ����� ���
    public void PauseSounds()
    {
        foreach(var value in _srcDict)
        {
            value.Value.Pause();
        }
    }

    public void ResumeSounds()
    {
        foreach (var value in _srcDict)
        {
            value.Value.UnPause();
        }
    }

    public void ResetVolume()
    {
        float volume = 1f;
        foreach (var bind in _bindDict)
        {
            string stateName = bind.Key;
            // bool type
            if (_srcDict.ContainsKey(stateName) == false)
            {
                foreach (AudioClip clip in bind.Value)
                {
                    if (_srcDict.TryGetValue(stateName + "/" + clip.ToString(), out var src))
                    {
                        volume = _settingDict[stateName + "/" + clip.ToString()].volume * globalVolume;
                        src.volume = volume;
                    }
                }
            }
            // trigger type
            else
            {
                _srcDict[stateName].volume = globalVolume;
            }
        }
    }

    // Bind Delete �� �Լ��� ����� �׽�Ʈ ���غ������� ���� ���� �� ���� �ٶ�
    // 202213053 ����ȣ
    public void DeleteAllBind() {
        foreach(var bind in _bindDict)
        {
            bind.Value.Clear();
        }
        _bindDict.Clear();
        foreach(var src in _srcDict)
        {
            Destroy(src.Value.gameObject);
            _srcDict.Remove(src.Key);
        }
    }

    public void DeleteBind(string stateName)
    {
        if (!_bindDict.ContainsKey(stateName)) 
            Debug.LogWarning("SoundHandler : DeleteBind() No Clip Error. Can't find a clip : " + stateName);
        else
        {
            foreach (var value in _srcDict)
            {
                string keyName = value.Key.Split("/")[0];
                if (stateName == keyName)
                {
                    Destroy(value.Value.gameObject);
                    _srcDict.Remove(value.Key);
                }
            }
            _bindDict[stateName].Clear();
            _bindDict.Remove(stateName);
        }
    }
    public void DeleteBind(string stateName, AudioClip clip)
    {
        if (!_bindDict.ContainsKey(stateName)) 
            Debug.LogWarning("SoundHandler : DeleteBind() No State Error. Can't find a State : " + stateName);
        else {
            if (!_bindDict[stateName].Contains(clip))
                Debug.LogWarning("SoundHandler : DeleteBind() No Clip Error. Can't find a clip : " + clip.ToString());

            _bindDict[stateName].Remove(clip);

            string key = stateName + "/" + clip.ToString();
            if (_srcDict.ContainsKey(key))
            {
                Destroy(_srcDict[key].gameObject);
                _srcDict.Remove(key);
            }
        }
    }
}
