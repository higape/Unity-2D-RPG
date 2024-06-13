using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 人类角色的初始配置
    /// </summary>
    [Serializable]
    public class Actor : Battler
    {
        public enum Motion
        {
            [InspectorName("待命")]
            Idle = 0,

            [InspectorName("射击")]
            Shoot = 1,

            [InspectorName("投掷")]
            Throw = 2,
        }

        /// <summary>
        /// 包含技能组和技能使用次数与等级的关系
        /// </summary>
        [Serializable]
        public class SkillData
        {
            /// <summary>
            /// 技能ID
            /// </summary>
            public int id;

            /// <summary>
            /// 使用次数与角色等级的关系
            /// </summary>
            [SerializeField]
            private GrowthValue count;

            public int GetCount(int level) => count.GetValue(level);
        }

        /// <summary>
        /// 包含各动作的精灵图，图像的中心要设置好。
        /// </summary>
        [Serializable]
        public class ActorSkin
        {
            public Sprite idle;
            public Sprite raiseGun;
            public Sprite liftItem;
            public Sprite throwItem;
            public Sprite dead;
        }

        /// <summary>
        /// 初始等级，由等级决定初始经验值
        /// </summary>
        public int lv;
        public SkillData[] skills;
        public int[] weaponIDs;
        public int[] armorIDs;
        public ActorSkin battleSkin;
        public CharacterSkin characterSkin;
    }
}
