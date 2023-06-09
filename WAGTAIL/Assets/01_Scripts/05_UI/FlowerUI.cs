using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _numberSprite;
    [SerializeField] private Image _units;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = GameManager.GetInstance();
        _units.sprite = _numberSprite[0];
    }

    void Update()
    {
        _units.sprite = _numberSprite[_gameManager.Flower % 10];
    }
}
