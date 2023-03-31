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
        // ������ ��ġ�� ���� �������� ����. - x(ũ��)��ŭ ��������
        if (_next != null && !_useNode)
        {
            Debug.Log("Slerp");
            // �ִ� ���
            transform.position 
                = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
            // �������̰� �ִ� ���
            //transform.position = Vector3.Slerp(_prev.transform.position, _next.transform.position, 0.5f);
        }
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
}
