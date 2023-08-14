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
    public Animator Animator;
    Vector3 temp;


    private void Start()
    {
        timer = 0;
        //Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
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
        {
            transform.LookAt(MPCenter.transform);
            temp = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(temp.x, temp.y +90, temp.z);
            this.transform.RotateAround(MPCenter.transform.position, -Vector3.up, (ObjSpeed * Time.deltaTime));

        }
    }

    void SetRot()
    {
        Vector3 dir = MPCenter.transform.position - transform.position;
        dir.y = transform.position.y;

        Quaternion quat = Quaternion.LookRotation(dir.normalized);
        Vector3 temp = quat.eulerAngles;
        
        transform.rotation = Quaternion.Euler(temp.x , temp.y -45f, temp.z);
    }

    public void SetUp(MovePoint Center, bool delay = false)
    {
        MPCenter = Center;
        ObjSpeed = Center.MoveSpeed;
        ObjSize = Center.Polygon;
        CircleR = Center.CircleSize;
        DamagedAnimTimer = Center.DamagedAnimTimer;
        //SetRot();
        Deg = 0;
        Animator = GetComponent<Animator>();
        if (delay && Animator != null)
        {
            Invoke("WalkAnimPlay", 1f);
        }
        else if (!delay && Animator != null)
        {
            WalkAnimPlay();
        }
    }

    private void WalkAnimPlay()
    {
        Animator.SetTrigger("isWalk");
    }

    public void OnDamage()
    {
        timer = 0;
        Hit = true;

    }


}
