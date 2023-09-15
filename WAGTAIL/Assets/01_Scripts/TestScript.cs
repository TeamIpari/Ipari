using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class TestScript : MonoBehaviour
{
    PullInOutState    _state;
    bool isTrigger = false;

    private void Start()
    {
        _state = new PullInOutState(Player.Instance, Player.Instance.movementSM);
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

    public void Enterprint()
    {
        Debug.Log("Enterprint");
    }

    public void Exitprint()
    {
        Debug.Log("Exitprint");
    }

    public void MoveScene()
    {
        SceneManager.LoadScene("Chapter02");
    }

}
