using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    // 
    private MeshRenderer mesh;
    private Collider col;
    public GameObject BasePlatform;
    public GameObject PlatformPiece; 
    public GameObject Light;
    public bool IsUpdownMode = false;

    public const float HideNDownTime = 1.0f;
    public const float ShowNUpTime = 2.0f;
    public const float VineHitTime = 2.5f;

    private float delayTime;
    // 위 아래 최종 이동 위치
    public const float MoveSpeed = 10f;
    private bool shake = false;
    
    
    public float ShakeSpeed = 0.1f;

    // 흔들리고 사라지거나 떨어질 때 원래 위치로 찾아오기 위한 Origin Position;
    private Vector3 startPos;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [SerializeField] private const float _explosionMinForce = 50;
    [SerializeField] private const float _explosionMaxForce = 100;
    [SerializeField] private const float _explosionForceRadius = 10;
    [SerializeField] private const float _fragScaleFactor = 0.1f;

    private float curTime;
    private float shakeDelay;
    private Vector3[] PosList;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    //===============================================
    /////           Magic Methods               /////
    //===============================================
    public bool Interact()
    {
        if (BossRoomFieldManager.Instance != null)
        {
            BossRoomFieldManager.Instance.PlayerOnTilePos = transform.localPosition;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        //if (IsUpdownMode)
        //{
        //    if (startPoint != null)
        //    {
        //        Gizmos.color = Color.red;
        //        Gizmos.DrawWireCube(startPoint.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        //    }

        //    if (endPoint != null)
        //    {
        //        Gizmos.color = Color.blue;
        //        Gizmos.DrawWireCube(endPoint.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        //    }
        //}
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.tag = "Platform";
        startPos = transform.position;
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        PosList = new Vector3[PlatformPiece.transform.childCount];
        for (int i = 0; i < PlatformPiece.transform.childCount; i++)
        {
            var piece = PlatformPiece.transform.GetChild(i);
            PosList[i] = piece.position;
        }
        PlatformPiece.SetActive(false);    
    }

    public void ExecutionFunction(float time)
    {
        if (IsHit)
            return;
        IsHit = true;
        if (Light != null)
            Light.SetActive(true);
        delayTime = time;
        if (!IsUpdownMode)
            StartCoroutine(HidePlatform());
        else
            StartCoroutine(DownPlatform());
    }

    // Update is called once per frame
    private void Update()
    {
        //if (shake)
        //{
        //    curTime += Time.deltaTime;
        //    if (curTime > shakeDelay)
        //    {
        //        Vector3 pos = startPos + (UnityEngine.Random.insideUnitSphere * ShakeSpeed);
        //        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        //        curTime = 0;
        //    }
        //}
    }

    //===============================================
    /////           Core Methods               /////
    //===============================================


    private IEnumerator DownPlatform(bool callBack = false)
    {
        if (Light != null)
            Light.SetActive(true);

        shake = true;
        yield return new WaitForSeconds(delayTime);
        shake = false;

        while(Mathf.Abs(Vector3.Distance(transform.position, endPoint.transform.position)) > 0.1f )
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.transform.position, MoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        if(!callBack)
            StartCoroutine(UpPlatform());
    }

    private IEnumerator UpPlatform()
    {
        if (Light != null)
            Light.SetActive(false);

        yield return new WaitForSeconds(delayTime);

        while (Mathf.Abs(Vector3.Distance(transform.position, startPoint.transform.position)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, MoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        IsHit = false;
    }


    // 부숴주는 기능을 추가 예정
    private IEnumerator HidePlatform(bool callBack = false)
    {
        if(Light != null)
            Light.SetActive(true);

        shake = true;
        yield return new WaitForSeconds(delayTime);
        BasePlatform.SetActive(false);
        PlatformPiece.SetActive(true);
        shake = false;

        for (int i = 0; i < PlatformPiece.transform.childCount; i++)
        {
            var rigidbody = PlatformPiece.transform.GetChild(i).GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                // 리지드 바디에 벨로시티를 부가함.
                rigidbody.velocity = ExplosionVelocity(rigidbody);
                rigidbody.gameObject.layer = LayerMask.NameToLayer("Pass");
            }
        }
        if (col != null) col.enabled = false;
        if (mesh != null) mesh.enabled = false;

        if (!callBack) StartCoroutine(ShowPlatform());
    }

    private Vector3 ExplosionVelocity(Rigidbody rigid)
    {
        // 현재 포지션에서 y값이 n만큼 내려간 위치에서 조각마다의 방향 벡터를 구함.
        Vector3 direction = rigid.gameObject.transform.position - (transform.position /*- Vector3.up * 1.5f*/);

        // 방향 벡터를 구함.
        direction.Normalize();

        return direction * 1.5f;
    }

    private IEnumerator ShowPlatform()
    {
        if (Light != null) Light.SetActive(false);

        yield return new WaitForSeconds(ShowNUpTime);

        Vector3 v = new Vector3(-90, 0, 0);

        for (int i = 0; i < PlatformPiece.transform.childCount; i++)
        {
            var piece = PlatformPiece.transform.GetChild(i);
            if (piece.GetComponent<Rigidbody>() != null)
            {
                piece.GetComponent<Rigidbody>().useGravity = false;
                piece.GetComponent<Rigidbody>().velocity = Vector3.zero;
                piece.GetComponent<Rigidbody>().isKinematic = true;
                piece.gameObject.layer = LayerMask.NameToLayer("Defualt");

                piece.rotation = Quaternion.Euler(v);
                piece.position = PosList[i];
            }
        }
        if (col != null) col.enabled = true;
        if (mesh != null) mesh.enabled = true;
        PlatformPiece.SetActive(false);
        BasePlatform.SetActive(true);

        IsHit = false;
    }

    public void HideOnly(bool isUpdown)
    {
        // 사라지게 하기.
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        if (!IsUpdownMode)
            StartCoroutine(HidePlatform(isUpdown));
        else 
        { 
            if(startPoint == null)
            {
                startPoint = transform.parent.GetChild(1);
                endPoint = transform.parent.GetChild(2);
            }
            StartCoroutine(DownPlatform(isUpdown));
        }
    }


}
