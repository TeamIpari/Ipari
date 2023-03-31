using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearInterpolation : MonoBehaviour
{
    public List<GameObject> m_Rope = new List<GameObject>();
    public GameObject HeadRope;
    public GameObject TailRope;
    // Start is called before the first frame update
    void Start()
    {

        for (int rN = 0; rN <= m_Rope.Count -1 ; rN++)
        {
            m_Rope[rN].AddComponent<Node>().Setparent(this.gameObject);
            Sorting(rN);
        }


    }

    // Update is called once per frame
    void Update()
    {
        //Linear();
    }

    public void Sorting(Node _node)
    {

    }
    public void Sorting(int _rN)
    {
        // 중간일 때
        if (_rN > 0 && _rN < m_Rope.Count - 1)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(m_Rope[_rN - 1]);
            m_Rope[_rN].GetComponent<Node>().SetNext(m_Rope[_rN + 1]);
        }
        // 처음일 때
        else if(_rN == 0)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(HeadRope);
            m_Rope[_rN].GetComponent<Node>().SetNext(m_Rope[_rN + 1]);
        }
        // 마지막일 때
        else if(_rN == m_Rope.Count - 1)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(m_Rope[_rN - 1]);
        }
    }

    public void Linear()
    {
    }

   

}
