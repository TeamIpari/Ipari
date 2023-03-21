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

    [SerializeField]
    public Transform _Player;

    public float radius;


    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
            instance = this;

        if (_Player == null)
            Debug.LogError("Player가 존재하지 않음.");



    }

    

    // Update is called once per frame
    void Update()
    {
        FirstCheck();
    }


    Vector3 FirstCircleCheck(float radius)
    {
        float x = Random.Range(-1.0f, 1.0f);
        float temp = Mathf.Pow(1.0f, 2) - Mathf.Pow(x, 2);
        float z = Mathf.Sqrt(temp);

        return (new Vector3(x, 0.0f, z) /** Random.Range(0.0f, radius)*/);

    }

    void FirstCheck()
    {
        //RaycastHit hit;
        Collider[] collider = Physics.OverlapSphere(_Player.position, radius);

        foreach(var obj in collider)
        {
            
            obj.GetComponent<Objects>()?.ShowMeshData();

        }
    }
}
