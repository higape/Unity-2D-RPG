using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BET = Static.BattleEffect.EffectType;

namespace Static
{
    /// <summary>
    /// 特性，为装备附加一些特殊效果
    /// </summary>
    [Serializable]
    public class Trait : BattleEffect
    {
        private static List<int> GoldRateList { get; set; } = new List<int>();

        public static float GoldRate =>
            GoldRateList.Count == 0 ? 1f : Mathf.Max(GoldRateList.Sum() / 100f + 1f, 0);

        public static int GetValue(IEnumerable<Static.TraitData> traits, BET type0, int type1)
        {
            int sum = 0;
            foreach (var item in traits)
                if (item.Trait.EqualType(type0, type1))
                    sum += item.traitValue;
            return sum;
        }

        public static int GetValue(IEnumerable<Dynamic.DurationState> states, BET type0, int type1)
        {
            int sum = 0;
            foreach (var item in states)
                if (item.EqualType(type0, type1))
                    sum += item.EffectValue;
            return sum;
        }

        /// <summary>
        /// 添加全局效果
        /// </summary>
        public static void AddGlobalEffect(IEnumerable<Static.TraitData> traits)
        {
            foreach (var i in traits)
            {
                if (i.Trait.type0 == BET.Global)
                {
                    switch ((GlobalType)i.Trait.type1)
                    {
                        case GlobalType.GoldRate:
                            GoldRateList.Add(i.traitValue);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除全局效果
        /// </summary>
        public static void RemoveGlobalEffect(IEnumerable<Static.TraitData> traits)
        {
            foreach (var i in traits)
            {
                if (i.Trait.type0 == BET.Global)
                {
                    switch ((GlobalType)i.Trait.type1)
                    {
                        case GlobalType.GoldRate:
                            GoldRateList.Remove(i.traitValue);
                            break;
                    }
                }
            }
        }
    }
}
