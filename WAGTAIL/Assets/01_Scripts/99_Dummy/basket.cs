using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basket : MonoBehaviour
{
    // 지붕
    public GameObject _lid;

    [SerializeField] Vector3 _loopPoint1;
    [SerializeField] Vector3 _loopPoint2;

    [SerializeField] Vector3 _localPoint1;
    [SerializeField] Vector3 _localPoint2;
    public Vector3 _loopTarget;

    [Range(1, 5)]
    public float _hideTime = 1;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        _localPoint1 = transform.position + _loopPoint1;
        _localPoint2 = transform.position + _loopPoint2;
        Gizmos.DrawWireCube(_localPoint1 + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        Gizmos.DrawWireCube(_localPoint2 + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));

    }

    // Start is called before the first frame update
    void Start()
    {
        _lid.SetActive(false);
        _loopTarget = _localPoint1;
    }

    // Update is called once per frame
    void Update()
    {
        if( Vector3.Distance(transform.position, _loopTarget) <= 1)
            // 초당 n의 속도로 목표를 향해 움직임.
            _loopTarget = _loopTarget == _localPoint1 ? _localPoint2 : _localPoint1;
        else
            transform.position = Vector3.MoveTowards(transform.position, _loopTarget, 5f * Time.deltaTime);
    }

    IEnumerator ShowLid()
    {
        yield return new WaitForSeconds(1.0f);
        _lid.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _lid.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "interactable")
        {
            StartCoroutine(ShowLid());  
        }
    }
}
