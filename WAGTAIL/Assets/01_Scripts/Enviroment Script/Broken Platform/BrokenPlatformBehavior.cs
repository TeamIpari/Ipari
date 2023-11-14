using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class BrokenPlatformBehavior : PlatformBehaviorBase
{
    /**************************************************
     * Bomb �Ǵ� Bullet�� ���� �μ����� Platform Object.
     * */

    /**************************************************
     * �ʿ��� �������� ������ ������?
     * 1. Platform ���� �ð�.
     * 2. Platform�� Piece���� ������ �ִ� ������.
     * 3. Platform Piece�� ���� ��ġ. 
     * 4. Platform �������� �������� �����ϱ�.
     */

    //===========================================
    /////           Property                /////
    //===========================================

    public GameObject[] PlatformArray;
    public bool isBroken;
    [HideInInspector] public Vector3[] InitPos;



    //=============================================
    /////           Private Fields          //////
    //============================================
    private MeshRenderer mesh;
    private Collider col;
    private GameObject curBrokenPlatform;
    




    //=============================================
    /////        Override Methods             /////
    //=============================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        this.tag = "Platform";
        // Platform Piece�� �� �� �ִ��� �Ľ̺��� �������.
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        isBroken = false;
        // �ı��Ǵ� ����� ���� �ٸ����̼����� �ı��ǰ� ����
        for (int i = 0; i < PlatformArray.Length; i++)
        {
            PlatformArray[i].SetActive(false);
        }
    }

    public override void BehaviorEnd(PlatformObject changedTarget)
    {
        base.BehaviorEnd(changedTarget);
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        #region Omit       
        /******************************************
         * �⺻������ �����ΰ� ����� �� �����ϴ� ���� �ƴ� �ı��Ǿ��� �� ����.
         * */ 
        if (standingTarget != null) return;
        isBroken = true;
        StartCoroutine(SpawnPlatform());


        #endregion


    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody)
    {
        base.OnObjectPlatformExit(affectedPlatform, exitTarget, exitBody);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        base.OnObjectPlatformStay(affectedPlatform, standingTarget, standingBody, standingPoint, standingNormal);
    }

    //============================================
    /////          Core Methods              /////
    //============================================
    private void SetPieceOriginPos(GameObject curPlatform)
    {
        curBrokenPlatform = curPlatform;
        InitPos = new Vector3[curPlatform.transform.childCount];
        for (int i = 0; i < curPlatform.transform.childCount; i++)
        {
            var piece = curPlatform.transform.GetChild(i);
            InitPos[i] = piece.position;
        }
        curBrokenPlatform.SetActive(true);
    }

    public IEnumerator SmallHidePiece(GameObject Piece)
    { 
        yield return null;
        Debug.Log("AA");
        int count = 0;
        while (Piece.transform.localScale.x >= 0.1f)
        {
            Piece.transform.localScale = Vector3.Lerp(Piece.transform.localScale, Vector3.zero , Time.deltaTime);
            yield return new WaitForSeconds(0.025f);
        }
    }

    public IEnumerator SpawnPlatform()
    {

        // �Ž����� ���ֱ�.
        if (col != null) col.enabled = false;
        if (mesh != null) mesh.enabled = false;
        SetPieceOriginPos(PlatformArray[Random.Range(0, PlatformArray.Length)]);
        // ���� �÷����� �޾ƿͼ� Piece�� �Ҵ�ް� Piece�� ���߽��� ��濡 ������ �ϱ�.

        for (int i = 0; i < curBrokenPlatform.transform.childCount; i++)
        {
            var rigidbody = curBrokenPlatform.transform.GetChild(i).GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                // ������ �ٵ� ���ν�Ƽ�� �ΰ���.
                rigidbody.velocity = ExplosionVelocity(rigidbody);
                rigidbody.gameObject.layer = LayerMask.NameToLayer("Pass");
                StartCoroutine(SmallHidePiece(rigidbody.gameObject));
            }
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.5f);

        Vector3 v = new Vector3(-90, 0, 0);

        for (int i = 0; i < curBrokenPlatform.transform.childCount; i++)
        {
            var piece = curBrokenPlatform.transform.GetChild(i);
            var rigid = piece.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;
                rigid.isKinematic = true;

                piece.rotation = Quaternion.Euler(v);
                piece.position = InitPos[i];
                //piece.localScale = Vector3.one;
            }
        }
        
        curBrokenPlatform.SetActive(false);
        if (col != null) col.enabled = true;
        if (mesh != null) mesh.enabled = true;
        isBroken = false;
    }

    private Vector3 ExplosionVelocity(Rigidbody rigid)
    {
        // ���� �����ǿ��� y���� n��ŭ ������ ��ġ���� ���������� ���� ���͸� ����.
        Vector3 direction = rigid.gameObject.transform.position - (transform.position /*- Vector3.up * 1.5f*/);

        // ���� ���͸� ����.
        direction.Normalize();

        return direction * 1.5f;
    }

}
