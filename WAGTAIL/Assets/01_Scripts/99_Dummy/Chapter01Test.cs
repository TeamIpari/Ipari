using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter01Test : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] private GameObject _camera;
    private bool _isPlaying = false;
    private bool _isEnd = false;
    private float _currentTime;
    [SerializeField] private float time;

    // Update is called once per frame
    void Update()
    {
        if (!_isEnd)
        {
            if (!_isPlaying)
            {
                _player = Player.Instance.gameObject;
                _currentTime = 0;
                _player.GetComponent<CharacterController>().enabled = false;
                _camera.SetActive(true);
                _isPlaying = true;
            }

            else if (_isPlaying)
            {
                if (_currentTime >= time)
                {
                    _player.GetComponent<CharacterController>().enabled = true;
                    _camera.SetActive(false);
                    _isPlaying = false;
                    _isEnd = true;
                }
                _currentTime += Time.deltaTime;
            }
        }
    }
}
