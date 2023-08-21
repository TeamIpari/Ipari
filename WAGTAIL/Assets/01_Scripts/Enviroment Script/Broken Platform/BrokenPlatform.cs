using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    private MeshRenderer mesh;
    private Collider col;
    public GameObject Light;
    public bool IsUpdownMode = false;

    public float HideNDownTime = 1.0f;
    public float ShowNUpTime = 1.0f;
    public float VineHitTime = 2.5f;

    private float DelayTime;
    // 위 아래 최종 이동 위치
    public float MoveSpeed = 0.0f;
    bool shake = false;
    
    
    public float ShakeSpeed = 0.0f;

    Vector3 startPos;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    float curTime;
    public float shakeDelay;

    Vector3 localPoint1;
    Vector3 localPoint2;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        if (BossRoomFildManager.Instance != null)
        {
            BossRoomFildManager.Instance.PlayerOnTilePos = this.transform.parent.localPosition;
        }
        if(this.enabled)
            ExecutionFunction(HideNDownTime);
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
        Light.SetActive(true);

        shake = true;
        yield return new WaitForSeconds(DelayTime);
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
        Light.SetActive(false);

        yield return new WaitForSeconds(DelayTime);

        while (Mathf.Abs(Vector3.Distance(transform.position, startPoint.transform.position)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, MoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        IsHit = false;

    }

    private IEnumerator HidePlatform(bool callBack = false)
    {
        Light.SetActive(true);

        shake = true;
        yield return new WaitForSeconds(DelayTime);
        shake = false;

        col.enabled = false;
        mesh.enabled = false;

        if (!callBack)
            StartCoroutine(ShowPlatform());
    }

    private IEnumerator ShowPlatform()
    {
        Light.SetActive(false);

        yield return new WaitForSeconds(ShowNUpTime);

        transform.position = startPos;
        col.enabled = true;
        mesh .enabled = true;
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
        startPos = transform.position;
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    public void ExecutionFunction(float time)
    {
        if (IsHit)
            return;
        IsHit = true;
        Light.SetActive(true);
        DelayTime = time;
        if (!IsUpdownMode)
        {
            StartCoroutine(HidePlatform());
        }
        else
        {
            StartCoroutine(DownPlatform());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(shake)
        {
            curTime += Time.deltaTime;
            if(curTime > shakeDelay)
            {
                transform.position = startPos + (UnityEngine.Random.insideUnitSphere * ShakeSpeed);
                curTime = 0;
            }
        }
    }
}
