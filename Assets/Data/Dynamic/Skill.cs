using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    public class Skill : Weapon, ITrait
    {
        public delegate bool BoolFunction();

        public Skill(int id)
        {
            DataObject = Root.ResourceManager.Skill.GetItem(id);
            IsCountLimit = false;
            CheckUsable = () => true;
        }

        public Skill(int id, int maxUsageCount)
        {
            DataObject = Root.ResourceManager.Skill.GetItem(id);
            MaxUsageCount = maxUsageCount;
            IsCountLimit = true;
            CheckUsable = () => true;
        }

        /// <param name="id">技能ID</param>
        /// <param name="checkUsable">此方法的返回值决定技能是否可以使用</param>
        public Skill(int id, BoolFunction checkUsable)
        {
            DataObject = Root.ResourceManager.Skill.GetItem(id);
            IsCountLimit = false;
            CheckUsable = checkUsable;
        }

        private Static.Skill DataObject { get; set; }

        public int ID => DataObject.id;

        public string Name => DataObject.Name;

        public Static.ActorWeaponSkin Skin => DataObject.Skin;

        public Static.UsedOccasion Occasion => DataObject.occasion;

        public Static.Skill.SkillType SkillType => DataObject.skillType;

        public Static.WeaponUsage Usage =>
            Root.ResourceManager.WeaponUsage.GetItem(DataObject.usageID);

        public int SelectionLimit => DataObject.selectionLimit;

        public int ItemQuantity => DataObject.itemQuantity;

        public int ItemUsedCount => DataObject.itemUsedCount;

        public int SkillEffectRatePercentage => DataObject.effectRatePercentage;

        public override float SkillEffectRate => DataObject.effectRatePercentage / 100f;

        public int WaitTime
        {
            get
            {
                if (SkillType == Static.Skill.SkillType.Usage)
                    return Usage.waitTime;
                else
                    return DataObject.waitTime;
            }
        }

        public Static.BattleEffectData[] AddedEffects => DataObject.addedEffects;

        public IEnumerable<Static.TraitData> Traits => DataObject.traits;

        public bool UsedInMenu => DataObject.UsedInMenu;

        public bool UsedInBattle => DataObject.UsedInBattle;

        /// <summary>
        /// 使用了多少次
        /// </summary>
        public int ConsumeCount { get; set; }

        /// <summary>
        /// 最大使用次数
        /// </summary>
        public int MaxUsageCount { get; set; }

        public int CurrentCount => MaxUsageCount - ConsumeCount;

        /// <summary>
        /// 是否有使用次数的限制
        /// </summary>
        public bool IsCountLimit { get; private set; }

        private BoolFunction CheckUsable { get; set; }

        /// <summary>
        /// 此技能是否激活，非激活的技能不会在菜单出现
        /// </summary>
        public bool IsEnable => IsCountLimit == false || MaxUsageCount > 0;

        /// <summary>
        /// 此技能是否可以使用
        /// </summary>
        public bool CanUse =>
            (IsCountLimit == false || ConsumeCount < MaxUsageCount) && CheckUsable.Invoke();

        public override int Attack => 0;

        protected override Vector3 FirePosition =>
            Skin.firePosition + Owner.DisplayObject.FirePosition;

        protected override void StartBullet()
        {
            if (Owner is Actor actor)
            {
                actor.ShowMotion(Skin, ProcessBullet);
            }
            else
            {
                ProcessBullet();
            }
        }

        public override bool CostAndCool()
        {
            if (CurrentWaitTime > 0)
                return false;

            if (IsCountLimit)
            {
                if (ConsumeCount >= MaxUsageCount)
                    return false;
                ConsumeCount++;
            }
            CurrentWaitTime = WaitTime;
            return true;
        }

        public void Cost()
        {
            if (IsCountLimit && ConsumeCount < MaxUsageCount)
            {
                ConsumeCount++;
            }
        }
    }
}
