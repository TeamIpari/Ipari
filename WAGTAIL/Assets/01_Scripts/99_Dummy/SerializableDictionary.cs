using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public TValue GetRandomValue<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;

        return values[rand.Next(size)];
    }


}
