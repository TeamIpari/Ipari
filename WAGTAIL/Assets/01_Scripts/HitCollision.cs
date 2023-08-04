using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitCollision : MonoBehaviour
{
    public Transform Door_1;
    public Transform Door_2;
    public float Distance = 10.00f;

    [Header("문이 열리는 속도")]
    public float OpenSpeed = 1.0f;
    [Header("문 열린 후 작동하는 시간.")]
    public float DelayTime = 1.0f;

    [SerializeField]
    private bool IsOpen = false;

    private void Start()
    {
        IsOpen = false;
    }

    private void Update()
    {
        if(IsOpen && Vector3.Distance(Door_1.position, Door_2.position) <= Distance)
        {
            Door_1.Translate(Vector3.left * Time.deltaTime * OpenSpeed);
            Door_2.Translate(Vector3.right * Time.deltaTime * OpenSpeed);
        }
    }

    private void OpenDoor()
    {
        StartCoroutine(DummyCo());
    }

    private IEnumerator DummyCo()
    {
        yield return null;
        Door_1.position = new Vector3(Door_1.position.x - .15f, Door_1.position.y, Door_1.position.z);
        Door_2.position = new Vector3(Door_2.position.x + .15f, Door_2.position.y, Door_2.position.z);

        yield return new WaitForSeconds(DelayTime);
        IsOpen = true;

        yield return null;


    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            OpenDoor();
        }
    }
}
