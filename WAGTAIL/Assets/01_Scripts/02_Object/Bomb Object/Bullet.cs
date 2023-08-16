using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody BulletRigidBody;
    public bool DirectionLine = false;

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
