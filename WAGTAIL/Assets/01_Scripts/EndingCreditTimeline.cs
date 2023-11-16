using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCreditTimeline : MonoBehaviour
{
    [HideInInspector] public GameObject[] cocosi = new GameObject[11];
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.GetInstance();
        
        for (int i =0; i < cocosi.Length; i++)
        {
            cocosi[i] = transform.GetChild(i + 1).gameObject;
            cocosi[i].SetActive(_gameManager.cocosi[i]);
        }
    }
}
