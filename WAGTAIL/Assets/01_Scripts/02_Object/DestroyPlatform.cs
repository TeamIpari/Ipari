using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    [Tooltip("파괴 시간")]
    [SerializeField] public float _destroyTime = 5.0f;
    [Range(0.0f, 10.0f)]
    [Tooltip ("생성 시간")]
    [SerializeField] public float _produceTime = 5.0f;

    //[SerializeField] public MeshRenderer _mesh; // 메시를 꺼줌
    //[SerializeField] public Collider _collider; // 콜라이더 꺼주기 위함.
    public GameObject body;
    private bool _hit = false;
    private bool _alive = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_alive )
        {

        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
        
    //}

    private void OnCollisionEnter (Collision other)
    {
        Debug.Log("Start");
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Start");
        if (!_hit && other.gameObject.tag == "Player")
        {
            // Destroy 사용 불가. 때문에 boolean값으로 체크를 함.
            _hit = true;
            StartCoroutine(HideComponent());
        }
    }

    IEnumerator HideComponent()
    {
        yield return new WaitForSeconds(_destroyTime);

        body.SetActive(false);
        //_alive = true;
        StartCoroutine(ShowComponent());


    }

    IEnumerator ShowComponent()
    {
        yield return new WaitForSeconds(_produceTime);

        body.SetActive(true);
        //_alive = false;
        _hit = false;
    }
}
