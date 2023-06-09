using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // Public Properties
    private float _volume = 1f;
    public float volume { 
        get => _volume; 
        set {
            _volume = value;
            _volume = Mathf.Clamp(_volume, 0, 1f);
            DeleteNullHandlers();
            for (int i = 0; i < _handlers.Count; i++)
                _handlers[i].ResetVolume();
        }
     }

    // Private Members
    List<SoundHandler> _handlers;
    List<SoundHandler> handlers { 
        get
        {
            if (_handlers == null)
                _handlers = new List<SoundHandler>();
            return _handlers;
        }
    }

    void DeleteNullHandlers()
    {
        for (int i = 0; i < handlers.Count; i++){
            if (_handlers[i] == null)
                _handlers.RemoveAt(i);
        }
    }

    public void RegisterHandler(SoundHandler handler)
    {
        handlers.Add(handler);
    }

    public void PauseAll()
    {
        DeleteNullHandlers();
        for (int i = 0; i < _handlers.Count; i++)
        {
            _handlers[i].PauseSounds();
        }
    }

    public void ResumeAll()
    {
        DeleteNullHandlers();
        for (int i = 0; i < _handlers.Count; i++)
        {
            _handlers[i].ResumeSounds();
        }
    }
}
