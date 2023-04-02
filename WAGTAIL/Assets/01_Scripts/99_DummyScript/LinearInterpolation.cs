using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearInterpolation : MonoBehaviour
{
    public List<GameObject> m_Rope = new List<GameObject>();
    public GameObject HeadRope;
    public GameObject TailRope;
    [HideInInspector]
    public float max_Distance;

    public float destroy_Distance = 5f;
    [HideInInspector]
    public float min_Distance;
    [HideInInspector]
    public float cur_Distance;
    // Start is called before the first frame update
    void Start()
    {

        for (int rN = 0; rN <= m_Rope.Count -1 ; rN++)
        {
            m_Rope[rN].AddComponent<Node>().Setparent(this.gameObject);
            Sorting(rN);
        }
        min_Distance = Vector3.Distance(HeadRope.transform.position, TailRope.transform.position);  // A to B �� �Ÿ��� üũ     
        max_Distance = min_Distance + destroy_Distance;
        cur_Distance = min_Distance;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Sorting(int _rN)
    {
        // �߰��� ��
        if (_rN > 0 && _rN < m_Rope.Count - 1)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(m_Rope[_rN - 1]);
            m_Rope[_rN].GetComponent<Node>().SetNext(m_Rope[_rN + 1]);
        }
        // ó���� ��
        else if(_rN == 0)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(HeadRope);
            m_Rope[_rN].GetComponent<Node>().SetNext(m_Rope[_rN + 1]);
        }
        // �������� ��
        else if(_rN == m_Rope.Count - 1)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(m_Rope[_rN - 1]);
            TailRope = m_Rope[_rN];
        }
    }

    public bool CheckUsingRope()
    {
        for(int i = 0; i < m_Rope.Count; i++)
        {
            if (m_Rope[i].GetComponent<Node>()._useNode)
                return true;
        }
        return false;
    }

    public int Percent()
    {
        cur_Distance = Vector3.Distance(HeadRope.transform.position, TailRope.transform.position);
        
        //Debug.Log();

        return (int)((cur_Distance - max_Distance) / destroy_Distance * 100);

    }

    public void Linear()
    {

    }
}
