using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public bool test = false;

    // Update is called once per frame
    void Update()
    {
        if(test)
        {
            CameraManager.GetInstance().SwitchCamera(CameraType.Village);
        }
    }
}
