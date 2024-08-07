using System;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 技能
    /// </summary>
    [Serializable]
    public class Skill : DescriptionItem
    {
        /// <summary>
        /// 使用方式
        /// </summary>
        public enum SkillType
        {
            [InspectorName("被动")]
            Passivity = 0,

            [InspectorName("用法")]
            Usage = 1,

            [InspectorName("选择角色武器")]
            SelectActorWeapon = 2,

            [InspectorName("选择角色道具")]
            SelectActorItem = 3,
        }

        [SerializeField]
        private int skinID;

        public UsedOccasion occasion;

        public SkillType skillType;

        public int usageID;

        /// <summary>
        /// 选项限制。
        /// 技能为选择类型时，此值作为对应的物品类型枚举值
        /// </summary>
        public int selectionLimit;

        public int itemQuantity;

        public int itemUsedCount;

        /// <summary>
        /// 效率
        /// </summary>
        public int effectRatePercentage;

        /// <summary>
        /// 冷却时间
        /// </summary>
        public int coolingTime;

        /// <summary>
        /// 在武器道具基础上附加效果。
        /// </summary>
        public BattleEffectData[] addedEffects;

        /// <summary>
        /// 被动效果，拥有技能即生效
        /// </summary>
        public TraitData[] traits;

        [NonSerialized]
        private ActorWeaponSkin skin;

        public ActorWeaponSkin Skin =>
            skin ??= Root.ResourceManager.ActorWeaponSkin.GetItem(skinID);

        public bool UsedInMenu =>
            (occasion & UsedOccasion.Menu) != UsedOccasion.None && skillType == SkillType.Usage;

        public bool UsedInBattle => (occasion & UsedOccasion.Battle) != UsedOccasion.None;
    }
}
