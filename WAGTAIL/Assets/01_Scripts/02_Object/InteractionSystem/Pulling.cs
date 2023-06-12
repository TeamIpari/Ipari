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
    [SerializeField] private Animator animator;

    private float _vAxis;
    private float _hAxis;

    Vector3 _playerPos;
    public int targetPercent = 70;

    Vector3 curPos;

    public string InteractionPrompt => _promt;

    public SkinnedMeshRenderer _skMesh;

    public int value = 0;

    private void Start()
    {
        _playerEquipPoint = Player.Instance.EquipPoint.gameObject;
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

    private void Update()
    {
        
    }

    void AngleCheck(Player player)
    {
        if (player.transform.rotation.eulerAngles.y < 45f
                && player.transform.rotation.eulerAngles.y > -45f)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
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
            _shatterObject.GetComponent<ShatterObject>()?.Explode();
            Destroy(_root);
            Destroy(this.gameObject);
        }
    }

    public int GetMeshfloat()
    {
        int a = 0;
        if (_skMesh != null)
            a = 100 - (100 - (int)_skMesh.GetBlendShapeWeight(0));
        return a;
    }

    public bool IsTarget()
    {
        if (GetMeshfloat() <= targetPercent)
            return false;
        if (animator != null)
            //animator.SetTrigger("End");
            _shatterObject.GetComponent<FlowerObject>()?.CreatePoint();
 
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
            float _val = _skMesh.GetBlendShapeWeight(0) + val;

            _skMesh.SetBlendShapeWeight(0, _val);
        }
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}
