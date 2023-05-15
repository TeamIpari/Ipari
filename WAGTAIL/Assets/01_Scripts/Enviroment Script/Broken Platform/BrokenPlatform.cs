using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    private MeshRenderer _mesh;
    private Collider _col;
    bool ishit = false;
    public bool isUpdownMode = false;

    public float _hide_n_DownTime = 1.0f;
    public float _show_n_UpTime = 1.0f;

    // 위 아래 최종 이동 위치
    public float downPos = 0.0f;
    public float upPos = 0.0f;
    public float moveSpeed = 0.0f;

    

    Vector3 _localPoint1;
    Vector3 _localPoint2;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool _hit { get { return ishit; } set { ishit = value; } }

    public bool Interact()
    {
        ishit = true;
        if (!isUpdownMode)
            StartCoroutine(hidePlatform());
        else
            StartCoroutine(DownPlatform());

        return false;
    }

    private void OnDrawGizmos()
    {
        if (isUpdownMode)
        {
            Gizmos.color = Color.green;
            _localPoint1 = transform.position + new Vector3(0f, downPos, 0f);
            _localPoint2 = transform.position + new Vector3(0f, upPos, 0f);
            Gizmos.DrawWireCube(_localPoint1 + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
            Gizmos.DrawWireCube(_localPoint2 + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }
    }

    IEnumerator DownPlatform()
    {
        _mesh.material.color = Color.red;
        yield return new WaitForSeconds(_hide_n_DownTime);

        while(Mathf.Abs(Vector3.Distance(transform.position, _localPoint1)) > 0.1f )
        {
            transform.position = Vector3.MoveTowards(transform.position, _localPoint1, moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        StartCoroutine(UpPlatform());
    }

    IEnumerator UpPlatform()
    {
        _mesh.material.color = Color.gray;
        yield return new WaitForSeconds(_show_n_UpTime);

        while (Mathf.Abs(Vector3.Distance(transform.position, _localPoint2)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _localPoint2, moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        ishit = false;

    }

    IEnumerator hidePlatform()
    {
        _mesh.material.color = Color.red;
        yield return new WaitForSeconds(_hide_n_DownTime);

        _col.enabled = false;
        _mesh.enabled = false;

        StartCoroutine(showPlatform());
    }

    IEnumerator showPlatform()
    {
        _mesh.material.color = Color.gray;
        yield return new WaitForSeconds(_show_n_UpTime);
        _col.enabled = true;
        _mesh .enabled = true;
        ishit = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
