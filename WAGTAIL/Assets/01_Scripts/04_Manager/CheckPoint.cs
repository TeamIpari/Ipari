using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public CheckPointType checkPointType;
    private GameManager _gameManager;
    public void Start()
    {
        _gameManager = GameManager.GetInstance();
        if (checkPointType == CheckPointType.StartPoint)
        {
            //gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _gameManager.num++;
            _gameManager.SwitchCheckPoint(transform.position);
            gameObject.SetActive(false);
        }
    }
}
