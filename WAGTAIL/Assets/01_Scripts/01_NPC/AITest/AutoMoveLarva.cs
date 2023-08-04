using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveLarva : MonoBehaviour
{

    public int ObjSize = 3;
    public float CircleR;
    public float Deg = 1;
    public float ObjSpeed;
    public bool Hit = false;
    public float DamagedAnimTimer = 1.5f;
    public float timer = 0;
    public MovePoint MPCenter;



    private void Start()
    {
        timer = 0;
        //timer = DamagedAnimTimer;
    }

    void Update()
    {
        if (Hit)
        {
            if (timer < DamagedAnimTimer)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                Hit = false;
            }
        }
        else 
            this.transform.RotateAround(MPCenter.transform.position, Vector3.up, (ObjSpeed * Time.deltaTime));
        //MoveCircle();
    }

    public void SetUp(MovePoint Center)
    {
        MPCenter = Center;
        ObjSpeed = 50f;
        ObjSize = Center.Polygon;
        CircleR = Center.CircleSize;
        DamagedAnimTimer = Center.DamagedAnimTimer;
        Deg = 0;
    }

    public void OnDamage()
    {
        timer = 0;
        // 데미지를 입음
        Hit = true;

    }


}
