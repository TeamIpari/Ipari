using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ۼ���: ������
/// �߰� �ۼ�
/// </summary>
public class basket : MonoBehaviour
{
    // ����
    public GameObject Lid;

    //[SerializeField] Vector3 _loopPoint1;
    //[SerializeField] Vector3 _loopPoint2;

    [SerializeField] private GameObject localPoint1;
    [SerializeField] private GameObject localPoint2;
    public Vector3 LoopTarget;

    [Range(1, 5)]
    public float HideTime = 1;

    private void OnDrawGizmos()
    {
        if (localPoint1 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(localPoint1.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

        if(localPoint2 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(localPoint2.transform.position + Vector3.up * GetComponent<BoxCollider>().center.y, new Vector3(1, 1, 1));
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        Lid.SetActive(false);
        LoopTarget = localPoint1.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if( Vector3.Distance(transform.position, LoopTarget) <= 1)
            // �ʴ� n�� �ӵ��� ��ǥ�� ���� ������.
            LoopTarget = LoopTarget == localPoint1.transform.position ? localPoint2.transform.position : localPoint1.transform.position;
        else
            transform.position = Vector3.MoveTowards(transform.position, LoopTarget, 5f * Time.deltaTime);
    }

    private IEnumerator ShowLid()
    {
        yield return new WaitForSeconds(1.0f);
        Lid.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        Lid.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag( "interactable"))
        {
            StartCoroutine(ShowLid());  
        }
    }
}
