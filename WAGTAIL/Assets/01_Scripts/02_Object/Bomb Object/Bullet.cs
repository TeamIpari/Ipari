using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody BulletRigidBody;
    public bool DirectionLine = false;
    public int Damage; // 임시로 만들어 놓은 폭탄의 데미지.

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
