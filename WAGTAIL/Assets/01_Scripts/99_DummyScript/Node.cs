using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject _parent;
    public GameObject _prev;
    public GameObject _next;
    public bool _useNode;
    // Start is called before the first frame update
    void Start()
    {

        _useNode = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 이전의 위치를 선형 보간으로 따라감. - x(크기)만큼 떨어져서
        if (_next != null && !_useNode)
        {
            Debug.Log("Slerp");
            // 휘는 기능
            transform.position 
                = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
            // 역동적이게 휘는 기능
            //transform.position = Vector3.Slerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
    }

    // 현재 노드가 사용중이라는 것을 알려주는 메서드
    public void GetNode()
    {
        _useNode = _useNode == true ? false : true;
    }
    

    // parent에 선택된 노드를 다시 넣어주는 메서드
    public void SetNode()
    {
        transform.SetParent(_parent.transform);
    }

    // 어떤 덩굴의 자식객체인지 세팅하는 메서드
    public void Setparent(GameObject parent)
    {
        _parent = parent;
    }

    // 현재 노드의 이전 노드를 세팅하는 메서드 (Head는 제외)
    public void SetPrev(GameObject prev)
    {
        _prev = prev;
    }

    // 현재 노드의 다음 노드를 세팅하는 메서드 (tail은 제외)
    public void SetNext(GameObject next)
    {
        _next = next;
    }
}
