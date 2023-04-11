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
/// 3�� ������ ��� ���� ��ũ��Ʈ.
/// A ~ C�� ������ �� 3�� ������ �Ͽ� 3�� ������ ��̶�� ��.
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
/// �����Ϳ� ��ũ��Ʈ�� ���� ��ũ��Ʈ ������ ���ÿ� �۵��� �Ұ�����.
/// ������ CanEditMultipleObjects�� �־��־� ��ũ��Ʈ ���ο��� ���ÿ� �۵��� �����ϵ��� ���ִ� ���� �߿�.
/// 
/// 
/// 
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(Test))]
public class Test_Editor : Editor
{
    // �̰� Scene���� ������ ��� �� �ְ� �غ� ����

    private void OnSceneGUI()
    {

        Test Generator = (Test)target;
        for(int _num = 0;_num < Generator.point.Count; _num ++ )
        {
            // ���� ������ ������ �� ������ ����Ʈ�� �� ���ο��� ���� �̵��� �� �ֵ��� ���� ����.
            vec_point curPoint = Generator.point[_num];
            // ������ ��ġ�� �����ָ� �̵� ���� �ش� ��ũ��Ʈ���� �����.
            //Generator.point.P1 = Handles.PositionHandle(Generator.curPoint.P1, Quaternion.identity);
            //Generator.point.P2 = Handles.PositionHandle(Generator.curPoint.P2, Quaternion.identity);
            //Generator.point.P3 = Handles.PositionHandle(Generator.curPoint.P3, Quaternion.identity);
            //Generator.point.P4 = Handles.PositionHandle(Generator.curPoint.P4, Quaternion.identity);

            curPoint.P1 = Handles.PositionHandle(curPoint.P1, Quaternion.identity);
            curPoint.P2 = Handles.PositionHandle(curPoint.P2, Quaternion.identity);
            curPoint.P3 = Handles.PositionHandle(curPoint.P3, Quaternion.identity);
            curPoint.P4 = Handles.PositionHandle(curPoint.P4, Quaternion.identity);

            // �� �������� �Ǵٸ� �������� ���� �׷��ִ� ���
            Handles.DrawLine(curPoint.P1, curPoint.P2);
            //Handles.DrawLine(Generator.p2, Generator.p3);
            Handles.DrawLine(curPoint.P3, curPoint.P4);


            int Detail = 100;
            // ������ Ŀ�� �̵� ��θ� �׷��ִ� �Լ�.
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
