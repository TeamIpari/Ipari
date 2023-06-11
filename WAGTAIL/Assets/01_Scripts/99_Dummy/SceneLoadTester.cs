using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTester : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneLoader.GetInstance().LoadScene("SceneLoadTest");
            //UIManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
        }
    }
}
