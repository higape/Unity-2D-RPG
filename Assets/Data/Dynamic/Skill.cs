using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    public class Skill : Weapon, ITrait
    {
        public delegate bool BoolFunction();

        /// <param name="id">技能ID</param>
        /// <param name="isCountLimit">是否有使用次数的限制</param>
        /// <param name="checkUsable">此方法的返回值决定技能是否可以使用</param>
        public Skill(int id, bool isCountLimit, BoolFunction checkUsable)
        {
            DataObject = Root.ResourceManager.Skill.GetItem(id);
            IsCountLimit = isCountLimit;
            CheckUsable = checkUsable;
        }

        private Static.Skill DataObject { get; set; }
        public int ID => DataObject.id;
        public string Name => DataObject.Name;
        public Static.ActorWeaponSkin Skin => DataObject.Skin;
        public Static.UsedOccasion Occasion => DataObject.occasion;
        public Static.Skill.UsedType UsedType => DataObject.usedType;
        public int UsedValue => DataObject.usedValue;
        public int SelectionFilter => DataObject.selectionFilter;
        public override float SkillEffectRate => DataObject.effectRate / 100f;
        public int WaitTime => DataObject.waitTime;
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

        /// <summary>
        /// 是否有使用次数的限制
        /// </summary>
        public bool IsCountLimit { get; private set; }

        private BoolFunction CheckUsable { get; set; }

        /// <summary>
        /// 此技能是否激活，非激活的技能将不会出现在使用菜单
        /// </summary>
        public bool IsEnable => IsCountLimit == false || MaxUsageCount > 0;

        /// <summary>
        /// 此技能是否可以使用，不可使用的技能会在UI显示，但不能选择
        /// </summary>
        public bool CanUse =>
            (IsCountLimit == false || ConsumeCount < MaxUsageCount) && CheckUsable.Invoke();

        public override int Attack => 0;
        public Static.WeaponUsage Usage => DataObject.Usage;
        public Static.BattleEffectData[] AddedEffects => DataObject.addedEffects;
        public IEnumerable<Static.TraitData> Traits => DataObject.TraitList;
        protected override Vector3 FirePosition =>
            Skin.firePosition + Owner.DisplayObject.FirePosition;

        protected override void StartBullet()
        {
            if (Owner is Actor human)
            {
                human.ShowMotion(Skin, ProcessBullet);
            }
            else
            {
                ProcessBullet();
            }
        }

        public override void CostAndCool()
        {
            Debug.Log("消费和冷却技能");
            CurrentWaitTime = WaitTime;
            if (IsCountLimit)
                ConsumeCount++;
        }
    }
}
