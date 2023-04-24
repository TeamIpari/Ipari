using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BrokenPlatform : MonoBehaviour, IEnviroment
{
    private MeshRenderer _mesh;
    private Collider _col;
    bool ishit = false;

    public float _hideTime = 1.0f;
    public float _showTime = 1.0f;

    public string EnviromentPrompt => throw new System.NotImplementedException();

    public bool _hit { get { return ishit; } set { ishit = value; } }

    public bool Interact()
    {
        ishit = true;
        StartCoroutine(hidePlatform());

        return false;
    }

    IEnumerator hidePlatform()
    {
        yield return new WaitForSeconds(_hideTime);

        _col.enabled = false;
        _mesh.enabled = false;

        StartCoroutine(showPlatform());
    }

    IEnumerator showPlatform()
    {
        yield return new WaitForSeconds(_showTime);
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
