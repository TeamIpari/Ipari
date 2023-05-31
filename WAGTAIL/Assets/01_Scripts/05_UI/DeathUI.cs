using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathUI : MonoBehaviour
{
    private Scale _scale;
    [SerializeField] private float _startScale;
    [SerializeField] private float _endScale;
    [SerializeField] private float _time;

    private float _currentTime;
    // Start is called before the first frame update
    void Start()
    {
        _startScale = _scale.value.y;
        _currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= _time)
        {
            _currentTime = _time;
        }
        
        = Mathf.Lerp(_startScale, _endScale, _currentTime / _time);
        
    }
}
