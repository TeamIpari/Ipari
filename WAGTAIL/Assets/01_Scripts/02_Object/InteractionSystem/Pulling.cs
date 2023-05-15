using System.Collections;
using System.Collections.Generic;
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

    private float _vAxis;
    private float _hAxis;

    Vector3 _playerPos;
    public Vector3 dir;

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
            // player가 object를 미는 방향으로 보게끔 조절
            // TODO : 더 깔끔하게 해보기
            //_playerPos = interactor.gameObject.transform.position;
            //_hAxis = _playerPos.x - gameObject.transform.position.x;
            //_vAxis = _playerPos.z - gameObject.transform.position.z;
            //dir = new Vector3(_hAxis, 0, _vAxis).normalized;
            //dir.x = -Mathf.Round(dir.x);
            //dir.z = -Mathf.Round(dir.z);
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.position = new Vector3(_movePos.position.x, Player.Instance.transform.position.y, _movePos.position.z);
            //interactor.transform.LookAt(interactor.transform.position + dir);
            Player.Instance.transform.LookAt(new Vector3(_root.transform.position.x, Player.Instance.transform.position.y, _root.transform.position.z));
            Player.Instance.GetComponent<CharacterController>().enabled = true;
            transform.SetParent(_playerEquipPoint.transform, true);
            return true;
        }

        else if (interactor.player.movementSM.currentState == interactor.player.pull)
        {
            interactor.player.isPull = false;
            _playerEquipPoint.transform.DetachChildren();
            return true;
        }

        return false;
    }

    public void Drop()
    {
        _playerEquipPoint.transform.DetachChildren();
        if(GetMeshfloat() >= 90)
        {
            //_interactable.GetComponent<IInteractable>().Interact(null);
            _shatterObject.GetComponent<ShatterObject>().Explode();
            Destroy(_root);
            Destroy(this.gameObject);
        }
    }

    public int GetMeshfloat()
    {
        int  a = 100 - (100 - (int)_skMesh.GetBlendShapeWeight(0));
        return a;
    }

    public void SetMeshfloat(float val)
    {
        float _val = _skMesh.GetBlendShapeWeight(0) + val;

        _skMesh.SetBlendShapeWeight(0, _val);
    }

    public bool AnimEvent()
    {
        throw new System.NotImplementedException();
    }
}
