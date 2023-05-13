using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
    }

    // Player가 충돌 시 coin +1
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(_player.coin >= 0)
            {
                _player.coin += 1;
                Destroy(this.gameObject);
            }
        }
    }
}
