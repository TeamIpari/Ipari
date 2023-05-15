using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
        //this.GetComponent<MeshRenderer>().enabled = false;
    }

    // Player가 충돌 시 coin +1
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(_player.flower >= 0)
            {
                _player.flower += 1;
                if (_interactionVFX != null)
                {
                    GameObject exploVFX = Instantiate(_interactionVFX, this.gameObject.transform.position, this.gameObject.transform.rotation);
                    Destroy(exploVFX, 2);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
