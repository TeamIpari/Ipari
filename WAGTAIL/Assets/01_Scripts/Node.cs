using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject _parent;
    public GameObject _prev;
    public GameObject _next;
    public Vector3 __next;

    public float minDistance = 1f;

    private bool isPrev = false;
    private bool isNext = false;

    public bool _useNode;
    public float halfsize_1 = 0;
    public float halfsize_2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        halfsize_1 = Calcsize();
        isPrev = _prev == null ? false : true;
        isNext = _next == null ? false : true;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Rigidbody>() != null)   
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        NodeSlerp_2();
    }

    private void NodeSlerp_2()
    {
        //if(_useNode)
        //{
        //    return;
        //}
        if (isPrev && Distance(_prev.transform))
        {
            _prev?.GetComponent<Node>().PrevSlerp();
        }
        if (isNext && Distance(_next.transform))
        {
            _next?.GetComponent<Node>().NextSlerp();
        }
    }

    private bool Distance(Transform target)
    {
        return Vector3.Distance(transform.position, target.transform.position) > minDistance ? true : false;        
    }
    
    public void NodeSlerp_1()
    {
        if(_useNode &&  _parent.GetComponent<LinearInterpolation>().CheckUsingNode())       // Use상태면 사용하지 않음.
        {
            _prev?.GetComponent<Node>().PrevSlerp();
            _next?.GetComponent<Node>().NextSlerp();
        }
        else if(!_parent.GetComponent<LinearInterpolation>().CheckUsingNode())
        {
            NodeSlerp();
        }
    }

    public void PrevSlerp()
    {
        try
        {
            if (isPrev)
            {
                transform.position
                    = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
                _prev?.GetComponent<Node>().PrevSlerp();
            }
            else if(!_useNode)
            {
                transform.position = Vector3.Lerp(transform.position, _next.transform.position, 0.5f);
            }
        }
        catch
        {

        }
    }

    public void NextSlerp()
    {
        try
        {
            if (isNext)
            {
                // 휘는 기능
                transform.position
                    = Vector3.Lerp(_next.transform.position, _prev.transform.position, 0.5f);
                _next?.GetComponent<Node>().NextSlerp();
            }
            else if (!_useNode)
            {
                transform.position = Vector3.Lerp(_prev.transform.position, transform.position, 0.5f);
            }
        }
        catch
        {
        }
    }

    public void NodeSlerp()
    {
        // 이전의 위치를 선형 보간으로 따라감. - x(크기)만큼 떨어져서
        if (_next != null && !_useNode && _prev != null
            /*&& !_parent.GetComponent<LinearInterpolation>().CheckUsingNode()*/)
        {
            // 휘는 기능
            transform.position
                = Vector3.Lerp(transform.position, _next.GetComponent<Node>().GetPrev(), 0.5f);
        }
        else if (!_useNode && _next != null && _prev != null)
        {
            transform.position
                = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
    }

    public Vector3 GetPrev()
    {
        return new Vector3(transform.position.x - (transform.position.x - _prev.transform.position.x) , transform.position.y,transform.position.z -( transform.position.z - _prev.transform.position.z));
    }


    public Vector3 GetNext()
    {
        // 안 쓸 예정
        return new Vector3(transform.position.x - halfsize_1, transform.position.y, transform.position.z );
    }

    public LinearInterpolation GetParent()
    {
        return _parent.GetComponent<LinearInterpolation>();
    }

    // 현재 노드가 사용중이라는 것을 알려주는 메서드
    public void GetNode()
    {
        _useNode = _useNode == true ? false : true;

        if (_useNode)
            _parent.GetComponent<LinearInterpolation>()._CurRope = this.gameObject;
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


    public float Calcsize()
    {
        float _height = 0f;

        MeshFilter _mf = GetComponent<MeshFilter>();

        Vector3[] _vertices = _mf.mesh.vertices;

        foreach(var _vertice in _vertices)
        {
            Vector3 _pos = transform.TransformPoint(_vertice);

            if (_pos.y > _height)
            {
                _height = _pos.y;
            }
        }

        return _height;
    }
}
