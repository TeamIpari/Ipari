using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public MeshRenderer meshData;

    private float targetTime = 1.2f;
    public float curTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        //if(meshData != null)
        //    meshData.enabled = false;

        ObjectCheckManager.Instance.AddTiles(this.gameObject);
        curTime = 0;

    }

    // ��� ������ �� ���ΰ�? 
    // 1. ù��° ���� ������� üũ ���� ������ Ÿ���� ������ meshRenderer�� ��.
    // 2. �ι�° ���� ������� üũ ���� ������ Ÿ���� ������ meshRenderer�� �� ī�޶� ������.
    
    public void GetPing()
    {
        curTime = 0;

        //meshData.enabled = true;
    }




    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if(curTime >= targetTime)
        {
            this.gameObject.SetActive(false);
        }
        
    }


}
