using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject _parent;
    public GameObject _prev;
    public GameObject _next;
    public Vector3 __next;

    public bool _useNode;
    public float halfsize_1 = 0;
    public float halfsize_2 = 0;
    // Start is called before the first frame update
    void Start()
    {

        _useNode = false;
        halfsize_1 = Calcsize();
         //= longer;

        //Debug.Log(__next);
    }

    // Update is called once per frame
    void Update()
    {
        // ������ ��ġ�� ���� �������� ����. - x(ũ��)��ŭ ��������
        if (_next != null && !_useNode 
            && !_parent.GetComponent<LinearInterpolation>().CheckUsingRope())
        {
            // �ִ� ���
            transform.position
                = Vector3.Lerp(transform.position, _next.GetComponent<Node>().GetPrev(), 0.5f);

            // �������̰� �ִ� ���
            // transform.position = Vector3.Slerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
        else if (!_useNode && _next != null)
        {
            transform.position 
                = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
    }

    public Vector3 GetPrev()
    {
        return new Vector3(transform.position.x - halfsize_1, transform.position.y, transform.position.z );

    }


    public Vector3 GetNext()
    {
        // �� �� ����
        return new Vector3(transform.position.x + halfsize_1, transform.position.y, transform.position.z );

    }

    public LinearInterpolation GetParent()
    {
        return _parent.GetComponent<LinearInterpolation>();
    }

    // ���� ��尡 ������̶�� ���� �˷��ִ� �޼���
    public void GetNode()
    {
        _useNode = _useNode == true ? false : true;
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
