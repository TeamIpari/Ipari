using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _numberSprite;
    [SerializeField] private Image _units;
    Player _player;

    void Start()
    {
        _player = Player.Instance;
        _units.sprite = _numberSprite[0];
    }

    void Update()
    {
        _units.sprite = _numberSprite[_player.flower % 10];
    }
}
