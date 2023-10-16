using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CoCoSiManager : MonoBehaviour
{
    public Toggle[] cocosiToggle;
    private CoCoSi[] _cocosi;
    // Start is called before the first frame update
    void Start()
    {
        _cocosi = GetComponentsInChildren<CoCoSi>();
        
        for(int i = 0; i < _cocosi.Length; i++)
        {
            _cocosi[i].index = i + 1;
        }
        
        cocosiToggle = UIManager.GetInstance().GetGameUI(GameUIType.CoCosi).GetComponentsInChildren<Toggle>();
    }
}
