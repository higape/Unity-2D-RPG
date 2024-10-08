using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BET = Static.BattleEffect.EffectType;

namespace Dynamic
{
    /// <summary>
    /// 挂在战斗者身上的状态，会在每个回合结束时对目标执行效果
    /// </summary>
    public class DurationState
    {
        public DurationState(
            Static.DurationState dataObject,
            Battler owner,
            Battler target,
            int effectValue,
            int attack,
            int hit,
            float weaponEffectRate,
            int duration
        )
        {
            DataObject = dataObject;
            Owner = owner;
            Target = target;
            EffectValue = effectValue;
            Attack = attack;
            Hit = hit;
            WeaponEffectRate = weaponEffectRate;
            LastTurn = duration;

            if (EqualType(BET.AdditionType, (int)Static.BattleEffect.AdditionType.Skill))
                Skill = new Skill(EffectValue);
        }

        private Static.DurationState DataObject { get; set; }
        public int ID => DataObject.id;
        public string Name => DataObject.Name;
        public BET EffectType0 => DataObject.type0;
        public int EffectType1 => DataObject.type1;
        public Static.ElementType ElementType => DataObject.elementType;
        public Static.FrameAnimation DisplayObject => DataObject.displayObject;
        public Battler Owner { get; set; }
        public Battler Target { get; set; }
        public int EffectValue { get; set; }
        public int Attack { get; set; }
        public int Hit { get; set; }
        public float WeaponEffectRate { get; set; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        public int LastTurn { get; set; }

        /// <summary>
        /// 由自身特性产生的技能
        /// </summary>
        public Skill Skill { get; private set; }

        /// <summary>
        /// 此实例的来源
        /// </summary>
        public Weapon SourceWeapon { get; private set; }

        public bool EqualType(BET type0, int type1) => DataObject.EqualType(type0, type1);

        public bool IsStrongerThan(DurationState state) =>
            DataObject.IsStrongerThan(state.DataObject);

        public bool IsWeakerThan(DurationState state) => DataObject.IsWeakerThan(state.DataObject);

        public string GetStatement(int effectValue) => DataObject.GetStatement(effectValue);

        public void OnEffect()
        {
            Root.Mathc.ProcessItemEffect(
                EffectType0,
                EffectType1,
                EffectValue,
                Owner,
                Target,
                ElementType,
                Attack,
                Hit,
                WeaponEffectRate
            );
        }
    }
}
