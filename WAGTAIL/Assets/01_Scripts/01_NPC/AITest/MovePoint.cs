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
    public float CircleSize = 1.0f;
    public Transform Center;
    public Vector3[] SpawnVertices;
    public List<GameObject> Objs;
    public List<GameObject> Larvas;
    public Vector3 Offset = new Vector3(0, 0, 0);
    public float caculate = 6.25f;

    public int CreatePoint;

    public GameObject HeadObj;
    public GameObject BodyObj;
    public bool Reverse = false;

    public int tails;
    int count = 1;
    public float DamagedAnimTimer = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        // 중심 설정
        if (Center == null)
            Center = this.transform;
        CircleSize = Polygon / caculate;
        CreatePoint = Polygon / CreatePoint;
        SetVertices(CircleSize, Polygon);

    }

    private void SetVertices(float size, int polygon)
    {
        GameObject CreateObj;
        float m_Vec;
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
                for (int ver2 = ver1; ver2 >= ver1 - tails; ver2--)
                {
                    Debug.Log(Objs[ver2 - 1].transform.position);
                    GameObject obj2 = Instantiate(HeadObj);
                    obj2.AddComponent<AutoMoveLarva>().SetUp(this);

                    obj2.transform.position = Objs[ver2 - 1].transform.position;
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

//[CanEditMultipleObjects]
//[CustomEditor(typeof(MovePoint))]
//public class MovePointEditor : Editor
//{


//    //MovePointEditor()
//    private void OnSceneGUI()
//    {
//        MovePoint Generator = (MovePoint)target;

//        Generator.P1 = Handles.PositionHandle(Generator.P1, Quaternion.identity);
//        Generator.P2 = Handles.PositionHandle(Generator.P2, Quaternion.identity);
//        Generator.P3 = Handles.PositionHandle(Generator.P3, Quaternion.identity);
//        Generator.P4 = Handles.PositionHandle(Generator.P4, Quaternion.identity);


//        //for(int i = 0; i < 10; i++)
//        //{
//        //    Vector3 vector3
//        //}

//    }

//}