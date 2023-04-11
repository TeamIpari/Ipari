using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Mono.Cecil;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR.Haptics;

[System.Serializable]
public struct vec_point
{

    public Vector3 Pos;
    [SerializeField]
     Vector3 p1;
    [SerializeField]
    Vector3 p2;
    [SerializeField]
    Vector3 p3;
    [SerializeField]
    Vector3 p4;

    public Vector3 P1
    {
        get
        {
            return p1;
        }
        set
        {
            p1 = value;
        }
    }

    public Vector3 P2
    {
        get
        {
            return p2;
        }
        set
        {
            p2 = value;
        }
    }

    public Vector3 P3
    {
        get
        {
            return p3;
        }
        set
        {
            p3 = value;
        }
    }

    public Vector3 P4
    {
        get
        {
            return p4;
        }
        set
        {
            p4 = value;
        }
    }

    public vec_point(Vector3 _p1, Vector3 _p2, Vector3 _p3, Vector3 _p4, Vector3 _pos)
    {
        Pos = _pos;
        p1 = _p1;
        p2 = _p2;
        p3 = _p3;   
        p4 = _p4;


    }
}

/// <summary>
/// 3차 베지어 곡선에 대한 스크립트.
/// A ~ C의 공간을 총 3번 보간을 하여 3차 베지어 곡선이라고 함.
/// </summary>
public class Test : MonoBehaviour
{

    public int cur = 0;

    public GameObject obj;
    [Range(0f, 1f)]
    public float value = 0;

    public List<vec_point> point = new List<vec_point>();

    public vec_point curPoint = new vec_point();

    //public Vector3 p1;
    //public Vector3 p2;
    //public Vector3 p3;
    //public Vector3 p4;

    private void Start()
    {
        //vec_point p = new vec_point(new Vector3(-5, 1, 0), new Vector3(-5, 1, 5), new Vector3(5, 1, 5), new Vector3(5, 1, 0), Vector3.zero);
        //point.Add(p);
        obj = Camera.main.gameObject;
        curPoint = point[0];
        cur = 0;
    }

    private void Update()
    {
        if(obj == null)
            return;
        
        obj.transform.position = BerzierTest(curPoint.P1, curPoint.P2, curPoint.P3, curPoint.P4, value);
        if ( value < 1f)
        {
            value += 0.1f * Time.deltaTime;
        }
        else if (cur < point.Count - 1)
        {
            cur++;
            curPoint = point[cur];
            value = 0;
        }
    }

    public Vector3 BerzierTest(
        Vector3 _p1,
        Vector3 _p2,
        Vector3 _p3,
        Vector3 _p4,
        float _value )
    {

        Vector3 A = Vector3.Lerp(_p1, _p2, _value);

        Vector3 B = Vector3.Lerp(_p2, _p3, _value);

        Vector3 C = Vector3.Lerp(_p3, _p4, _value);


        Vector3 D = Vector3.Lerp(A, B, _value);
        Vector3 E = Vector3.Lerp(B, C, _value);


        Vector3 F = Vector3.Lerp(D, E, _value);

        return F;
    }
}


/// <summary>
/// 에디터와 스크립트는 같은 스크립트 내에서 동시에 작동이 불가능함.
/// 때문에 CanEditMultipleObjects를 넣어주어 스크립트 내부에서 동시에 작동이 가능하도록 해주는 것이 중요.
/// 
/// 
/// 
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(Test))]
public class Test_Editor : Editor
{
    // 이걸 Scene에서 여러개 띄울 수 있게 해봐 ㅇㅇ

    private void OnSceneGUI()
    {

        Test Generator = (Test)target;
        for(int _num = 0;_num < Generator.point.Count; _num ++ )
        {
            // 전역 변수로 지정한 네 가지의 포인트를 씬 내부에서 보고 이동할 수 있도록 해줄 것임.
            vec_point curPoint = Generator.point[_num];
            // 정점의 위치를 보여주며 이동 또한 해당 스크립트에서 진행됨.
            //Generator.point.P1 = Handles.PositionHandle(Generator.curPoint.P1, Quaternion.identity);
            //Generator.point.P2 = Handles.PositionHandle(Generator.curPoint.P2, Quaternion.identity);
            //Generator.point.P3 = Handles.PositionHandle(Generator.curPoint.P3, Quaternion.identity);
            //Generator.point.P4 = Handles.PositionHandle(Generator.curPoint.P4, Quaternion.identity);

            curPoint.P1 = Handles.PositionHandle(curPoint.P1, Quaternion.identity);
            curPoint.P2 = Handles.PositionHandle(curPoint.P2, Quaternion.identity);
            curPoint.P3 = Handles.PositionHandle(curPoint.P3, Quaternion.identity);
            curPoint.P4 = Handles.PositionHandle(curPoint.P4, Quaternion.identity);

            // 한 정점에서 또다른 정점까지 선을 그려주는 기능
            Handles.DrawLine(curPoint.P1, curPoint.P2);
            //Handles.DrawLine(Generator.p2, Generator.p3);
            Handles.DrawLine(curPoint.P3, curPoint.P4);


            int Detail = 100;
            // 베지어 커브 이동 경로를 그려주는 함수.
            for (float i = 0; i < Detail; i++)
            {
                float value = i / Detail;
                Vector3 Before = Generator.BerzierTest(curPoint.P1, curPoint.P2, curPoint.P3, curPoint.P4, value);
                value = (i + 1) / Detail;
                Vector3 After = Generator.BerzierTest(curPoint.P1, curPoint.P2, curPoint.P3, curPoint.P4, value);

                Handles.DrawLine(Before, After);
            }
        }

    }
}
