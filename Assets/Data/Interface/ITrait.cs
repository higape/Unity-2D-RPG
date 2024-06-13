using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BE = Static.BattleEffect;
using BET = Static.BattleEffect.EffectType;

namespace Dynamic
{
    /// <summary>
    /// 为实现特殊能力的类提供属性和方法。
    /// </summary>
    public interface ITrait
    {
        private static List<int> GoldRateList { get; set; } = new List<int>();

        public static float GoldRate =>
            GoldRateList.Count == 0 ? 1f : Mathf.Max(GoldRateList.Max<int>() / 100f + 1f, 0);

        public static int GetValue(IEnumerable<ITrait> traits, BET type0, int type1)
        {
            int sum = 0;
            foreach (object item in traits)
                if (item is ITrait t)
                    sum += t.GetValue(type0, type1);
            return sum;
        }

        public static int GetValue(IEnumerable<DurationState> states, BET type0, int type1)
        {
            int sum = 0;
            foreach (var item in states)
                if (item.EffectType0 == type0 && item.EffectType1 == type1)
                    sum += item.EffectValue;
            return sum;
        }

        public static int GetValue(
            IEnumerable<ITrait> traits0,
            IEnumerable<ITrait> traits1,
            IEnumerable<DurationState> states,
            BET type0,
            int type1
        )
        {
            int sum = 0;
            foreach (object item in traits0)
                if (item is ITrait t)
                    sum += t.GetValue(type0, type1);
            foreach (object item in traits1)
                if (item is ITrait t)
                    sum += t.GetValue(type0, type1);
            foreach (var item in states)
                if (item.EffectType0 == type0 && item.EffectType1 == type1)
                    sum += item.EffectValue;
            return sum;
        }

        IEnumerable<Static.TraitData> Traits { get; }

        /// <summary>
        /// 添加全局效果
        /// </summary>
        public void AddGlobalEffect()
        {
            foreach (var i in Traits)
            {
                if (i.Effect.type0 == BET.Global)
                {
                    switch ((BE.GlobalType)i.Effect.type1)
                    {
                        case BE.GlobalType.GoldRate:
                            GoldRateList.Add(i.effectValue);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除全局效果
        /// </summary>
        public void RemoveGlobalEffect()
        {
            foreach (var i in Traits)
            {
                if (i.Effect.type0 == BET.Global)
                {
                    switch ((BE.GlobalType)i.Effect.type1)
                    {
                        case BE.GlobalType.GoldRate:
                            GoldRateList.Remove(i.effectValue);
                            break;
                    }
                }
            }
        }

        public int GetValue(BET type0, int type1)
        {
            int value = 0;
            foreach (var i in Traits)
            {
                if (i.Effect.type0 == type0 && i.Effect.type1 == type1)
                {
                    value += i.effectValue;
                }
            }
            return value;
        }
    }
}
