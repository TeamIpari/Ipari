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

    // 어떻게 구현을 할 것인가? 
    // 1. 첫번째 구로 만들어진 체크 범위 안으로 타일이 들어오면 meshRenderer를 끔.
    // 2. 두번째 구로 만들어진 체크 범위 안으로 타일이 들어오면 meshRenderer를 켜 카메라에 보여줌.
    
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
