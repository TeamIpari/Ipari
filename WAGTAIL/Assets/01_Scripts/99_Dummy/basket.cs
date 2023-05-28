using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
public class basket : MonoBehaviour
{
    // 지붕
    public GameObject _lid;

    //[SerializeField] Vector3 _loopPoint1;
    //[SerializeField] Vector3 _loopPoint2;

    [SerializeField] private GameObject _localPoint1;
    [SerializeField] private GameObject _localPoint2;
    public Vector3 _loopTarget;

    [Range(1, 5)]
    public float _hideTime = 1;

    private void OnDrawGizmos()
    {
        if (_localPoint1 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_localPoint1.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

        if(_localPoint2 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_localPoint2.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        _lid.SetActive(false);
        _loopTarget = _localPoint1.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if( Vector3.Distance(transform.position, _loopTarget) <= 1)
            // 초당 n의 속도로 목표를 향해 움직임.
            _loopTarget = _loopTarget == _localPoint1.transform.position ? _localPoint2.transform.position : _localPoint1.transform.position;
        else
            transform.position = Vector3.MoveTowards(transform.position, _loopTarget, 5f * Time.deltaTime);
    }

    private IEnumerator ShowLid()
    {
        yield return new WaitForSeconds(1.0f);
        _lid.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _lid.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag( "interactable"))
        {
            StartCoroutine(ShowLid());  
        }
    }
}
