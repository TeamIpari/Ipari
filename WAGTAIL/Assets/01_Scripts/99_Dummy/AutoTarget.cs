using System.Collections.Generic;
using UnityEngine;

public class AutoTarget : MonoBehaviour     // �̸��� ������ �����̹� �ϴ°ɷ�
{
    /////////////////////////////////////////////////
    /////               Properties              /////
    /////////////////////////////////////////////////
    [SerializeField] public GameObject curTarget;
    [SerializeField] private List<Vector3> lPoints = new List<Vector3>();
    [SerializeField] private SphereCollider hitCollision;
    private Queue<Vector3> qPoints = new Queue<Vector3>();

    /////////////////////////////////////////////////
    /////           Magic Method                /////
    /////////////////////////////////////////////////

    private void Awake()
    {
        hitCollision = GetComponent<SphereCollider>();
        if (hitCollision == null)
        {
            hitCollision = gameObject.AddComponent<SphereCollider>();
            hitCollision.isTrigger = true;
        }
        for (int i = 0; i < lPoints.Count; i++)
        {
            qPoints.Enqueue(lPoints[i]);
        }


    }

    private void Start()
    {
        Vector3 curPos = qPoints.Dequeue();
        // Ÿ�� ����
        curTarget = GameObject.Instantiate<GameObject>(new GameObject(), this.transform.position + curPos, Quaternion.identity, this.transform);
        curTarget.layer = LayerMask.NameToLayer("Enemies");
        SphereCollider a = curTarget.AddComponent<SphereCollider>();
        a.isTrigger = true;
        hitCollision.center = curPos;
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    /////////////////////////////////////////////////
    /////            Core Method                /////
    /////////////////////////////////////////////////


}
