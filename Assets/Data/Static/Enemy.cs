using System;
using UnityEngine;

namespace Static
{
    [Serializable]
    public class Enemy : Battler
    {
        [Serializable]
        public class EnemyAction
        {
            public int usageID;

            /// <summary>
            /// 耐久，可抵挡耐久攻击
            /// </summary>
            public int durability;

            public Vector3 firePosition;

            [NonSerialized]
            private WeaponUsage usage;

            public WeaponUsage Usage => usage ??= Root.ResourceManager.WeaponUsage.GetItem(usageID);
        }

        public ElementGroup elementGroup;

        public Sprite skin;

        public GrowthValue gold;

        /// <summary>
        /// 一回合的行动次数
        /// </summary>
        public int actionCount;

        public EnemyAction[] actions;

        public TraitData[] traits;
    }
}
