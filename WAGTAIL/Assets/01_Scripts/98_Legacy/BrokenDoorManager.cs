using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenDoorManager : MonoBehaviour, IInteractable
{
    // 이 클래스 ShatterObject로 대체됨 반드시 삭제해야됨
    [SerializeField] float destroyTime;
    [SerializeField] float force;

    
    GameObject player;
    float time;
    Transform[] allChildren;
    bool isBoom;



    [SerializeField] private string _promt;
    public string InteractionPrompt => _promt;

    void Start()
    {
        player = Player.Instance.gameObject;
        allChildren = GetComponentsInChildren<Transform>();
        isBoom = false;
    }

    void Update()
    {
        if (isBoom)
        {
            time += Time.deltaTime;
            if (time >= destroyTime)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public bool Interact(GameObject interactor)
    {
        Boom();

        return true;
    }

    public void Boom()
    {
        foreach (Transform child in allChildren)
        {
            // 자기 자신은 무시
            if (child.name != transform.name)
            {
                child.GetComponent<Rigidbody>().isKinematic = false;
                child.GetComponent<BrokenDoorController>().Throw(destroyTime, force, player);
            }
        }
        isBoom = true;
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}