using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheckManager : MonoBehaviour
{
    private static ObjectCheckManager instance;

    public static ObjectCheckManager Instance
    {
        get { return instance; }

    }
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
