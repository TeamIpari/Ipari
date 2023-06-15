using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class Pulling : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;
    [SerializeField] private GameObject _root;
    [SerializeField] private GameObject _interactable;
    [SerializeField] private Transform _movePos;
    [SerializeField] private GameObject _shatterObject;
    [SerializeField] private Vector3 InitPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private float minDistance;

    private float _vAxis;
    private float _hAxis;

    Vector3 _playerPos;
    public int blendTarget = 0;
    public float targetPercent = 70;

    Vector3 curPos;

    public string InteractionPrompt => _promt;

    public SkinnedMeshRenderer _skMesh;

    public int value = 0;

    private void Start()
    {
        _playerEquipPoint = Player.Instance.EquipPoint.gameObject;
        InitPoint = this.transform.position;    // 자신의 처음 위치 기억.
    }

    public bool Interact(Interactor interactor)
    {
        if (interactor.player.movementSM.currentState == interactor.player.idle)
        {
            interactor.player.isPull = true;
            if (animator != null)
                animator.SetTrigger("Start");
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            //Player.Instance.transform.position = _movePos.position;
            Player.Instance.transform.position = new Vector3(_movePos.position.x, interactor.player.transform.position.y, _movePos.position.z);
            //interactor.transform.LookAt(interactor.transform.position + dir);
            Player.Instance.transform.LookAt(new Vector3(transform.position.x, Player.Instance.transform.position.y, transform.position.z));

            AngleCheck(interactor.player);
            Player.Instance.GetComponent<CharacterController>().enabled = true;
            transform.SetParent(_playerEquipPoint.transform, true);
            return true;
        }

        else if (interactor.player.movementSM.currentState == interactor.player.pull)
        {
            interactor.player.isPull = false;
            if (animator != null)
                animator.SetTrigger("Cancel");
            _playerEquipPoint.transform.DetachChildren();
            return true;
        }

        return false;
    }

    public bool GetDistance()
    {
       // Debug.Log(Vector3.Distance(Player.Instance.transform.position, _shatterObject.transform.position)/* > minDistance*/);
        if (Vector3.Distance(Player.Instance.transform.position, _shatterObject.transform.position) > minDistance)
        {
            return false;
        }

        return true;
    }

    //private void Update()
    //{
        
    //}

    //private void LateUpdate()
    //{

    //}

    void AngleCheck(Player player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        //if (player.transform.rotation.eulerAngles.y < 45f
        //        && player.transform.rotation.eulerAngles.y > -45f)
        //{
        //}
        //else if (player.transform.rotation.eulerAngles.y > 45f
        //    && player.transform.rotation.eulerAngles.y < 135f)
        //{
        //    player.transform.rotation = Quaternion.Euler(0, 90, 0);
        //}
        //else if (player.transform.rotation.eulerAngles.y > 135
        //    && player.transform.rotation.eulerAngles.y )


    }

    public void Drop()
    {
        _playerEquipPoint.transform.DetachChildren();
        if(IsTarget())
        {
            try
            {
                _shatterObject.GetComponent<ShatterObject>()?.Explode();

            }
            catch
            {
                ;
            }
            Destroy(_root);
            Destroy(this.gameObject);
        }
        // 원래 위치로 초기화
        transform.position = InitPoint;
    }

    public int GetMeshfloat()
    {
        int a = 0;
        try
        {
            Mesh m = _skMesh.sharedMesh;

            for (int i = 0; i < blendTarget; i++)
            {
                a = 100 - (100 - (int)_skMesh.GetBlendShapeWeight(i));

            }

        }
        catch
        {

        }
        return a;
    }

    public bool IsTarget()
    {
        float floats = GetMeshfloat();
        if (floats <= targetPercent)
        {
            ;
            return false;
        }
        try
        {
            ;
            if (animator != null)
                _shatterObject.GetComponent<FlowerObject>()?.CreatePoint();
        }
        catch
        {

        }


        return true;
    }

    public void SetMeshfloat(float val)
    {
        if (animator != null && _skMesh != null )
        {
            animator.SetFloat("Blend", animator.GetFloat("Blend") + val);

        }
        else if( _skMesh != null)
        {
            Mesh m = _skMesh.sharedMesh;

            float _val = _skMesh.GetBlendShapeWeight(0) + val;
            _skMesh.SetBlendShapeWeight(0, _val);

        }
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}
