using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LinearInterpolation : MonoBehaviour
{
    public List<GameObject> m_Rope = new List<GameObject>();

    public GameObject _HeadRope;
    public GameObject _TailRope;
    public GameObject _CurRope;

    [HideInInspector]
    public float max_Distance;
    [HideInInspector]
    public float min_Distance;
    [HideInInspector]
    public float cur_Distance;

    public float destroy_Distance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int rN = 0; rN <= m_Rope.Count -1 ; rN++)
        {
            m_Rope[rN].AddComponent<Node>().Setparent(this.gameObject);
            Sorting(rN);
        }
        //min_Distance = Vector3.Distance(_HeadRope.transform.position, _TailRope.transform.position);  // A to B 의 거리를 체크     
        //max_Distance = min_Distance + destroy_Distance;
        //cur_Distance = min_Distance;
    }

    // Update is called once per frame
    void Update()
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
            m_Rope[_rN].GetComponent<Node>().SetPrev(_HeadRope);
            m_Rope[_rN].GetComponent<Node>().SetNext(m_Rope[_rN + 1]);
        }
        // 마지막일 때
        else if(_rN == m_Rope.Count - 1)
        {
            m_Rope[_rN].GetComponent<Node>().SetPrev(m_Rope[_rN - 1]);
            _TailRope = m_Rope[_rN];
        }
    }

    public void BrokenRope()
    {
        for(int i = 0; i <= m_Rope.Count - 1; i++)
        {
            Destroy(m_Rope[i]);
        }
    }

    public bool CheckUsingNode()
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
        cur_Distance = Vector3.Distance(_HeadRope.transform.position, _TailRope.transform.position);

        return (int)((cur_Distance - max_Distance) / destroy_Distance * 100);
    }

    public void Linear()
    {

    }

}
