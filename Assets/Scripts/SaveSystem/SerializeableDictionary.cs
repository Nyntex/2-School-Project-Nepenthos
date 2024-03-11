using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializeableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    public List<TKey> keys = new List<TKey>();
    public List<TValue> values = new List<TValue>();

    public void OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {
            Debug.LogError("The amount of keys in the dictionary: " +  keys.Count +
                ", does not equal\nthe amount of values: " +  values.Count + "," +
                "\nwhich indicates that something went horribly wrong!" + 
                "\n\nContinuing with Deserialization.");
        }

        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
