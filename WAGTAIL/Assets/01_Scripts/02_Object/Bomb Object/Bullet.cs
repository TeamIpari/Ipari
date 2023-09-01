using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    //================================================
    /////           propertys and Fields          ////
    //================================================
    public Rigidbody BulletRigidBody;
    public bool DirectionLine = false;
    public int Damage; // �ӽ÷� ����� ���� ��ź�� ������.
    public GameObject HitFX;



    //===============================================
    /////           Virtual methods             /////
    //===============================================
    public virtual void ShotDirection(Vector3 vector3)
    {

    }
    public virtual void SetDirection(Vector3 vector3) 
    {
    
    }

    public virtual void Flying()
    {

    }
}
