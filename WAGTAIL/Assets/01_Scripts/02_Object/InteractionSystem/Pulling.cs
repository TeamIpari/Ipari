using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pulling : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promt;
    [SerializeField] GameObject _playerEquipPoint;

    private float _vAxis;
    private float _hAxis;

    Vector3 _playerPos;
    public Vector3 dir;

    Vector3 curPos;

    public string InteractionPrompt => _promt;

    public SkinnedMeshRenderer _skMesh;

    public int value = 0;

    public bool Interact(Interactor interactor)
    {
        if (interactor.player.movementSM.currentState == interactor.player.idle)
        {
            interactor.player.isPull = true;
            // player�� object�� �̴� �������� ���Բ� ����
            // TODO : �� ����ϰ� �غ���
            _playerPos = interactor.gameObject.transform.position;
            _hAxis = _playerPos.x - gameObject.transform.position.x;
            _vAxis = _playerPos.z - gameObject.transform.position.z;
            dir = new Vector3(_hAxis, 0, _vAxis).normalized;
            dir.x = -Mathf.Round(dir.x);
            dir.z = -Mathf.Round(dir.z);
            interactor.transform.LookAt(interactor.transform.position + dir);
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

    public int GetMeshfloat()
    {
        int  a = 100 - (100 - (int)_skMesh.GetBlendShapeWeight(0));
        //Debug.Log(a);
        return a;
        // (���� ��ġ - �ִ� ��ġ) / 100
    }

    public void SetMeshfloat(float val)
    {
        float _val = _skMesh.GetBlendShapeWeight(0) + val;

        _skMesh.SetBlendShapeWeight(0, _val);
    }
}
