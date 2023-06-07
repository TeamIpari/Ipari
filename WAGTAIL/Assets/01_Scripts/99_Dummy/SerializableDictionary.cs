using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SerializableDictionary<TKey, TValue> :Dictionary<TKey, TValue>, ISerializationCallbackReceiver 
{
    [SerializeField]
    public List<TKey> Keys = new List<TKey>();
    [SerializeField]
    public List<TValue> Values = new List<TValue>();


    public void OnAfterDeserialize()
    {
        this.Clear();
        if (Keys.Count != Values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable"));
        for (int i = 0; i < Keys.Count; i++)
            this.Add(Keys[i], Values[i]);

    }

    public void OnBeforeSerialize()
    {
        Keys.Clear();
        Values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            Keys.Add(pair.Key); Values.Add(pair.Value);
        }

    }

}
