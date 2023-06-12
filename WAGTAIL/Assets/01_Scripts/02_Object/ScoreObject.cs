using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    private GameManager _gameManager;
    private Rigidbody _rigidbody;
    //private Vector3 oneVector = Vector3.forward;
    private bool isMagnet = false;
    private float magnetSpeed = 1.1f;
    private float delayTime;      // 지연 시간.
    private float curtime;        // 경과 시간.
    [SerializeField] private Vector3 Speed;

    public ScoreType scoreType;
    private void Start()
    {
        _gameManager = GameManager.GetInstance();
        _rigidbody = GetComponent<Rigidbody>();
        isMagnet = false;
        // 추후에 밑에 주석 풀어줘야함 안하면 죽음
        // 배치를 위한 Dummy Mesh이므로 Coin 모델링 또는 FX완성 되면 반드시 주석을 풀어줘야함
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if(_rigidbody != null)
        {
            if (curtime < delayTime)
            {
                curtime += Time.deltaTime;
            }
            else
            {
                Destroy(_rigidbody);
                // 자석
                isMagnet = true;
                curtime = 0;
                //delayTime =f;
            }    
        }
        if(isMagnet)
        {
            transform.position += TargetVector(Player.Instance.transform.position, transform.position);
        }
    }
    

    private Vector3 TargetVector(Vector3 target, Vector3 origin)
    {
        Vector3 distance = target - origin;
        Speed += distance.normalized * 0.1f * Time.deltaTime;




        return Speed;

    }

    public void SetTime(float time)
    {
        delayTime = time / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        switch (scoreType)
        {
            case ScoreType.Coin:
                _gameManager.Coin += 1;
                SoundTest.GetInstance().PlaySound("isCoinInteract");
                break;
            case ScoreType.Flower:
                _gameManager.Flower += 1;
                SoundTest.GetInstance().PlaySound("isFlowerInteract");
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
