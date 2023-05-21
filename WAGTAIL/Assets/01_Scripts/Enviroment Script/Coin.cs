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
        // ���Ŀ� �ؿ� �ּ� Ǯ������� ���ϸ� ����
        // ��ġ�� ���� Dummy Mesh�̹Ƿ� Coin �𵨸� �Ǵ� FX�ϼ� �Ǹ� �ݵ�� �ּ��� Ǯ�������
        //this.GetComponent<MeshRenderer>().enabled = false;
    }

    // Player�� �浹 �� coin +1
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
