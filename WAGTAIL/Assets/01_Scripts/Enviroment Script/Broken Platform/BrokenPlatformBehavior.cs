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
     * Bomb 또는 Bullet에 의해 부서지는 Platform Object.
     * */

    /**************************************************
     * 필요한 변수들이 무엇이 있을까?
     * 1. Platform 생성 시간.
     * 2. Platform의 Piece들을 가지고 있는 데이터.
     * 3. Platform Piece의 최초 위치. 
     * 4. Platform 랜덤으로 떨어지게 설정하기.
     */

    //===========================================
    /////           Property                /////
    //===========================================

    public GameObject[] PlatformArray;
    [HideInInspector] public bool isBroken;
    [HideInInspector] public Vector3[] InitPos;
    [HideInInspector] public Vector3[] EulerRotate;
    [Header("순차적으로 떨어질 때 각 조각들의 지연 시간")]
    public float pieceDownDelay = 0.0f;
    [Header("돌이 파괴되고 재생성까지 걸리는 시간")]
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
        // Platform Piece가 몇 개 있는지 파싱부터 해줘야함.
        mesh = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        isBroken = false;
        bossNepenthes = GameObject.Find("NewBoss").GetComponent<BossNepenthes>();
        spawnDelay = spawnDelay == 0.0f ? 1.5f : spawnDelay;
        pieceDownDelay = pieceDownDelay == 0.0f ? 0.25f : pieceDownDelay;
        // 파괴되는 방식이 여러 바리에이션으로 파괴되게 세팅
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
         * 기본적으로 무엇인가 밟았을 때 실행하는 것이 아닌 파괴되었을 때 실행.
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
        // 매쉬부터 꺼주기.
        if (col != null) col.enabled = false;
        if (mesh != null) mesh.enabled = false;
        SetPieceOriginPos(PlatformArray[Random.Range(0, PlatformArray.Length)]);
        // 랜덤 플랫폼을 받아와서 Piece를 할당받고 Piece를 폭발시켜 사방에 퍼지게 하기.

        for (int i = 0; i < curBrokenPlatform.transform.childCount; i++)
        {
            var rigidbody = curBrokenPlatform.transform.GetChild(i).GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                //Vector3 v = new Vector3(0, 0, 0);
                //rigidbody.transform.rotation = Quaternion.Euler(v);

                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                // 리지드 바디에 벨로시티를 부가함.
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
        // 현재 포지션에서 y값이 n만큼 내려간 위치에서 조각마다의 방향 벡터를 구함.
        Vector3 direction = rigid.gameObject.transform.position - (transform.position /*- Vector3.up * 1.5f*/);

        // 방향 벡터를 구함.
        direction.Normalize();

        return direction * 1.5f;
    }

}
