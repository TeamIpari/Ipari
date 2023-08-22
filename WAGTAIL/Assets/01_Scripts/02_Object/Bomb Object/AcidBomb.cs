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

    public override void ShotDirection(Vector3 vector3)
    {
        DirectionLine = true;
        BulletRigidBody.velocity = vector3;
    }

    public override void SetDirection(Vector3 vector3)
    {
        //base.SetDirection(vector3);
        DirectionLine = false;
        Direction = vector3;
    }

    // Start is called before the first frame update
    void Awake()
    {
        BulletRigidBody = GetComponent<Rigidbody>();

        if (BulletRigidBody == null)
        {
            this.AddComponent<Rigidbody>();
            BulletRigidBody = GetComponent<Rigidbody>();
        }
        Damage = Damage == 0 ? 10 : Damage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!DirectionLine)
        {
            BulletRigidBody.velocity = Direction;
        }
    }

    void BulletHit(Transform target)
    {
        // 방향 벡터 구하기
        Vector3 bombPos = target.position - transform.position;
        float distance = Vector3.Distance(target.position, transform.position);
        

        GameObject hitFX = GameObject.Instantiate(HitFX);

        hitFX.transform.position = transform.position + bombPos.normalized;
        Destroy(hitFX, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 강띵호가 추가함
        if (other.CompareTag("Player"))
        {
            Debug.Log("맞음");
            other.GetComponent<Player>().isDead = true;
        }
        // ==========================================================
        if (other.CompareTag("Platform"))
        {
            other.GetComponent<IEnviroment>().ExecutionFunction(0.0f);
        }
        BulletHit(other.transform);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 강띵호가 추가함
        if(collision.collider.CompareTag("Player"))
        {
            Debug.Log("맞음");
            collision.collider.GetComponent<Player>().isDead = true;
        }
        // ==========================================================
        if (collision.collider.CompareTag("Platform"))
        {
            collision.collider.GetComponent<IEnviroment>().ExecutionFunction(0.0f);
        }
        BulletHit(collision.transform);
        Destroy(this.gameObject);
    }
}
