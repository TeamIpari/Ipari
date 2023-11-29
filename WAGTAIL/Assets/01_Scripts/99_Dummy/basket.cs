using UnityEngine;
using TMPro;


public class basket : MonoBehaviour
{
    public int TargetCount;
    //[SerializeField] private int CurCount;
    public TextMeshProUGUI CountBase;
    public GameObject target;


    private void Start()
    {
        target.SetActive(false); 
        ShowText();
    }

    void ShowText()
    {
        CountBase.text = (TargetCount + 1).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("interactable") && other.GetComponent<BombObject>() == null)
        {
            try
            {
                ThrowObject _throw = other.GetComponent<ThrowObject>();
                if(_throw.enabled == true)
                {
                    if (TargetCount >= 0)
                    {
                        _throw.gameObject.layer = 0;
                        _throw.enabled = false;
                        FModAudioManager.PlayOneShotSFX(FModSFXEventType.Put_KoKoShi);
                        TargetCount--;
                    }
                    if (TargetCount == -1)
                        target.SetActive(true);
                    ShowText();
                }
            }
            catch
            {
                Debug.Log("Not Throw Object");
            }
        }
    }
}
