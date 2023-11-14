using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AcidBomb : Bullet
{
    private Vector3 direction;
    private GameObject bombMarker;
    [SerializeField] private LayerMask passedMask;

    //======================================
    /////          magic Methods        ////
    //======================================


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

    private void Start()
    {
        // n�� ��
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!DirectionLine)
        {
            BulletRigidBody.velocity = direction;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����ȣ�� �߰���
        if (other.CompareTag("Player"))
        {
            Debug.Log("����");
            FModAudioManager.PlayOneShotSFX(
                FModSFXEventType.Player_Hit,
                FModLocalParamType.PlayerHitType,
                FModParamLabel.PlayerHitType.MiniNepenthes_Attack,
                other.transform.position,
                10f
            );
            other.GetComponent<Player>().isDead = true;
        }
        // ==========================================================
        if (other.CompareTag("Platform"))
        {
            other.GetComponent<IEnviroment>().ExecutionFunction(0.5f);
        }
        BulletHit(other.transform);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        FModAudioManager.PlayOneShotSFX(FModSFXEventType.BossNepen_AcidBoom);
        BulletHit(collision.transform);
        Destroy(this.gameObject);
        // ����ȣ�� �߰���
        if (collision.collider.CompareTag("Player"))
        {
            FModAudioManager.PlayOneShotSFX(
                FModSFXEventType.Player_Hit,
                FModLocalParamType.PlayerHitType,
                FModParamLabel.PlayerHitType.MiniNepenthes_Attack,
                collision.transform.position
            );
            collision.collider.GetComponent<Player>().isDead = true;
        }
        // ==========================================================
        // �μ����°� �ذ��Ϸ��� ����.
        if (collision.collider.CompareTag("Platform"))
        {
            DestroyPlatform();
            //collision.collider.GetComponent<IEnviroment>().ExecutionFunction(0.0f);
        }
    }

    //=======================================
    /////          core Method            ////
    //=======================================
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
        DirectionLine = false;
        direction = vector3;
    }
    void BulletHit(Transform target)
    {
        // ���� ���� ���ϱ�
        //Vector3 bombPos = target.position - transform.position;
        //float distance = Vector3.Distance(target.position, transform.position);
        

        GameObject hitFX = GameObject.Instantiate(HitFX);

        hitFX.transform.position = transform.position/* + bombPos.normalized*/;
        Destroy(hitFX, 2f);
    }

    void DestroyPlatform()
    {
        // ��Ŀ�� �������� �� ������ ������Ʈ�� üũ.
        // ������ �������.
        #region Omit
        int mask = (1 << 1) | ( 1 << 2) | (1 << 3)  | (1 << 4) | (1 <<5)| (1 << 6) | /*(1<<7) |*/(1<<8)  | ( 1 << 9) | (1<< 10) | (1 << 11) | (1<<12) | (1 <<14) | (1<<15) | (1 << 16);
        #endregion
            Collider[] cols = 
            Physics.OverlapSphere(
                bombMarker.transform.localPosition, 
                this.transform.localScale.x-0.5f);
            foreach (var c in cols)
            {
                try
                {
                    if (c.CompareTag("Platform"))
                        c.GetComponent<IEnviroment>().ExecutionFunction(0.5f);
                    else if (c.CompareTag("Player"))
                        c.GetComponent<Player>().isDead = true;

                }
                catch
                {
                    ;
                }
            }
    }

    public override void SetMarker(GameObject marker)
    {
        this.bombMarker = marker;
    }
}
