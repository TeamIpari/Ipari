using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject _interactionVFX;
    private Player _player;


    private void Start()
    {
        _player = Player.Instance;
        // 추후에 밑에 주석 풀어줘야함 안하면 죽음
        // 배치를 위한 Dummy Mesh이므로 Coin 모델링 또는 FX완성 되면 반드시 주석을 풀어줘야함
        //this.GetComponent<MeshRenderer>().enabled = false;
    }

    // Player가 충돌 시 coin +1
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(_player.coin >= 0)
            {
                _player.coin += 1;
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
