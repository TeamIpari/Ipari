using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

[System.Serializable]
public struct FMODEventCategoryDesc
{
   [SerializeField] public string categoryName;
   [SerializeField] public bool show;
   [SerializeField] public int Count;
}

public class FMODAudioEventData : ScriptableObject
{
    [SerializeField] public string BGMBusName;
    [SerializeField] public string SFXBusName;
    [SerializeField] public string BGMGroupRootFolder = "BGM";
    [SerializeField] public string SFXGroupRootFolder = "SFX";
    [SerializeField] public List<EventReference> BGMEvents      = new List<EventReference>();
    [SerializeField] public List<FMODEventCategoryDesc> BGMDesc = new List<FMODEventCategoryDesc>();
    [SerializeField] public List<EventReference> SFXEvents      = new List<EventReference>();
    [SerializeField] public List<FMODEventCategoryDesc> SFXDesc = new List<FMODEventCategoryDesc>();
    [SerializeField] public List<EventReference> NoneEvents     = new List<EventReference>();
    [SerializeField] public List<FMODEventCategoryDesc> NoneDesc = new List<FMODEventCategoryDesc>();
}

#endif