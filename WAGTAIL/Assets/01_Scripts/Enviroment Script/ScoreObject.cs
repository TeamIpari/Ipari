using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    private GameManager _gameManager;

    public ScoreType scoreType;

    private void Start()
    {
        _gameManager = GameManager.GetInstance();
        // 추후에 밑에 주석 풀어줘야함 안하면 죽음
        // 배치를 위한 Dummy Mesh이므로 Coin 모델링 또는 FX완성 되면 반드시 주석을 풀어줘야함
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        switch (scoreType)
        {
            case ScoreType.Coin:
                _gameManager.coin += 1;
                break;
            case ScoreType.Flower:
                _gameManager.flower += 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        SpawnVFX();
        gameObject.SetActive(false);
    }

    private void SpawnVFX()
    {
        if (_interactionVFX != null)
        {
            GameObject exploVFX = Instantiate(_interactionVFX, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(exploVFX, 2);
        }

        else
            Debug.LogWarning("InteractionVFX was missing!");
    }
}
