using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour     // 이름은 다음에 리네이밍 하는걸로
{
    /////////////////////////////////////////////////
    /////               Properties              /////
    /////////////////////////////////////////////////
    [SerializeField] public GameObject curTarget;
    [SerializeField] private List<Vector3> lPoints = new List<Vector3>();
    [SerializeField] private BoxCollider hitCollision;
    private Queue<Vector3> qPoints = new Queue<Vector3>();

    /////////////////////////////////////////////////
    /////           Magic Method                /////
    /////////////////////////////////////////////////

    private void Awake()
    {
        hitCollision = GetComponent<BoxCollider>();
        if (hitCollision == null)
        {
            hitCollision = gameObject.AddComponent<BoxCollider>();
            hitCollision.isTrigger = true;
            hitCollision.size = new Vector3(1, 3, 1);
        }
        for (int i = 0; i < lPoints.Count; i++)
        {
            qPoints.Enqueue(lPoints[i]);
        }


    }

    private void Start()
    {
        ChangeTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name}");
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            ChangeTarget();
            other.attachedRigidbody.velocity = new Vector3(0, other.attachedRigidbody.velocity.y, 0);

        }

    }

    /////////////////////////////////////////////////
    /////            Core Method                /////
    /////////////////////////////////////////////////

    private void ChangeTarget()
    {
        if(qPoints.Count > 0)
        {
            Vector3 curPos = qPoints.Dequeue();
            // 타겟 설정
            Debug.Log($"{curPos}");
            if (curTarget == null)
            {
                curTarget = GameObject.Instantiate<GameObject>(new GameObject(), this.transform.position + curPos, Quaternion.identity, this.transform);
                curTarget.layer = LayerMask.NameToLayer("Enemies");
            }
            else
            {
                curTarget.transform.position = transform.position + curPos;
            }
            SphereCollider a = curTarget.AddComponent<SphereCollider>();
            a.isTrigger = true;
            hitCollision.center = curPos;
        }
    }

}
