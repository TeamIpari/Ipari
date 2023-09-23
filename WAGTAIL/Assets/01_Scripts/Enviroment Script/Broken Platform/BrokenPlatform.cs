using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    private MeshRenderer mesh;
    private Collider col;
    public GameObject Light;
    [SerializeField] private GameObject _interactionVFX;
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

    [SerializeField] private const float _explosionMinForce = 5;
    [SerializeField] private const float _explosionMaxForce = 100;
    [SerializeField] private const float _explosionForceRadius = 10;
    [SerializeField] private const float _fragScaleFactor = 0.1f;

    float curTime;
    public float shakeDelay;
    private Vector3[] PosList;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        if (BossRoomFieldManager.Instance != null)
        {
            BossRoomFieldManager.Instance.PlayerOnTilePos = this.transform.parent.localPosition;
        }
        //if (this.enabled)
        //    ExecutionFunction(HideNDownTime);
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
        shake = false;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            var rigidbody = this.transform.GetChild(i).GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = true;
                rigidbody.AddExplosionForce(Random.Range(_explosionMinForce, _explosionMaxForce),
                    transform.parent.position, _explosionForceRadius);
            }
        }
        if (col != null)
            col.enabled = false;
        if (mesh != null)
            mesh.enabled = false;

        if (!callBack)
            StartCoroutine(ShowPlatform());
    }

    private IEnumerator ShowPlatform()
    {
        if (Light != null)
            Light.SetActive(false);

        yield return new WaitForSeconds(ShowNUpTime);


        for (int i = 0; i < transform.childCount; i++)
        {
            var piece = transform.GetChild(i);
            if (piece.GetComponent<Rigidbody>() != null)
            {
                piece.GetComponent<Rigidbody>().useGravity = false;
                piece.GetComponent<Rigidbody>().velocity = Vector3.zero;
                piece.position = PosList[i];
            }
        }

        //transform.position = startPos;
        if (col != null)
            col.enabled = true;
        if (mesh != null)
            mesh.enabled = true;
        IsHit = false;
    }

    public void HideOnly(bool isUpdown)
    {
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


    // Start is called before the first frame update
    private void Start()
    {
        this.tag = "Platform";
        startPos = transform.position;
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        PosList = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            var piece = transform.GetChild(i);
            PosList[i] = piece.position;
        }
    }

    public void ExecutionFunction(float time)
    {
        if (IsHit)
            return;
        IsHit = true;
        SpawnVFX();
        //if (Light != null)
        //    Light.SetActive(true);
        //delayTime = time;
        //if (!IsUpdownMode)
        //{
        //    StartCoroutine(HidePlatform());
        //}
        //else
        //{
        //    StartCoroutine(DownPlatform());
        //}
    }
    private void SpawnVFX()
    {
        if (_interactionVFX != null)
        {
            GameObject exploVFX = Instantiate(_interactionVFX, gameObject.transform.position + Vector3.up * 0f, gameObject.transform.rotation);
            Destroy(exploVFX, 2);
        }

        else
            Debug.LogWarning("InteractionVFX was missing!");
    }

    // Update is called once per frame
    private void Update()
    {
        if(shake)
        {
            curTime += Time.deltaTime;
            if(curTime > shakeDelay)
            {
                Vector3 pos = startPos + (UnityEngine.Random.insideUnitSphere * ShakeSpeed);
                transform.position = new Vector3(pos.x, transform.position.y, pos.z);
                curTime = 0;
            }
        }
    }
}
