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


    [System.Serializable]
    public class SerializeDic : SerializeDictionary<Vector3, GameObject> { }
    [SerializeField]
    public SerializeDic dic_Object = new SerializeDic ();

    [SerializeField]
    public Transform _Player;

    public float radius;

    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {

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
        Collider[] collider = Physics.OverlapSphere(_Player.position, radius);

        foreach(var obj in collider)
        {
            obj.gameObject.SetActive(true);
            //obj.GetComponent<Objects>()?.ShowMeshData();

        }
    }

    public void AddTiles(GameObject obj)
    {
        dic_Object.Add(obj.transform.position, obj);
        obj.SetActive(false);
    }

}
