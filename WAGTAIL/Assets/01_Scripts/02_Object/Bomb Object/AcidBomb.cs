using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AcidBomb : Bullet
{
    Vector3 Direction;

    public override void Flying()
    {
        base.Flying();
    }

    public override void SetDirection(Vector3 vector3, float speed)
    {
        //base.SetDirection(vector3);
        Direction = vector3.normalized * speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        BulletRigidBody = GetComponent<Rigidbody>();
        if (BulletRigidBody == null)
        {
            this.AddComponent<Rigidbody>();
            BulletRigidBody = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        BulletRigidBody.velocity = Direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
