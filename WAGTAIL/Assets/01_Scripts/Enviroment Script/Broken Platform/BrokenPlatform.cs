using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 작성자 : 성지훈
/// 추가 작성
/// </summary>
public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    private MeshRenderer mesh;
    private Collider col;
    public bool IsUpdownMode = false;

    public float HideNDownTime = 1.0f;
    public float ShowNUpTime = 1.0f;

    // 위 아래 최종 이동 위치
    //public float downPos = 0.0f;
    //public float upPos = 0.0f;
    public float MoveSpeed = 0.0f;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;


    Vector3 localPoint1;
    Vector3 localPoint2;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool IsHit { get; set; }

    public bool Interact()
    {
        IsHit = true;
        if (!IsUpdownMode)
            StartCoroutine(HidePlatform());
        else
            StartCoroutine(DownPlatform());

        return false;
    }

    private void OnDrawGizmos()
    {
        if (IsUpdownMode)
        {
            if (startPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(startPoint.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
            }

            if (endPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(endPoint.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
            }
        }
    }

    private IEnumerator DownPlatform(bool callBack = false)
    {
        mesh.material.color = Color.red;
        yield return new WaitForSeconds(HideNDownTime);

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
        mesh.material.color = Color.gray;
        yield return new WaitForSeconds(ShowNUpTime);

        while (Mathf.Abs(Vector3.Distance(transform.position, startPoint.transform.position)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, MoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        IsHit = false;

    }

    private IEnumerator HidePlatform(bool callBack = false)
    {
        mesh.material.color = Color.red;
        yield return new WaitForSeconds(HideNDownTime);

        col.enabled = false;
        mesh.enabled = false;

        if (!callBack)
            StartCoroutine(ShowPlatform());
    }

    private IEnumerator ShowPlatform()
    {
        mesh.material.color = Color.gray;
        yield return new WaitForSeconds(ShowNUpTime);
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
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    //// Update is called once per frame
    //private void Update()
    //{
        
    //}


}
