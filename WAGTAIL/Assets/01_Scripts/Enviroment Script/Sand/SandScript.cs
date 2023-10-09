using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


/************************************************
 *   �𷡰� �������� ȿ���� ������ ������Ʈ�Դϴ�.
 * ***/
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public sealed class SandScript : MonoBehaviour, IEnviroment
{
    #region Editor_Extenstion
#if UNITY_EDITOR
    [CustomEditor(typeof(SandScript))]
    private sealed class SandScriptEditor : Editor
    {
        //====================================
        ////            Fields            ////
        //====================================
        private SandScript EditorTarget;
        private Transform TargetTr;
        private bool SelectCenter;

        private GUIStyle _NodeStyle;
        private GUIStyle _NodeCenterStyle;



        //================================================
        ////        Magic and override methods        ////
        //================================================
        private void OnEnable()
        {
            GUI_Initialized();
        }


        //==========================================
        /////           GUI methods            /////
        //==========================================
        private void GUI_Initialized()
        {
            #region Omit
            /*****************************************
             *   ��� ��ҵ��� �ʱ�ȭ�Ѵ�....
             * ***/
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(.8f, .8f, .8f));
            tex.Apply();

            Texture2D tex2 = new Texture2D(1, 1);
            tex2.SetPixel(0, 0, Color.yellow);
            tex2.Apply();

            _NodeStyle = new GUIStyle();
            _NodeStyle.normal.background = tex;

            _NodeCenterStyle = new GUIStyle();
            _NodeCenterStyle.normal.background = tex2;

            /**SandScript�� ���������� �����Դٸ�...*/
            if (EditorTarget != null || (EditorTarget = (target as SandScript)))
            {
                TargetTr = EditorTarget.transform;
                EditorTarget.InitComp();
                EditorTarget.InitMesh();
                EditorTarget.UpdateSendMesh();
            }

            #endregion
        }



    }
