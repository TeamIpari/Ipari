using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public MeshRenderer meshData;



    // Start is called before the first frame update
    void Start()
    {
        if(meshData != null)
            meshData.enabled = false;

    }

    // ��� ������ �� ���ΰ�? 
    // 1. ù��° ���� ������� üũ ���� ������ Ÿ���� ������ meshRenderer�� ��.
    // 2. �ι�° ���� ������� üũ ���� ������ Ÿ���� ������ meshRenderer�� �� ī�޶� ������.
    
    public void ShowMeshData()
    {
        meshData.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


}
