using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IPariUtility;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine.Rendering.Universal;

public class ScoreObject : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    private GameManager _gameManager;
    private Rigidbody _rigidbody;
    //private Vector3 oneVector = Vector3.forward;
    private bool isMagnet = false;
    private float magnetSpeed = 1.1f;
    private float delayTime;      // ���� �ð�.
    private float curtime;        // ��� �ð�.
    private bool fromCoinFlower = true;
    //[SerializeField] private Vector3 Speed;
    [Tooltip("ü���ð�(���� ü���ð� = ü���ð� / div")]
    private const float Height= 1.7f;
    private const float FlightDiv =4.0f; 


    public ScoreType scoreType;
    private void Start()
    {
        _gameManager = GameManager.GetInstance();
        _rigidbody = GetComponent<Rigidbody>();
        isMagnet = false;
        fromCoinFlower = true;
        // ���Ŀ� �ؿ� �ּ� Ǯ������� ���ϸ� ����
        // ��ġ�� ���� Dummy Mesh�̹Ƿ� Coin �𵨸� �Ǵ� FX�ϼ� �Ǹ� �ݵ�� �ּ��� Ǯ�������
        //GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        if(_rigidbody != null && !isMagnet )
        {
            if (curtime < delayTime)
            {
                curtime += Time.deltaTime;
            }
            else
            {
                // �ڼ�
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.useGravity = false;  
                if(fromCoinFlower)
                    isMagnet = true;
                else if(!fromCoinFlower)
                {
                    SpawnVFX();
                    gameObject.SetActive(false);
                }
                curtime = 0;
            }    
        }
        if(isMagnet)
        {
            // ���� ���� ���ϱ�
            Vector3 directionToMagnet = Player.Instance.transform.position - transform.position;

            float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
            float magentDistance = (10 / distance) * 1.25f;
            _rigidbody.velocity = directionToMagnet * magentDistance;
        }
    }

    // method�� �ӽ�.
    private bool GetHit()
    {
        if (!isMagnet)
        {
            SetTime(Height, FlightDiv);
            gameObject.AddComponent<Rigidbody>();
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _rigidbody.velocity = IpariUtility.CaculateVelocity(this.transform.position, this.transform.position, Height);
            fromCoinFlower = false;
            return false;
        }
        return true;
    }

    public void SetTime(float time, float div)
    {
        delayTime = time / div;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        GetComponent<SphereCollider>().enabled = false;
        switch (scoreType)
        {
            case ScoreType.Coin:
                _gameManager.Coin += 1;
                if(GetHit())
                {
                    SpawnVFX();
                    gameObject.SetActive(false);

                }
                else
                {
                    FModAudioManager.PlayOneShotSFX(FModSFXEventType.Get_Bead);
                }
                //SoundTest.GetInstance().PlaySound("isCoinInteract");
                break;
            case ScoreType.Flower:
                _gameManager.Flower += 1;
                FModAudioManager.PlayOneShotSFX(FModSFXEventType.Get_Flower);
                SpawnVFX();
                gameObject.SetActive(false);
                //SoundTest.GetInstance().PlaySound("isFlowerInteract");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
