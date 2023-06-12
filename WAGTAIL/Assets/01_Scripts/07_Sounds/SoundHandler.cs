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
        // 1. Register on Manager (매니저에 자신을 등록하기)
        SoundManager.GetInstance().RegisterHandler(this);
    }

    public void RegisterBool(string keyName) => RegisterState(keyName, SoundKeyType.Boolean);
    public void RegisterTrigger(string keyName) => RegisterState(keyName, SoundKeyType.Trigger);
    private void RegisterState(string keyName, SoundKeyType type)
    {
        // If stateDict already have key that same name with keyName
        // 만약 stateDict 가 keyName 과 같은 키를 이미 갖고 있을 경우
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
        // 만약 stateDict가 keyName 을 갖고 있지 않은 경우 에러
        if (!_stateDict.ContainsKey(stateName))
        {
            Debug.LogError("SoundHandler : Bind() No Key Error. Can't find a key : " + stateName);
            return false;
        }

        // 다 잘 있다면 bind 완료
        else if (!_bindDict.ContainsKey(stateName))
            _bindDict.Add(stateName, new List<AudioClip>());

        // 근데 이미 이 조건에 같은 클립이 있다면 스킵
        else if (_bindDict[stateName].Contains(clip))
            return false;

        // 바인드 완료
        _bindDict[stateName].Add(clip);

        // 미리 AudioSource 만들어서 등록하기

        // bool 같은 경우는 클립 하나당 소스 하나씩 필요해서 bool 일땐 소스 이름을 디테일하게 정하기
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

        // 세팅을 저장하는 딕셔너리에 기본적인 설정 등록
        _settingDict.Add(stateName+"/"+clip.ToString(), new SoundSetting());

        return true;
    }
    public void SetBool(string stateName, bool value)
    {
        // 만약 stateDict가 stateName 을 갖고 있지 않은 경우 에러
        if (!_stateDict.ContainsKey(stateName))
            Debug.LogError("SoundHandler : SetBool() No State Key Error. Can't find a key : " + stateName);

        // 만약 값이 바뀌지 않았다면 스킵
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

    // 잡다한 기능
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

    // Bind Delete 쪽 함수는 제대로 테스트 안해보았으니 문제 생길 시 연락 바람
    // 202213053 박정호
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
