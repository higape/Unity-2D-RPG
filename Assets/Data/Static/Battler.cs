using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 战斗成员的基类
    /// </summary>
    public abstract class Battler : NameItem
    {
        [Flags]
        public enum BattlerType
        {
            [InspectorName("未知/不限")]
            Unknown = 0b0,

            [InspectorName("生物")]
            LivingThing = 0b1,

            [InspectorName("机械")]
            Machinery = 0b10,
        }

        public BattlerType type;
        public GrowthValue exp;
        public GrowthValue hp;
        public GrowthValue atk;
        public GrowthValue def;
        public GrowthValue agi;
        public GrowthValue hit;
        public GrowthValue eva;
    }
}