#endif
    #endregion


    //======================================
    /////          Property             ////
    //======================================
    public Vector3[]    SandOffsetBounds   { get { return _sandBounds; } }
    public string       EnviromentPrompt   { get; set; } = String.Empty;
    public int          Precision          { get { return _precision; } set { _precision = (value < 0 ? 0 : value); } }
    public bool         IsHit              { get; set; } = false;
    public bool         PlayerOnSand       { get { return _playerOnSand; } }
    public bool         IsIntake           { get { return _isIntake; } }
    public float        IntakeDuration     {

        get { return _IntakeDuration; }
        set
        {
            _IntakeDuration = (value <= 0 ? 0 : value);
            _intakeDiv      = (1f / _IntakeDuration);
        }
    }

    [SerializeField] private bool       _playerOnSand = false;
    [SerializeField] public Vector3     SandCenterIntakeOffset  = Vector3.zero;
    [SerializeField] public Vector3     SandCenterIdleOffset    = Vector3.zero;
    [SerializeField] public float       PullPower = 6f;
    [SerializeField] private float      _IntakeDuration = 0.8f;
    [SerializeField] private int        _precision = 2;



    //=====================================
    ////            Fields             ////
    //=====================================

    /**ĳ���� ������Ʈ��� ���õ� �ʵ�...*/
    private MeshCollider    _meshCollider;
    private MeshRenderer    _renderer;
    private MeshFilter      _filter;
    private Material        _sandMat;

    /**�޽� ������ ���õ� �ʵ�...*/
    private static readonly Vector3[]   _sandBounds         = new Vector3[4];
    private Vector3                     _currCenterOffset   = Vector3.zero;
    private float                       _precisionDiv       = 1f;

    private Mesh        _mesh;
    private Vector3[]   _vertices;
    private Vector2[]   _uvs;
    private int[]       _indices;

    /**ħ�� ����...*/
    private float _intakeDiv          = 1f;
    private float _intakeDstDiv       = 0f;
    private float _intakeTime         = 0f;

    private Vector3 _startCenterOffset = Vector3.zero;
    private Vector3 _goalCenterOffset  = Vector3.zero;

    private float _startPullSpeed = 0f;
    private float _currPullSpeed  = 0f;
    private float _goalPullSpeed  = 0f;
    private float _totalTime = 0f;

    private float _playerLastSpeed  = 0f;
    private float _playerLastJumPow = 0f;

    private bool  _isIntake = false;
    private float _shakeTime = 0f;
    private bool _playerDead = false;


    //=======================================
    /////         Magic methods         /////
    //=======================================
    private void Awake()
    {
        #region Omit
        InitComp();
        InitMesh();
        UpdateSendMesh();

        _currCenterOffset = SandCenterIdleOffset;
        gameObject.layer = LayerMask.NameToLayer("Platform");
        gameObject.tag = "Platform";

        if (_renderer != null) {

            _sandMat = _renderer.material;
            _sandMat.SetFloat("_Speed", 0f);
        }
        #endregion
    }

    private void Update()
    {
        #region Omit

        /********************************************
         *   ���� �İ���� ȿ�� ����.
         * ***/
        float deltaTime = Time.deltaTime;
        if(_intakeTime>0f)
        {
            _intakeTime -= deltaTime;
            float progressRatio = ( 1f - Mathf.Clamp(_intakeTime * _intakeDiv,0f,1f));

            _currCenterOffset   = _startCenterOffset + (_goalCenterOffset- _startCenterOffset) * progressRatio;
            _currPullSpeed      = _startPullSpeed + (_goalPullSpeed - _startPullSpeed) * progressRatio;
            UpdateSendMesh();
        }

        /**�Ķ���͸� �����Ѵ�...*/
        _sandMat?.SetFloat("_Speed", _totalTime+=(deltaTime*_currPullSpeed));

        /**��鸲 ȿ��*/
        if (_currPullSpeed>0f && (_shakeTime -= deltaTime) <= 0f)
        {
            CameraManager.GetInstance().CameraShake(1.5f, .1f);
            _shakeTime = .1f;
        }


        /*********************************************
         *   �÷��̾ ���� ��� �ִ� ������ ó���� �Ѵ�.
         * ***/
        if (_playerOnSand) {

            Player player = Player.Instance;
            if (_playerDead) {

                /**�÷��̾ �������� ���...*/
                player.transform.position += (Vector3.down * .025f);
                return;
            }


            RaycastHit hit;
            if (_currPullSpeed>0f && GetPlayerFloorinfo(out hit))
            {
                bool isGround       = (hit.normal.y > 0);
                bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

                /**���� ���� ������ �����Ѵ�...*/
                if (isGround && isSameCollider)
                {
                    Vector3 playerPos       = player.transform.position;
                    Vector3 centerPos       = GetWorldCenterPosition();
                    Vector3 target2Center   = (centerPos - playerPos).normalized;
                    Vector3 right           = Vector3.Cross(hit.normal, target2Center);
                    Vector3 forward         = Vector3.Cross(right, hit.normal);

                    //Debug.DrawLine(playerPos, centerPos, Color.red);
                    float pullPow = (PullPower * _currPullSpeed);
                    player.controller.Move((forward * pullPow * deltaTime));

                    float target2CenterLen = (centerPos - playerPos).magnitude;

                    /**�÷��̾ �� ����� ���Դٸ� ��������...*/
                    if (!player.isDead && target2CenterLen < 1f){

                        player.controller.enabled = true;
                        _playerDead   = true;
                        player.isDead = true;
                    }
                }

                else{

                    _playerOnSand = false;
                    _playerDead = false;
                }
            }
        }

        #endregion
    }

    private void OnDrawGizmosSelected()
    {
        if (_mesh != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(_mesh, transform.position, transform.rotation, transform.localScale);
        }

        Vector3 pos = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pos+SandCenterIdleOffset, .2f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pos+SandCenterIntakeOffset, .2f);
    }

    private void OnCollisionStay(Collision collision)
    {
        #region Omit
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Boss") || _currPullSpeed <= 0f) return;

        /****************************************
         *   �浹�� ��ü�� ���� ���� ������ üũ�Ѵ�...
         * ****/
        int Count          = collision.contactCount;
        bool isGround      = false;
        ContactPoint point = new ContactPoint();

        for (int i = 0; i < Count; i++) {

            point = collision.GetContact(i);

            /**���� ������� Ȯ���ϰ� �񱳸� �ߴ��Ѵ�..*/
            if (point.normal.y < 0)
            {
                isGround = true;
                break;
            }
        }

        /*****************************************
         *   ���� ����� ���� ó��...
         * ***/
        if (isGround == false) return;

        Rigidbody body          = collision.rigidbody;
        Vector3 centerPos       = GetWorldCenterPosition();
        Vector3 target2Center   = (centerPos - body.position).normalized;
        Vector3 right           = Vector3.Cross(point.normal, target2Center);
        Vector3 forward         = Vector3.Cross(right, point.normal);

        float target2CenterLen  = (centerPos - body.position).magnitude;

        if (body != null) {

            float pullPow = (PullPower * _currPullSpeed);
            body.MovePosition(body.position + (forward * pullPow * Time.fixedDeltaTime));

            if(target2CenterLen<1f)
            {
                Physics.IgnoreCollision(collision.collider, _meshCollider);
                body.velocity = Vector3.zero;
                GameObject.Destroy(body.gameObject, 2f);
            }
        }

        #endregion
    }



    //======================================
    /////        Core methods           ////
    //======================================
    public void IntakeSand(bool apply)
    {
        #region Omit
        _isIntake         = apply;
        _goalCenterOffset = (apply? SandCenterIntakeOffset : SandCenterIdleOffset);
        _goalPullSpeed    = (apply? 1f:0f);

        _intakeTime         = _IntakeDuration;
        _intakeDiv          = (1f / _IntakeDuration);
        _startCenterOffset  = _currCenterOffset;
        _startPullSpeed     = _currPullSpeed;
        #endregion
    }

    private void InitComp()
    {
        #region Omit
        /**************************************
         *   ��� ��ҵ��� �ʱ�ȭ�Ѵ�...
         * ***/
        if(_meshCollider==null){

            _meshCollider = GetComponent<MeshCollider>();
        }

        if(_renderer==null){

            _renderer = GetComponent<MeshRenderer>();
        }

        if (_filter==null){

            _filter = GetComponent<MeshFilter>();    
        }
        #endregion
    }

    private void InitMesh()
    {
        #region Omit
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "AntHellMesh";
        }

        Vector3 currPos = Vector3.zero;
        _sandBounds[0] = currPos + (Vector3.left * 2f) + (Vector3.forward * 2f);
        _sandBounds[1] = currPos + (Vector3.right * 2f) + (Vector3.forward * 2f);
        _sandBounds[2] = currPos + (Vector3.left * 2f) + (Vector3.back * 2f);
        _sandBounds[3] = currPos + (Vector3.right * 2f) + (Vector3.back * 2f);

        _vertices = new Vector3[5 + (_precision * 4)];
        _uvs      = new Vector2[5 + (_precision * 4)];
        _indices  = new int[(_precision + 1) * 3 * 4];

        _precisionDiv = (1f / (_precision + 1));
        #endregion
    }

    private void UpdateSendMesh()
    {
        #region Omit
        if (_mesh == null) return;

        /************************************
         *   ��� �ﰢ������ �����Ѵ�....
         * ****/
        _vertices[0] = _currCenterOffset;
        _vertices[1] = _sandBounds[0];
        _vertices[2] = _sandBounds[1];
        _vertices[3] = _sandBounds[2];
        _vertices[4] = _sandBounds[3];

        _uvs[0] = new Vector2(.5f, .5f);
        _uvs[1] = new Vector2(0f, 1f);
        _uvs[2] = new Vector3(1f, 1f);
        _uvs[3] = new Vector3(0f, 0f);
        _uvs[4] = new Vector3(1f, 0f);

        int vIndex = 5, iIndex = 0;
        AddTriangles( 1, 2, 0, ref vIndex, ref iIndex );
        AddTriangles( 2, 4, 0, ref vIndex, ref iIndex );
        AddTriangles( 3, 1, 0, ref vIndex, ref iIndex );
        AddTriangles( 4, 3, 0, ref vIndex, ref iIndex );

        /**������ ������� �ٿ�� �ڽ�/����� ���� �� ��������...*/
        _mesh.vertices  = _vertices;
        _mesh.triangles = _indices;
        _mesh.uv        = _uvs;

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _filter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;

        #endregion
    }

    private void AddTriangles(int fromIndex, int toIndex, int centerIndex, ref int vertexStart, ref int indexStart)
    {
        #region Omit
        ref Vector3 from    = ref _vertices[fromIndex];
        ref Vector3 to      = ref _vertices[toIndex];
        ref Vector3 center  = ref _vertices[centerIndex];    

        float   ratio       = _precisionDiv;
        int     prevIndex   = vertexStart;
        Vector3 distance    = (to-from);

        Vector2 uvFrom      = _uvs[fromIndex];
        Vector2 uvDistance  = (_uvs[toIndex] - _uvs[fromIndex]);


        /***************************************
         *   ���ڸ� ������� �ﰢ���� �����Ѵ�...
         * ***/

        /**���ۺκ�...*/
        {
            _vertices[vertexStart] = from + (distance * ratio);
            _uvs[vertexStart]    = uvFrom + (uvDistance * ratio);

            _indices[indexStart++] = centerIndex;
            _indices[indexStart++] = fromIndex;
            _indices[indexStart++] = vertexStart++;

            ratio += _precisionDiv;
        }


        /**�߰� �κе��� ������ �߰��Ѵ�...*/
        for(int i=1; i<_precision; i++)
        {
            _vertices[vertexStart] = from + (distance * ratio);
            _uvs[vertexStart]      = uvFrom + (uvDistance * ratio);

            _indices[indexStart++] = (vertexStart - 1);
            _indices[indexStart++] = vertexStart++;
            _indices[indexStart++] = centerIndex;

            ratio += _precisionDiv;
        }


        /**������ �κ�...*/
        {
            _indices[indexStart++] = (vertexStart-1);
            _indices[indexStart++] = toIndex;
            _indices[indexStart++] = centerIndex;
        }
        #endregion
    }

    public bool AnimEvent()
    {
        throw new NotImplementedException();
    }

    public bool GetPlayerFloorinfo(out RaycastHit hit)
    {
        #region Ommision
        CharacterController con = Player.Instance.controller;

        float heightHalf        = con.height;
        float radius            = con.radius;
        float heightHalfOffset  = (heightHalf * .5f) - radius;
        Vector3 playerPos       = con.transform.position;
        Vector3 center          = (playerPos + con.center);

        return Physics.SphereCast(
            center,
            radius,
            Vector3.down,
            out hit,
            heightHalf + .1f,
            1<<gameObject.layer
        );
        #endregion
    }

    public bool Interact()
    {
        #region Omit

        /**************************************
         *   �÷��̾ ���� ����� ����� ó��..
         * ***/
        if (_playerOnSand == false)
        {
            RaycastHit hit;
            if (GetPlayerFloorinfo(out hit)){

                bool isGround       = (hit.normal.y > 0);
                bool isSameCollider = (hit.collider.gameObject.Equals(gameObject));

                /**���� ���� ������ �����Ѵ�...*/
                if (isGround && isSameCollider)
                {
                    _playerOnSand = true;
                    _playerDead = false;
                    return true;
                }
            }
        }

        return false;
        #endregion
    }

    public void ExecutionFunction(float time)
    {
        //interface implements...
    }

    private Vector3 GetWorldCenterPosition()
    {
        #region Omit
        Vector3 centerOffsetScaled = _currCenterOffset;
        centerOffsetScaled.Scale(transform.localScale);

        return (transform.position + centerOffsetScaled);
        #endregion
    }

    private Vector3 GetScaledExtent()
    {
        #region Omit
        Vector3 extent = transform.localScale;
        extent.Scale(Vector3.one * 2f);
        extent.y = 0f;

        return extent;
        #endregion
    }

    public float SampleHeight( Vector3 worldPos )
    {
        #region Omit
        /******************************************
         *   �־��� ���� ��ǥ������ ���̰��� ��ȯ�Ѵ�..
         * ***/
        Vector3 scaledExtent  = GetScaledExtent();
        Vector3 centerPos     = GetWorldCenterPosition();
        Vector3 center2target = (worldPos-centerPos);


        return 0f;

        #endregion
    }
}
