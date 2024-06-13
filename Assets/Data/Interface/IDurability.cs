using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dynamic
{
    public interface IDurability
    {
        /// <summary>
        /// 还挺得住
        /// </summary>
        public bool IsDurabilityOk => !IsZeroDurability;

        /// <summary>
        /// 完好无损
        /// </summary>
        public bool IsFullDurability => Damage == 0;

        /// <summary>
        /// 完全损坏
        /// </summary>
        public bool IsZeroDurability => Damage == MaxDurability && MaxDurability > 0;

        int Damage { get; set; }

        int MaxDurability { get; }

        public int CurrentDurability => MaxDurability - Damage;

        public void GainDurability()
        {
            if (!IsFullDurability)
                Damage--;
        }

        public void LoseDurability()
        {
            if (!IsZeroDurability)
                Damage++;
        }
    }
}
