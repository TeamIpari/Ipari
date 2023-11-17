using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [Header("NumberSprites")]
    [SerializeField] private Sprite[] numberSprite;
    
    [Header("CoinSprites")]
    [SerializeField] private Image coinTens;
    [SerializeField] private Image coinUnits;
    
    [Header("CocosiSprites")]
    [SerializeField] private Image cocosiTens;
    [SerializeField] private Image cocosiUnits;
    
    [Header("DeathSprites")]
    [SerializeField] private Image deathTens;
    [SerializeField] private Image deathUnits;

    [Header("Debug")] 
    [SerializeField] private GameObject check;
    private GameManager _gameManager;

    private void Start()
    {
        SetResult();
    }

    private void Update()
    {
        if (check.activeSelf && Input.GetKeyDown(KeyCode.F))
            SceneLoader.GetInstance().LoadScene(ChapterType.Title.ToString());
    }

    public void SetResult()
    {
        GameManager gameManager = GameManager.GetInstance();
        
        // CoinResult
        coinTens.sprite = numberSprite[(gameManager.Coin % 100) / 10];
        coinUnits.sprite = numberSprite[gameManager.Coin % 10];
        
        // CocosiResult
        int cocosiNum = 0;
        for(int i = 0; i < gameManager.cocosi.Length; i++)
        {
            if(gameManager.cocosi[i])
            {
                cocosiNum++;
            }
        }
        cocosiTens.sprite = numberSprite[(cocosiNum % 100) / 10];
        cocosiUnits.sprite = numberSprite[cocosiNum % 10];
        
        // DeathResult
        deathTens.sprite = numberSprite[(gameManager.deathCount % 100) / 10];
        deathUnits.sprite = numberSprite[gameManager.deathCount % 10];
    }
}
