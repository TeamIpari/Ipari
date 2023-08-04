using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SwitchRange
{
    public Vector3 offset;
    public Vector3 Scale;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class TreeObstacles : MonoBehaviour
{
    //=====================================
    ////      Property And Fields      ////
    //=====================================
    [SerializeField] public SwitchRange Range = new SwitchRange();
    [SerializeField] public Transform   TreePosition;
    [SerializeField] public Transform   FallDownDir;
    [SerializeField] public float       FallDownSpeed = 1f;
    [SerializeField] public bool        FallDownUsedCollision = false;
    [SerializeField] public float       FallDownTargetAngle = 90f;

    private const float ReboundValue = .325f;
    private Rigidbody   _Body;
    private Collider    _Collider;
    private bool        _IsSwitch = false;

    private float fallDownSpeed = 0f;
    private float fallDownRot   = 0f;
    private float rebound       = .4f;
    private float maxRebound    = .4f;


    //=====================================
    ////         Magic Methods         ////
    //=====================================
    private void Start()
    {
        /**Intialized*/
        _Body = GetComponent<Rigidbody>();
        _Body.useGravity = false;
        _Body.mass = 100f;
        _Body.isKinematic = true;
        _Body.Sleep();

        _Collider= GetComponent<Collider>();
        maxRebound = rebound;
    }

    private void FixedUpdate()
    {
        //���� �ȿ� �÷��̾ ���Դ��� üũ.
        if(_IsSwitch==false && TriggerTestPlayer())
        {
            _Collider.isTrigger = !FallDownUsedCollision;
            _Body.isKinematic = false;
            _IsSwitch = true;
            _Body.WakeUp();
            StartCoroutine(FallDownProgress());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Platform")
        {
            //������ �浹�� ���ǵ��� �ı��Ѵ�.
            //TODO: �ı��� ���ǵ��� ����Ʈ��, �� ������ OnDestory()���� �����ϵ��� ������.
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            //������ �浹�� ���ǵ��� �ı��Ѵ�.
            //TODO: �ı��� ���ǵ��� ����Ʈ��, �� ������ OnDestory()���� �����ϵ��� ������.
            Destroy(collision.gameObject);
            return;
        }

        //���� ������ ƨ�ܳ���.
        if (rebound > 0)
        {
            fallDownSpeed = -rebound;
            fallDownRot -= rebound;
            rebound -= maxRebound * ReboundValue;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 center = (TreePosition.position + Range.offset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, Range.Scale);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine( TreePosition.position, center );
    }



    //=====================================
    ////         Core Methods          ////
    //=====================================
    private IEnumerator FallDownProgress()
    {
        #region Omission

        /***************************************
         * ��꿡 �ʿ��� ��� �͵��� ���Ѵ�.
         ***/
        Vector3 currRot = transform.eulerAngles;
        Vector3 standingDir = (FallDownDir.position - TreePosition.position).normalized;
        Vector3 fallDownDir = new Vector3(standingDir.x, 0f, standingDir.z).normalized;

        WaitForFixedUpdate waitTime = new WaitForFixedUpdate();
        Vector3 startPos = new Vector3()
        {
            x = transform.position.x,
            y = TreePosition.position.y,
            z = transform.position.z
        };
        Vector3 heightDir    = (transform.position - startPos);
        float   height       = heightDir.magnitude * (heightDir.y>0?1f:-1f);
        Vector3 fallDownDir2 = new Vector3()
        {
            x = fallDownDir.z,
            y = 0f,
            z = -fallDownDir.x
        };


        /****************************************
         * ������ ��������.
         ***/
        while (fallDownRot<= FallDownTargetAngle && rebound>0f)
        {
            fallDownSpeed += FallDownSpeed * Time.fixedDeltaTime;
            fallDownRot   += fallDownSpeed;

            //������ �������� ���� ���� ���.
            if(fallDownRot>= FallDownTargetAngle) {
               FModAudioManager.PlayOneShotSFX(FModSFXEventType.Stone_Broken, transform.position, 1f);
            }

            //������ �ٴڿ� �پ��ֵ��� ����.
            if (FallDownUsedCollision) _Body.MovePosition(startPos + (transform.up * height));
            else _Body.position = startPos + (transform.up * height);

            _Body.MoveRotation(Quaternion.Euler(fallDownDir2 * fallDownRot));
            yield return waitTime;
        }

        //collision�� ������̰�, �ݵ��� 0�̸� Ż��.
        if(rebound<=0f && FallDownUsedCollision)
        {
            _Body.Sleep();
            _Body.isKinematic = true;
            yield break;
        }


        /****************************************
         * ������ ������ �� �ݵ� ����.
         ***/
        _Body.isKinematic = true;
        _Body.Sleep();
        fallDownSpeed = 0f;
        fallDownRot = FallDownTargetAngle;

        while (rebound>0 || fallDownRot<= FallDownTargetAngle)
        {
            //���� ������ ƨ�ܳ���.
            if (rebound > 0 && fallDownRot >= FallDownTargetAngle) {
                fallDownSpeed = -rebound;
                fallDownRot = FallDownTargetAngle;
                rebound -= maxRebound * ReboundValue;
            }

            fallDownSpeed += FallDownSpeed * Time.deltaTime;
            fallDownRot += fallDownSpeed;

            transform.rotation = Quaternion.Euler(fallDownDir2 * fallDownRot);
            yield return waitTime;
        }

        #endregion
    }

    private bool TriggerTestPlayer()
    {
        //Player Range
        CharacterController con = Player.Instance.controller;
        Vector3 pCenter = con.transform.position + con.center;
        float pHeight = con.height;
        float pRadius = con.radius;
        float pminX = pCenter.x - pRadius;
        float pmaxX = pCenter.x + pRadius;
        float pminY = pCenter.y - pHeight * .5f;
        float pmaxY = pCenter.y + pHeight * .5f;
        float pminZ = pCenter.z - pRadius;
        float pmaxZ = pCenter.z + pRadius;

        //Trigger Range
        Vector3 rangePos = (TreePosition.position + Range.offset);
        float minX = rangePos.x - Range.Scale.x * .5f;
        float maxX = rangePos.x + Range.Scale.x * .5f;
        float minY = rangePos.y - Range.Scale.y * .5f;
        float maxY = rangePos.y + Range.Scale.y * .5f;
        float minZ = rangePos.z - Range.Scale.z * .5f;
        float maxZ = rangePos.z + Range.Scale.z * .5f;

        //Check overlap
        bool xOvrlap = (pminX <= maxX && pmaxX >= minX);
        bool yOverlap = (pminY <= maxY && pmaxY >= minY);
        bool ZOverlap = (pminZ <= maxZ && pmaxZ >= minZ);

        return xOvrlap && yOverlap && ZOverlap;
    }



}
