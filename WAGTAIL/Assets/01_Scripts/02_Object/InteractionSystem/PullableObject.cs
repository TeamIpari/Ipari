using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/***********************************************
 *   ����� �� �ִ� ������ �����ϴ� ������Ʈ�Դϴ�...
 * **/
public class PullableObject : MonoBehaviour
{
    [System.Serializable]
    public sealed class PullCompleteEvent : UnityEvent
    {
    }

    [System.Serializable]
    private struct BlendShapeData
    {
        public SkinnedMeshRenderer renderer;
        public int index;
    }

    //=====================================
    ////            Property           ////
    //=====================================
    public bool       PulledIsCompleted  { get; private set; }
    public float      PulledAmount       { get { return _pulledAmount; } }
    public GameObject CurrentPuller      { get { return _currentPuller.gameObject; } }

    [SerializeField] private BlendShapeData[]   BlendShapeDatas;
    [SerializeField] public Transform           HoldingPoint;
    [SerializeField] public Animator            Animator;
    [SerializeField] public Vector3             PullingDir;
    [SerializeField] public Transform           MaxLengthPoint;
    [SerializeField] public PullCompleteEvent   OnPullComplete;

    [Range(0f, 100f)]
    [SerializeField]
    public float RestoringForce = .5f;
    


    //=====================================
    ////            Fields             ////
    //=====================================
    private int       _blendShapeCount;
    private float     _pulledAmount;
    private Transform _currentPuller;
    private float     _progressDiv = (1f / 100f);
    private float     _limitTime = 0f;
    private float     _MaxLength;



    //====================================
    /////        Magic methods        ////
    ///===================================
    private void Start()
    {
        #region Omit
        /**************************************
         *  ��� ��ҵ��� �ʱ�ȭ�Ѵ�...
         * **/
        if (Animator == null){

            Animator = GetComponent<Animator>();
        }

        _blendShapeCount = (BlendShapeDatas==null? 0:BlendShapeDatas.Length);

        if (MaxLengthPoint == null) _MaxLength = 3f;
        else 
        {
           _MaxLength = (MaxLengthPoint.position - HoldingPoint.position).magnitude;
        }
        #endregion
    }

    private void OnDrawGizmos()
    {
        #region Omit
        if (HoldingPoint!=null){

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(HoldingPoint.position, .1f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(HoldingPoint.position, HoldingPoint.position + ( PullingDir * .5f ));
        }
        #endregion
    }

    private void Update()
    {
        #region Omit
        if (_currentPuller==null) return;

        float deltaTime         = Time.deltaTime;
        float progressRatio     = (_pulledAmount * _progressDiv);
        float addForce          = (progressRatio>=.6f? (RestoringForce * (progressRatio)) : 0f);
        float restoringForce    = .1f + (RestoringForce * (progressRatio)) + addForce;
        _pulledAmount           = Mathf.Clamp(_pulledAmount, 0f, 100f);
        progressRatio           = (_pulledAmount * _progressDiv);

        /**��������*/
        for(int i=0; i<_blendShapeCount; i++) {

            ref BlendShapeData data = ref BlendShapeDatas[i];
            data.renderer?.SetBlendShapeWeight(data.index, _pulledAmount);
        }

        if (progressRatio > .7f) _limitTime += deltaTime;
        else _limitTime = 0f;

        /**����� �ִ� ����� �ִٸ� ����..*/
        if (_currentPuller!=null){

            Vector3 defaultPos = HoldingPoint.position;
            float   offset     = (_MaxLength * progressRatio );

            Vector3 holdPos    = defaultPos + (PullingDir * offset);
            Vector3 moveOffset = ( holdPos -_currentPuller.transform.position );
            moveOffset.y = 0f;
            Player.Instance.controller.SimpleMove(moveOffset);
        }

        /**�Ѱ踦 �Ѿ��, �Ϸ� �̺�Ʈ�� �����Ѵ�..*/
        if( _limitTime>=2f )
        {
            OnPullComplete?.Invoke();
            PulledIsCompleted = true;
            UnHold();
        }
        #endregion
    }


    //====================================
    /////       Public methods        ////
    ///===================================
    public void AddBlendShapeData(SkinnedMeshRenderer renderer, int index)
    {

    }

    public void RemoveBlendShapeData(int index)
    {
        
    }

    public void Hold( GameObject target )
    {
        if (target == null) return;
        _currentPuller = target.transform;
        Animator.enabled = false;
    }

    public void UnHold()
    {
        _currentPuller = null;
        Animator.enabled = true;
    }

    public void Pull( float power, Vector3 dir )
    {
        _pulledAmount += power;
    }
}
