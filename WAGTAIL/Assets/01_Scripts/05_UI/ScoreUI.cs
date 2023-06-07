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
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.GetInstance();
        _tens.sprite = _numberSprite[0];
        _units.sprite = _numberSprite[0];
    }

    private void Update()
    {
        _units.sprite = _numberSprite[_gameManager.Coin % 10];
        _tens.sprite = _numberSprite[(_gameManager.Coin % 100) / 10];
    }
    
    
}
