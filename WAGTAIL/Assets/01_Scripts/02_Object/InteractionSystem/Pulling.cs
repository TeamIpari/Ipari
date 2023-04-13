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
            // player가 object를 미는 방향으로 보게끔 조절
            // TODO : 더 깔끔하게 해보기
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

    public float GetMeshfloat()
    {
        float  a = (100 - _skMesh.GetBlendShapeWeight(0)) / 100;
        //Debug.Log(a);
        return a;
        // (현재 위치 - 최대 위치) / 100
    }

    public void SetMeshfloat(int a)
    {
        //_skMesh.SetBlendShapeWeight(0);
    }
}
