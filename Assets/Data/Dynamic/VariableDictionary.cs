using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 在运行时存放游戏逻辑所需的值。
    /// 一个ID对应一个值。
    /// </summary>
    public class VariableDictionary<TKey, TValue>
    {
        /// <summary>
        /// 值改变时发送事件，参数包含键和改变后的值。
        /// </summary>
        public UnityEvent<TKey, TValue> valueChanged = new();

        private List<TKey> keys;
        private List<TValue> values;

        public VariableDictionary()
        {
            keys = new();
            values = new();
        }

        public VariableDictionary(IList<TKey> keys, IList<TValue> values)
        {
#if UNITY_EDITOR
            if (keys.Count != values.Count)
            {
                Debug.LogError("键值对配对异常。");
            }
#endif
            this.keys = new(keys);
            this.values = new(values);
        }

        public void SetValue(TKey key, TValue value)
        {
            int index = keys.IndexOf(key);

            if (index == -1)
            {
                keys.Add(key);
                values.Add(value);
            }
            else
            {
                values[index] = value;
            }

            valueChanged.Invoke(key, value);
        }

        public TValue GetValue(TKey key)
        {
            int index = keys.IndexOf(key);
            return index >= 0 ? values[index] : default;
        }

        //用于序列化保存
        public TKey[] GetKeys()
        {
            return keys.ToArray();
        }

        //用于序列化保存
        public TValue[] GetValues()
        {
            return values.ToArray();
        }
    }
}
