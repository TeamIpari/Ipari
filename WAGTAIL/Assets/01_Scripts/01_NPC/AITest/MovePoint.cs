using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovePoint : MonoBehaviour
{

    //public
    [Header("Create Rot Around Objects")]
    [Range(3, 100)]
    public int Polygon = 3;
    public Transform Center;
    public Vector3[] SpawnVertices;
    public List<GameObject> Objs;
    public List<GameObject> Larvas;
    public Vector3 Offset = new Vector3(0, 0, 0);
    public float caculate;  // defualt 6.25f -> 4.75f
    public float initPrefabSize = 1.0f;

    public int CreatePoint;
    public float MoveSpeed = 0; 

    public GameObject[] LarvaPrefabs;
    public bool Reverse = false;
    
    public float DamagedAnimTimer = 1.5f;
    //public int tails;
    [HideInInspector] public float CircleSize = 1.0f;
    private int count = 1;

    // Start is called before the first frame update
    void Start()
    {
        // 중심 설정
        if (Center == null)
            Center = this.transform;
        CircleSize = Polygon / (caculate / initPrefabSize);
        CreatePoint = Polygon / CreatePoint;
        if (MoveSpeed == 0)
            MoveSpeed = 50f;
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

            if ((ver1 % (CreatePoint * count)) == 0.0f)
            {
                count += 1;
                // ver2는 -- 연산자임에 반해 LarvaCur은 ++인 이유
                // 개발자들이 Inspector에는 머리 - 몸통 - 꼬리 순으로 넣을 것이기 때문에 반대로 연산하게 만듬.
                for (int ver2 = ver1, LarvaCur = LarvaPrefabs.Length - 1; ver2 > ver1 - LarvaPrefabs.Length; ver2--)
                {
                    GameObject obj2 = Instantiate(LarvaPrefabs[LarvaCur--]);
                    obj2.transform.rotation = Quaternion.Euler(0f , 0f ,0f);
                    obj2.AddComponent<AutoMoveLarva>().SetUp(this, ver2 % 2 == 0 ? false: true);
                    obj2.transform.localScale = new Vector3(initPrefabSize, initPrefabSize, initPrefabSize);
                    obj2.transform.position = Objs[ver2 - 1].transform.position;
                    obj2.transform.parent = this.transform;
                    Larvas.Add(obj2);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
