using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Numerics.Vector3;
using Vector3 = System.Numerics.Vector3;

public class DeathUI : MonoBehaviour
{
    private Scale _scale;
    [SerializeField] private float _startScale;
    [SerializeField] private float _endScale;
    [SerializeField] private float _time;

    private Vector3 _start;
    private Vector3 _end;

    private float _currentTime;
    // Start is called before the first frame update
    void Start()
    {
        _scale = GetComponent<Scale>();
        _start = new Vector3(_startScale, _startScale, _startScale);
        _end = new Vector3(_endScale, _endScale, _endScale);
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

        _scale.value = Lerp(_start, _end, _currentTime / _time);

        //Mathf.Lerp(_startScale, _endScale, _currentTime / _time);

    }
}
