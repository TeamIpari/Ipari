using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject _prev;
    public GameObject _next;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 이전의 위치를 선형 보간으로 따라감. - x(크기)만큼 떨어져서
        if (_next != null)
        {
            Debug.Log("Slerp");
            // 휘는 기능
            transform.position = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
            // 역동적이게 휘는 기능
            //transform.position = Vector3.Slerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
    }

    public void SetPrev(GameObject prev)
    {
        _prev = prev;
    }

    public void SetNext(GameObject next)
    {
        _next = next;
    }
}
