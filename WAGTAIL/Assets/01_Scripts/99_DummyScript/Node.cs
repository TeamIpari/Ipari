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
