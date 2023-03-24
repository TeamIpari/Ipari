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

    [Header("인식 범위(int)")]
    public int radius;

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

        int _x = (int)_Player.position.x;
        int _y = (int)_Player.position.y;
        int _z = (int)_Player.position.z;

        Vector3 _vec = new Vector3();
        
        for(int x = _x - radius ; x < _x + radius; x++)
        {
            //for(int y = _y; y < _y + radius; y++)
            {
                for(int z = _z - radius; z < _z + radius; z++)
                {
                    _vec = new Vector3(x + .5f, _y, z + .5f);
                    //Debug.Log(x.ToString() + ", " +y.ToString() + ", " + z.ToString());
                    if (dic_Object.ContainsKey(_vec))
                    {
                        Debug.Log("get");
                        dic_Object[_vec].gameObject.SetActive(true);
                    }
                }
            }
        }
        //Collider[] collider = Physics.OverlapSphere(_Player.position, radius);

        //foreach(var obj in collider)
        //{
        //    obj.gameObject.SetActive(true);

        //}

        // 플레이어 기준으로 일정 범위 체크 후 범위 내의 타일들 SetActive(On);


    }

    private void FixedUpdate()
    {
        // 초당 한 번 씩 인식되는 타일들에게 신호 보내기
        FirstCheck();

    }

    public void AddTiles(GameObject obj)
    {
        dic_Object.Add(obj.transform.position, obj);
        obj.SetActive(false);
    }

}
