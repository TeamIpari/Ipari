using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SerializeDictionary<key, value> : Dictionary<key, value>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<key> keys = new List<key>();

    [SerializeField]
    List<value> values = new List<value>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<key, value> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }

    }

    public void OnAfterDeserialize()
    {

        for (int i = 0, icount = keys.Count; i < icount; ++i)
        {
            this.Add(keys[i], values[i]);
        }

    }

}




