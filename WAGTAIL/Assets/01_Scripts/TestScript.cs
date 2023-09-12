using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TestScript : MonoBehaviour
{
    FModEventInstance _WaterStream;
    PullInOutState    _state;
    bool isTrigger = false;

    private void Start()
    {
        _WaterStream = FModAudioManager.CreateInstance(FModSFXEventType.Water_Stream, transform.position);
        _WaterStream.Volume = 10f;
        _WaterStream.Set3DDistance(20f, 40f);
        //_WaterStream.Play();

        _state = new PullInOutState(Player.Instance, Player.Instance.movementSM);
    }

    private void OnDestroy()
    {
        _WaterStream.Destroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _WaterStream.Max3DDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Player.Instance.tag)){

            isTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Player.Instance.tag)){

            isTrigger = false;
        }
    }

    private void Update()
    {
        if (isTrigger && Input.GetKeyDown(KeyCode.R)){

            _state.HoldTarget(gameObject);
        }
    }

}
