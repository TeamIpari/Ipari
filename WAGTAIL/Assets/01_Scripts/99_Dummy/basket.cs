using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 작성자: 성지훈
/// 추가 작성
/// </summary>
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
        if (other.CompareTag("interactable"))
        {
            if (TargetCount >= 0)
            {
                other.GetComponent<Throw>().enabled = false;
                TargetCount--;
            }
            if(TargetCount == -1)
                target.SetActive(true);
            ShowText();
        }
    }
}
