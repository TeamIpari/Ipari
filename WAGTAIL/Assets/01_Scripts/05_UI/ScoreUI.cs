using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _numberSprite;
    [SerializeField] private Image _tens;
    [SerializeField] private Image _units;
    Player _player;

    void Start()
    {
        _player = Player.Instance;
        _tens.sprite = _numberSprite[0];
        _units.sprite = _numberSprite[0];
    }

    void Update()
    {
        _units.sprite = _numberSprite[_player.coin % 10];
        _tens.sprite = _numberSprite[(_player.coin % 100) / 10];
    }
}
