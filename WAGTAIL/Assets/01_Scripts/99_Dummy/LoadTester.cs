using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTester : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneLoader.GetInstance().LoadScene("SceneLoadTest2");
            //_uiManager.SwitchCanvas(CanvasType.GameUI);
        }
    }
}
