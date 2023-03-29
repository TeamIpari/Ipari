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
        // ������ ��ġ�� ���� �������� ����. - x(ũ��)��ŭ ��������
        if (_next != null)
        {
            Debug.Log("Slerp");
            // �ִ� ���
            transform.position = Vector3.Lerp(_prev.transform.position, _next.transform.position, 0.5f);
            // �������̰� �ִ� ���
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
