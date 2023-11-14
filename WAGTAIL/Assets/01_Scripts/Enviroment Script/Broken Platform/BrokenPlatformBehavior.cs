using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.ProBuilder.Shapes;

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
    [HideInInspector] public bool isBroken;
    [HideInInspector] public Vector3[] InitPos;
    [HideInInspector] public Vector3[] EulerRotate;
    [Header("���������� ������ �� �� �������� ���� �ð�")]
    public float pieceDownDelay = 0.0f;
    [Header("���� �ı��ǰ� ��������� �ɸ��� �ð�")]
    public float spawnDelay = 0.0f;
    public AnimationCurve curve;


    //=============================================
    /////           Private Fields          //////
    //============================================
    private MeshRenderer mesh;
    private Collider col;
    private GameObject curBrokenPlatform;
    private BossNepenthes bossNepenthes;




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
        bossNepenthes = GameObject.Find("NewBoss").GetComponent<BossNepenthes>();
        spawnDelay = spawnDelay == 0.0f ? 1.5f : spawnDelay;
        pieceDownDelay = pieceDownDelay == 0.0f ? 0.25f : pieceDownDelay;
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
        if (!isBroken)
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
        EulerRotate = new Vector3[curPlatform.transform.childCount];
        for (int i = 0; i < curPlatform.transform.childCount; i++)
        {
            var piece = curPlatform.transform.GetChild(i);
            InitPos[i] = piece.position;
            EulerRotate[i] = piece.eulerAngles;
        }
        curBrokenPlatform.SetActive(true);
    }

    public IEnumerator SmallHidePiece(GameObject piece)
    { 
        yield return null;
        float time = 0;
        float lerpRatio = 0;
        //Vector3  = 
        while (piece.transform.localScale.x >= 0.1f)
        {
            time += Time.deltaTime;
            lerpRatio = time / spawnDelay;

            piece.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero , curve.Evaluate(lerpRatio));
            yield return new WaitForSeconds(0.001f);
        }
        piece.transform.localScale = Vector3.one;
    }

    public IEnumerator SpawnPlatform()
    {
        isBroken = true;
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
                //Vector3 v = new Vector3(0, 0, 0);
                //rigidbody.transform.rotation = Quaternion.Euler(v);

                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                // ������ �ٵ� ���ν�Ƽ�� �ΰ���.
                rigidbody.velocity = ExplosionVelocity(rigidbody);
                rigidbody.gameObject.layer = LayerMask.NameToLayer("Pass");
                StartCoroutine(SmallHidePiece(rigidbody.gameObject));
            }
            yield return new WaitForSeconds(pieceDownDelay);
        }
        yield return new WaitForSeconds(spawnDelay);
        if (bossNepenthes.AiSM.CurrentState != bossNepenthes.AiDie)
        {
            //Vector3 v = new Vector3(0, 0, 0);

            for (int i = 0; i < curBrokenPlatform.transform.childCount; i++)
            {
                var piece = curBrokenPlatform.transform.GetChild(i);
                var rigid = piece.GetComponent<Rigidbody>();
                if (rigid != null)
                {
                    rigid.useGravity = false;
                    rigid.velocity = Vector3.zero;
                    rigid.isKinematic = true;

                    piece.rotation = Quaternion.Euler(EulerRotate[i]);
                    piece.position = InitPos[i];
                }
            }

            curBrokenPlatform.SetActive(false);
            if (col != null) col.enabled = true;
            if (mesh != null) mesh.enabled = true;
            isBroken = false;
        }

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
