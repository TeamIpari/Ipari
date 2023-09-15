using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
#endif


/**********************************************
 *   ������ �������� ȿ���� ������ ������Ʈ�Դϴ�.
 * ***/
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Triggerable/TreeObstacles")]
public sealed class TreeObstacles : MonoBehaviour
{
    #region Editor_Extension
    /***************************************
     *   ������ Ȯ���� ���� private class...
     * ***/
#if UNITY_EDITOR
    [CustomEditor(typeof(TreeObstacles))]
    private sealed class TreeObstaclesEditor : Editor
    {
        //==================================
        /////       Property            ////
        //==================================
        private SerializedProperty FallDownDirProperty;
        private SerializedProperty TreeRootPointProperty;
        private SerializedProperty InitProperty;
        private Collider Collider;



        //===============================================
        /////      Override and Magic methods        ////
        //===============================================
        private void OnEnable()
        {
            GUI_Initialized();
        }

        private void OnSceneGUI()
        {
            #region Omit
            if (FallDownDirProperty == null || TreeRootPointProperty == null) return;
            serializedObject.Update();

            Vector3 center = Collider.bounds.center;
            if (InitProperty!=null && !InitProperty.boolValue)
            {
                Bounds bounds = Collider.bounds;
                FallDownDirProperty.vector3Value   = center + (Vector3.forward * 10f);
                TreeRootPointProperty.vector3Value = center + (Vector3.down * (bounds.extents.y));
                InitProperty.boolValue = true;
            }

            /**�Ѹ������� ���� ǥ��...*/
            using(var scope=new EditorGUI.ChangeCheckScope())
            {
                Vector3 point       = FallDownDirProperty.vector3Value;
                Vector3 treePos     = center;
                Vector3 dir         = (point - treePos);
                float dirLen        = dir.magnitude;
                Quaternion dirQuat  = Quaternion.LookRotation(dir);

                dir.Normalize();
                Handles.color = Color.yellow;
                Handles.ScaleSlider(.1f, treePos, dir, dirQuat, dirLen, .5f);
                point = Handles.PositionHandle(FallDownDirProperty.vector3Value, Quaternion.identity);

                /**���� �ٲ���ٸ� ����...*/
                if (scope.changed){
                    FallDownDirProperty.vector3Value = point;   
                }
            }

            /**�Ѹ������� ���� ǥ��...*/
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                Vector3 point = Handles.PositionHandle(TreeRootPointProperty.vector3Value, Quaternion.identity);
                Handles.color = Color.yellow;
                Handles.RadiusHandle(Quaternion.identity, point, .2f);

                /**���� �ٲ���ٸ� ����...*/
                if (scope.changed){

                    TreeRootPointProperty.vector3Value = point;
                }
            }

            /**���� �ٲ���ٸ� ����...*/
            if(GUI.changed){

                serializedObject.ApplyModifiedProperties();
            }
            #endregion
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }



        //=====================================
        /////         GUI methods          ////
        //=====================================
        private void GUI_Initialized()
        {
            #region Omit
            /************************************
             *   ��� ������Ƽ���� �ʱ�ȭ�Ѵ�...
             * ***/
            if(FallDownDirProperty==null){

                FallDownDirProperty = serializedObject.FindProperty("FallDownDir");
            }

            if(TreeRootPointProperty==null){

                TreeRootPointProperty = serializedObject.FindProperty("TreeRootPoint");
            }

            if(Collider==null){

                TreeObstacles tree = (target as TreeObstacles);
                if (tree != null) Collider = tree.GetComponent<Collider>();
            }

            if(InitProperty==null){

                InitProperty = serializedObject.FindProperty("_Init");
            }

            #endregion
        }


    }
