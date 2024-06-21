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
        public enum UsedType
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

        public UsedType usedType;

        /// <summary>
        /// 由 UsedType 决定意义，可以是用法ID或数量
        /// </summary>
        public int usedValue;

        /// <summary>
        /// 选项过滤。
        /// 技能为选择类型时，将此值可转换为对应的flag枚举
        /// </summary>
        public int selectionFilter;

        /// <summary>
        /// 效率
        /// </summary>
        public int effectRate;

        /// <summary>
        /// 冷却时间，技能类型为Usage时也以此值为准
        /// </summary>
        public int waitTime;

        /// <summary>
        /// 在武器道具基础上附加效果。
        /// </summary>
        public BattleEffectData[] addedEffects;

        /// <summary>
        /// 被动效果，拥有技能即生效
        /// </summary>
        [SerializeField]
        private TraitData[] traits;

        [NonSerialized]
        private List<TraitData> traitList;

        public List<TraitData> TraitList => traitList ??= new(traits);

        [NonSerialized]
        private ActorWeaponSkin skin;

        public ActorWeaponSkin Skin =>
            skin ??= Root.ResourceManager.ActorWeaponSkin.GetItem(skinID);

        [NonSerialized]
        private WeaponUsage usage;

        public WeaponUsage Usage => usage ??= Root.ResourceManager.WeaponUsage.GetItem(usedValue);

        public bool UsedInMenu =>
            (occasion & UsedOccasion.Menu) != UsedOccasion.None && usedType == UsedType.Usage;

        public bool UsedInBattle => (occasion & UsedOccasion.Battle) != UsedOccasion.None;
    }
}
