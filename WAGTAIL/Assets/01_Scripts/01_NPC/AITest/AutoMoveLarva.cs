using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class AutoMoveLarva : MonoBehaviour
{

    public int ObjSize = 3;
    public float CircleR;
    public float Deg = 1;
    public float ObjSpeed;
    public bool Hit = false;
    public float DamagedAnimTimer = 1.5f;
    public float Timer = 0;
    
    public LarvaSpawner MPCenter;
    public Animator animator;
    private Vector3 temp;
    private bool reverse;
    private float rotAngle;
    private float rotDirection;

    private void Start()
    {
        Timer = 0;
        reverse = false;
        //Animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (Hit)
        {
            if (Timer < DamagedAnimTimer)
                Timer += Time.deltaTime;
            else
            {
                Timer = 0;
                Hit = false;
            }
        }
        else
        {
            transform.LookAt(MPCenter.transform);
            temp = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(temp.x, temp.y - rotAngle, temp.z);
            this.transform.RotateAround(MPCenter.transform.position, Vector3.up * rotDirection, (ObjSpeed * Time.deltaTime));

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

    public void SetUp(LarvaSpawner center,/* bool delay = false*/ float delay, float animSpeed)
    {
        MPCenter = center;
        ObjSpeed = center.MoveSpeed;
        ObjSize = center.Polygon;
        CircleR = center.CircleSize;
        reverse = center.Reverse;
        //Debug.Log(reverse);
        DamagedAnimTimer = center.DamagedAnimTimer;
        rotAngle = reverse ? 90f : -90f;
        rotDirection = reverse ? 1f : -1f;
        Deg = 0;
        animator = GetComponent<Animator>();
        animator.speed = animSpeed;
        //if (delay && Animator != null)
        //{
        //    StartCoroutine(WalkAnimCo(0.3f));
        //    //Invoke("WalkAnimPlay", 1f);
        //}
        //else if (!delay && Animator != null)
        //{
        //    StartCoroutine(WalkAnimCo(0f));
        //}

        StartCoroutine(WalkAnimCo(delay));
    }

    IEnumerator WalkAnimCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play("Walk");
    }

    //private void WalkAnimPlay()
    //{
    //    Animator.SetTrigger("isWalk");
    //}

    public void OnDamage()
    {
        Timer = 0;
        Hit = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        /*************************************
         *   띵호가 추가함.....
         * ***/
        if (other.gameObject.CompareTag("Player"))
        {

            Debug.Log($"사망");
            other.gameObject.GetComponent<Player>().isDead = true;
        }


        /***************************************
         *   PlatformObject에 대한 처리...
         * ***/

        //if (!other.gameObject.CompareTag("Platform")) return;

        //bool isGround = (other.gameObject.GetComponent<Collision>().GetContact(0).normal.y > 0);
        //bool NoParent = (transform.parent == null);
        //bool SameParent = (transform.parent == collision.transform);

        //if (isGround && (NoParent || SameParent))
        //{
        //    MPCenter.PlatformEnterCount = MPCenter.Larvas.Count; /*애벌레 개수...*/

        //    /***/
        //    PlatformObject obj = collision.gameObject.GetComponent<PlatformObject>();
        //    if (obj == null) return;

        //    for (int i = 0; i < MPCenter.PlatformEnterCount; i++)
        //    {

        //        MPCenter.Larvas[i].transform.parent = collision.transform;
        //        obj.IgnoreCollisionExit(gameObject);
        //        obj.IgnoreCollisionEnter(gameObject);
        //    }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*************************************
         *   띵호가 추가함.....
         * ***/
        if (collision.collider.CompareTag("Player")){
            collision.collider.GetComponent<Player>().isDead = true;
        }


        /***************************************
         *   PlatformObject에 대한 처리...
         * ***/

        if (!collision.gameObject.CompareTag("Platform")) return;

        bool isGround = (collision.GetContact(0).normal.y > 0);
        bool NoParent = (transform.parent == null);
        bool SameParent = (transform.parent == collision.transform);

        if (isGround && (NoParent||SameParent))
        {
            MPCenter.PlatformEnterCount = MPCenter.Larvas.Count; /*애벌레 개수...*/

            /***/
            PlatformObject obj = collision.gameObject.GetComponent<PlatformObject>();
            if (obj == null) return;

            for (int i = 0; i < MPCenter.PlatformEnterCount; i++){

                MPCenter.Larvas[i].transform.parent = collision.transform;
                obj.IgnoreCollisionExit(gameObject);
                obj.IgnoreCollisionEnter(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Platform")) return;
        if (collision.transform != transform.parent) return;

        /***/
        if ((--MPCenter.PlatformEnterCount) <= 0)
        {
            int Count = MPCenter.Larvas.Count;
            for (int i = 0; i < Count; i++){

                MPCenter.Larvas[i].transform.parent = null;
            }
        }
    }

}