#endif
    #endregion


    //=====================================
    ////      Property And Fields      ////
    //=====================================
    [SerializeField] public Vector3     FallDownDir   = Vector3.zero;
    [SerializeField] public Vector3     TreeRootPoint = Vector3.zero;
    [SerializeField] public float       FallDownSpeed = 1f;
    [SerializeField] public bool        FallDownUsedCollision = false;
    [SerializeField] public float       FallDownTargetAngle = 90f;

    [SerializeField] [HideInInspector]
    private bool       _Init = false;

    private const float ReboundValue = .325f;
    private Rigidbody   _Body;
    private Collider    _Collider;
    private bool        _IsSwitch = false;
    private Vector3     _TreePosition;

    private float fallDownSpeed = 0f;
    private float fallDownRot   = 0f;
    private float rebound       = .4f;
    private float maxRebound    = .4f;

    private List<Collider> _hitColliders= new List<Collider>();
    private Coroutine fallDownCoroutine;


    //=====================================
    ////         Magic Methods         ////
    //=====================================
    private void Start()
    {
        #region Omit
        /**Intialized*/
        _Body = GetComponent<Rigidbody>();
        _Body.useGravity = false;
        _Body.mass = 100f;
        _Body.isKinematic = true;
        _Body.Sleep();

        _Collider= GetComponent<Collider>();
        maxRebound = rebound;
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Omit
        if (other.gameObject.CompareTag("Platform"))
        {
            //have ShatterObject...
            ShatterObject shatter = other.gameObject.GetComponent<ShatterObject>();
            if(shatter!=null)
            {
                shatter.Explode();
                return;
            }

            //������ �浹�� ���ǵ��� �ı��Ѵ�.
            //TODO: �ı��� ���ǵ��� ����Ʈ��, �� ������ OnDestory()���� �����ϵ��� ������.
            Destroy(other.gameObject);
        }

        #endregion
    }

    private void OnCollisionExit(Collision collision)
    {
        #region Omit
        if (_hitColliders.Contains(collision.collider))
        {
            _hitColliders.Remove(collision.collider);

            /**������ �浹���� �̵��� ���ٰ�, ����� �ٽ� ����...*/
            if(_hitColliders.Count<=0 && rebound<=0f){

                rebound = maxRebound;
            }
        }
        #endregion
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region Omit
        _hitColliders.Add(collision.collider);

        if (collision.gameObject.CompareTag("Platform"))
        {
            //have ShatterObject...
            ShatterObject shatter = collision.gameObject.GetComponent<ShatterObject>();
            if (shatter != null)
            {
                shatter.Explode();
                return;
            }

            //������ �浹�� ���ǵ��� �ı��Ѵ�.
            //TODO: �ı��� ���ǵ��� ����Ʈ��, �� ������ OnDestory()���� �����ϵ��� ������.
            Destroy(collision.gameObject);
            return;
        }

        //���� ������ ƨ�ܳ���.
        if (rebound>=0)
        {
            //Tree Sound ���...
            FModAudioManager.PlayOneShotSFX(
                  FModSFXEventType.Tree_Obstacle,
                  FModLocalParamType.TreeActionType,
                  FModParamLabel.TreeActionType.TreeCrash,
                  transform.position,

                  -1,
                  -1
              );

            fallDownSpeed = -rebound;
            fallDownRot -= rebound;
            rebound -= maxRebound * ReboundValue;
            if (rebound < 0) rebound = 0;
        }
        #endregion
    }



    //=====================================
    ////         Core Methods          ////
    //=====================================
    public void StartFallDown()
    {
        #region Omit
        if (_IsSwitch) return;

        _Collider.isTrigger = !FallDownUsedCollision;
        _Body.isKinematic = false;
        _IsSwitch = true;
        _Body.WakeUp();
        fallDownCoroutine = StartCoroutine(FallDownProgress());

        //Tree Sound ���...
        FModAudioManager.PlayOneShotSFX(
              FModSFXEventType.Tree_Obstacle,
              FModLocalParamType.TreeActionType,
              FModParamLabel.TreeActionType.TreeFallDown,
              transform.position,

              -1,
              -1
          );
        #endregion
    }

    private IEnumerator FallDownProgress()
    {
        #region Omission

        /***************************************
         * ��꿡 �ʿ��� ��� �͵��� ���Ѵ�.
         ***/
        Vector3     currRot     = transform.eulerAngles;
        Quaternion  startRot    = transform.rotation;
        Vector3     startPos    = transform.position;

        /**������ �������� ������ ������ ȸ�����⺤�͸� ���Ѵ�.*/
        Vector3 fallDownDir         = (FallDownDir - transform.position);
        float   height              = (transform.position-startPos).magnitude;
        float   radian              = Mathf.Atan2(fallDownDir.z, fallDownDir.x);
        float   cos                 = Mathf.Cos(radian);
        float   sin                 = Mathf.Sin(radian);

        Vector3 rotEuler = new Vector3(
            sin,
            0f,
            -cos
        );

        WaitForFixedUpdate waitTime = new WaitForFixedUpdate();


        /****************************************
         * ������ ��������.
         ***/
        while (fallDownRot<= FallDownTargetAngle)
        {
            if(rebound>0)
            {
                fallDownSpeed += FallDownSpeed * Time.fixedDeltaTime;
                fallDownRot += fallDownSpeed;
            }

            //������ �������� ���� ���� ���.
            if(fallDownRot>= FallDownTargetAngle) {

                //Tree Sound ���...
                FModAudioManager.PlayOneShotSFX(
                    FModSFXEventType.Tree_Obstacle,
                    FModLocalParamType.TreeActionType,
                    FModParamLabel.TreeActionType.TreeCrash,
                    transform.position,
 
                    -1,
                    -1
                );
            }

            //������ �ٴڿ� �پ��ֵ��� ����.
            _Body.position = startPos + (transform.up * height);

            _Body.MoveRotation(Quaternion.AngleAxis( fallDownRot, rotEuler)*startRot);
            yield return waitTime;
        }

        //collision�� ������̰�, �ݵ��� 0�̸� Ż��.
        if (rebound<=0f && FallDownUsedCollision)
        {
            _Body.Sleep();
            _Body.isKinematic = true;
            yield break;
        }


        /****************************************
         * ������ ������ �� �ݵ� ����.
         ***/
        _Body.isKinematic = true;
        _Body.Sleep();
        fallDownSpeed = 0f;
        fallDownRot = FallDownTargetAngle;

        while (rebound>0 || fallDownRot<= FallDownTargetAngle)
        {
            //���� ������ ƨ�ܳ���.
            if (rebound > 0 && fallDownRot >= FallDownTargetAngle) {
                fallDownSpeed = -rebound;
                fallDownRot = FallDownTargetAngle;
                rebound -= maxRebound * ReboundValue;

                //Tree Sound ���...
                FModAudioManager.PlayOneShotSFX(
                      FModSFXEventType.Tree_Obstacle,
                      FModLocalParamType.TreeActionType,
                      FModParamLabel.TreeActionType.TreeCrash,
                      transform.position,

                      -1,
                      -1
                  );
            }

            fallDownSpeed += FallDownSpeed * Time.deltaTime;
            fallDownRot += fallDownSpeed;

            //������ �ٴڿ� �پ��ֵ��� ����.
            if (FallDownUsedCollision) _Body.MovePosition(startPos + (transform.up * height));
            else _Body.position = startPos + (transform.up * height);

            _Body.MoveRotation(Quaternion.AngleAxis(fallDownRot, rotEuler) * startRot);
            yield return waitTime;
        }

        #endregion
    }
}
