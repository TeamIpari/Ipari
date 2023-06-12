using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject _spriteMask;
    [SerializeField] private float _startScale;
    [SerializeField] private float _endScale;
    [SerializeField] private float _time;
    [SerializeField] private float _coolDown;

    private bool _isChange;
    private bool _isPlay;
    private Vector3 _start;
    private Vector3 _end;

    private float _currentTime;
    private float _coolDownTime;

    // Start is called before the first frame update
    void Start()
    {
        _start = new Vector3(_startScale, _startScale, _startScale);
        _end = new Vector3(_endScale, _endScale, _endScale);
        _isChange = false;
        _isPlay = false;
        _currentTime = 0;
        _coolDownTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_spriteMask.transform.localScale == _end)
        {
            _coolDownTime += Time.deltaTime;

            if(_coolDownTime >= _coolDown)
            {
                if (!_isPlay)
                {
                    SoundTest.GetInstance().PlaySound("isDeathUI");
                    _isPlay = true;
                }
                ChangePoint();
            }
        }

        else
        {
            _currentTime += Time.deltaTime;
            ChangeScale(_currentTime);
        }
    }

    void ChangeScale (float t)
    {
        _spriteMask.transform.localScale = Vector3.Lerp(_start, _end, t / _time);
    }

    private void ChangePoint()
    {
        Vector3 _currentPoint = _start;
        _start = _end;
        _end = _currentPoint;
        _currentTime = 0;
        _coolDownTime = 0;

        // DeathUI 출력이 끝나면 SetActive(false) 시켜줌
        if (_isChange)
        {
            _isChange = false;
            gameObject.SetActive(false);
        }

        else
        {
            _isChange = true;
        }
    }
}
