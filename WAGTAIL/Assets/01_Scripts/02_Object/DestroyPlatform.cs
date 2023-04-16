using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    [Tooltip("�ı� �ð�")]
    [SerializeField] public float _destroyTime = 5.0f;
    [Range(0.0f, 10.0f)]
    [Tooltip ("���� �ð�")]
    [SerializeField] public float _produceTime = 5.0f;

    //[SerializeField] public MeshRenderer _mesh; // �޽ø� ����
    //[SerializeField] public Collider _collider; // �ݶ��̴� ���ֱ� ����.
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
            // Destroy ��� �Ұ�. ������ boolean������ üũ�� ��.
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
