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

            if (
                EffectType0 == BET.AdditionType
                && EffectType1 == (int)Static.BattleEffect.ActorType.Skill
            )
                Skill = new Skill(EffectValue);
        }

        private Static.DurationState DataObject { get; set; }
        public int ID => DataObject.id;
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

        public bool IsStrongerThan(DurationState state) =>
            DataObject.IsStrongerThan(state.DataObject);

        public bool IsWeakerThan(DurationState state) => DataObject.IsWeakerThan(state.DataObject);

        public void OnEffect()
        {
            Root.Mathc.ProcessItemEffect(
                DataObject,
                Owner,
                Target,
                EffectValue,
                ElementType,
                Attack,
                Hit,
                WeaponEffectRate
            );
        }
    }
}
