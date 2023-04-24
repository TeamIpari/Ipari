using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Debug�� ������
    [SerializeField] public bool isChange;
    [SerializeField] public bool isBack;

    // CM vcam�� linkedlist�� ����
    [SerializeField] private GameObject cameraGroup;
    public LinkedList<CinemachineVirtualCamera> list;
    private LinkedList<CinemachineVirtualCamera> _currentList;
    
    private Transform _player;
    
    // Start is called before the first frame update
    private void Awake()
    {
        // Cinemachine Camera���� LinkedList�� ����
        var cameraList = cameraGroup.GetComponentsInChildren<CinemachineVirtualCamera>();
        list = new LinkedList<CinemachineVirtualCamera>(cameraList);
        _currentList = new LinkedList<CinemachineVirtualCamera>();
        
        // Follow, LookAt�� Player.Transform�� �־����
        _player = Player.Instance.GetComponent<Transform>();
        
        isChange = false;
        isBack = false;
    }

    private void Update()
    {
        if (isChange)
        {
            ChangeCam();
        }
        
        else if (isBack)
        {
            BackCam();
        }
    }

    public void ChangeCam()
    {
        _currentList.AddFirst(list.First.Value);
        list.RemoveFirst();
        list.First.Value.m_Priority = _currentList.First.Value.Priority + 1;
        list.First.Value.Follow = _player;
        list.First.Value.LookAt = _player;
        isChange = false;
    }

    public void BackCam()
    {
        _currentList.First.Value.m_Priority = list.First.Value.Priority + 1;
        list.AddFirst(_currentList.First.Value);
        _currentList.RemoveFirst();
        isBack = false;
    }
}
