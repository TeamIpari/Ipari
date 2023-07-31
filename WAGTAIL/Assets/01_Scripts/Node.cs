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
        if(_useNode &&  _parent.GetComponent<LinearInterpolation>().CheckUsingNode())       // Use���¸� ������� ����.
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
                // �ִ� ���
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
        // ������ ��ġ�� ���� �������� ����. - x(ũ��)��ŭ ��������
        if (_next != null && !_useNode && _prev != null
            /*&& !_parent.GetComponent<LinearInterpolation>().CheckUsingNode()*/)
        {
            // �ִ� ���
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
        // �� �� ����
        return new Vector3(transform.position.x - halfsize_1, transform.position.y, transform.position.z );
    }

    public LinearInterpolation GetParent()
    {
        return _parent.GetComponent<LinearInterpolation>();
    }

    // ���� ��尡 ������̶�� ���� �˷��ִ� �޼���
    public void GetNode()
    {
        _useNode = _useNode == true ? false : true;

        if (_useNode)
            _parent.GetComponent<LinearInterpolation>()._CurRope = this.gameObject;
    }

    // parent�� ���õ� ��带 �ٽ� �־��ִ� �޼���
    public void SetNode()
    {
        transform.SetParent(_parent.transform);
    }

    // � ������ �ڽİ�ü���� �����ϴ� �޼���
    public void Setparent(GameObject parent)
    {
        _parent = parent;
    }

    // ���� ����� ���� ��带 �����ϴ� �޼��� (Head�� ����)
    public void SetPrev(GameObject prev)
    {
        _prev = prev;
    }

    // ���� ����� ���� ��带 �����ϴ� �޼��� (tail�� ����)
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
