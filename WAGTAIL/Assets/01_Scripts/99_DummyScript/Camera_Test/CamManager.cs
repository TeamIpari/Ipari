using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

public class CamManager : MonoBehaviour
{


}

//[CanEditMultipleObjects]
[CustomEditor(typeof(CamManager))]
public class CustomCamLine : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();


    }
}
