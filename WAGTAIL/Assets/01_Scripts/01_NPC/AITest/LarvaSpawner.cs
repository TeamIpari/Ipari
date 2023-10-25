using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LarvaSpawner : MonoBehaviour
{

    //public
    [Header("Create Rot Around Objects")]
    [Range(3, 100)]
    public int Polygon = 3;
    public Transform Center;
    public Vector3[] SpawnVertices;
    public List<GameObject> Objs;
    public const float Caculate = 3.85f;  // defualt 6.25f -> 4.75f

    [Header("Larva Properties")]
    public List<GameObject> Larvas;
    public Vector3 Offset = new Vector3(0, 0, 0);
    public float InitPrefabSize = 1.0f;
    public int SpawnEA;
    public float MoveSpeed = 0;
    [Tooltip("각 다리마다의 딜레이")]
    public float AnimDelay = 0.25f;
    public float AnimSpeed = 1f;

    public GameObject[] LarvaPrefabs;
    public bool Reverse = false;
    
    public float DamagedAnimTimer = 1.5f;
    
    //public int tails;
    /*[HideInInspector]*/ public float CircleSize = 1.0f;
    private int count = 1;

    public int PlatformEnterCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 중심 설정
        if (Center == null)
            Center = this.transform;
        CircleSize = Polygon / (Caculate / InitPrefabSize);
        if (MoveSpeed == 0)
            MoveSpeed = 50f;
        if (SpawnEA == 0 || SpawnEA > Polygon - LarvaPrefabs.Length)
            SpawnEA = 1;
        AnimSpeed = CircleSize / Caculate;
        SetVertices(CircleSize, Polygon);
    }

    private void SetVertices(float size, int polygon)
    {
        Vector3 createPos;

        for (int i = 0; i < Objs.Count; i++)
        {
            Destroy(Objs[i]);
        }
        Objs.Clear();
        Larvas.Clear();

        SpawnVertices = new Vector3[polygon + 1];

        SpawnVertices[0] = new Vector3(0, 0, 0) + Offset;

        for (int ver1 = 1; ver1 <= polygon; ver1++)
        {
            float angle = -ver1 * (Mathf.PI * 2.0f) / polygon;
            SpawnVertices[ver1]
                = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * size) + Offset;

            GameObject obj = new GameObject();
            createPos = SpawnVertices[ver1] + transform.position;
            obj.transform.position = createPos;
            obj.transform.parent = this.transform;
            Objs.Add(obj);
        }

        float delayTime = 0;

        for(int i = 1; i <= SpawnEA; i++)
        {
            for (int ver2 = polygon / i, LarvaCur = LarvaPrefabs.Length - 1; ver2 > (polygon / i)- LarvaPrefabs.Length/* - (polygon / i)*/; ver2--)
            {
                //Debug.Log($"{ver2}");
                GameObject obj2 = Instantiate(LarvaPrefabs[LarvaCur--]);
                obj2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Debug.Log($"{ver2} % {2} = {ver2 % 2}");
             
                obj2.AddComponent<AutoMoveLarva>().SetUp(this, delayTime, AnimSpeed);
                obj2.transform.localScale = new Vector3(InitPrefabSize, InitPrefabSize, InitPrefabSize);
                obj2.transform.position = Objs[ver2 - 1].transform.position;
                obj2.transform.parent = this.transform;
                Larvas.Add(obj2);
                delayTime += AnimDelay;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
