using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 成长数值，描述数值增长曲线
    /// </summary>
    [Serializable]
    public class GrowthValue
    {
        [SerializeField]
        private float basic;

        [SerializeField]
        private float power1;

        [SerializeField]
        private float power2;

        [SerializeField]
        private float power3;

        public int GetValue(int level)
        {
            float result =
                basic + level * power1 + level * level * power2 + level * level * level * power3;
            return (int)result;
        }
    }
}
