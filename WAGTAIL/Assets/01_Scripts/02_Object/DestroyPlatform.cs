using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour, IEnvironment
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

    private MeshRenderer _mesh;
    private Collider _collider;

    public string InteractionPrompt => throw new System.NotImplementedException();

    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
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

    }

    IEnumerator HideComponent()
    {
        yield return new WaitForSeconds(_destroyTime);

        //body.SetActive(false);
        _mesh.enabled = false;
        _collider.enabled = false;
        //_alive = true;
        StartCoroutine(ShowComponent());


    }

    IEnumerator ShowComponent()
    {
        yield return new WaitForSeconds(_produceTime);

        //body.SetActive(true);
        _mesh.enabled = true;
        _collider.enabled = true;
        //_alive = false;
        _hit = false;
    }

    public bool Interact()
    {
        if (!_hit)
        {
            _hit = true;
            StartCoroutine(HideComponent());

        }

        return false;
    }
}
