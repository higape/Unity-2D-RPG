using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 同样的效果可以对应不同的名称和说明。
    /// 增益和减益的分类未完成。
    /// </summary>
    [Serializable]
    public class BattleEffect : DescriptionItem
    {
        public enum EffectType
        {
            None = 0,

            [InspectorName("伤害")]
            Damage = 1,

            [InspectorName("恢复")]
            Recover = 2,

            [InspectorName("消耗")]
            Cost = 3,

            [InspectorName("反馈")]
            Repay = 4,

            [InspectorName("消除状态")]
            RemoveState = 5,

            [InspectorName("特殊")]
            Special = 6,

            [InspectorName("控制")]
            Control = 7,

            [InspectorName("全局")]
            Global = 1001,

            [InspectorName("能力常数变化")]
            AbilityConst = 1002,

            [InspectorName("能力比例变化")]
            AbilityRate = 1003,

            // [InspectorName("状态免疫")]
            // IgnoreState = 1004,

            [InspectorName("附加")]
            AdditionType = 1005,
        }

        public enum DamageType
        {
            [InspectorName("倍率")]
            Rate = 1,

            [InspectorName("常量")]
            Constant = 2,

            [InspectorName("攻击力比例")]
            AttackRate = 3,

            [InspectorName("无视防御力比例")]
            NonDefenceRate = 4,

            [InspectorName("防御力比例")]
            DefenceRate = 5,

            [InspectorName("耐久度伤害(常量)")]
            Durability = 6,
        }

        public enum RecoverType
        {
            [InspectorName("生命(比例)")]
            LifeRate = 101,

            [InspectorName("生命(常量)")]
            LifeConst = 102,

            [InspectorName("生命(等级)")]
            LifeLevel = 103,

            [InspectorName("复活并恢复生命(比例)")]
            RebornRate = 201,

            [InspectorName("复活并恢复生命(常量)")]
            RebornConst = 202,

            [InspectorName("复活并恢复生命(等级)")]
            RebornLevel = 203,

            [InspectorName("耐久(常量)")]
            Durability = 302,
        }

        public enum CostType
        {
            [InspectorName("耐久")]
            Durability = 1,

            [InspectorName("普通道具(ID)")]
            NormalItem = 2,
        }

        public enum RepayType
        {
            [InspectorName("以伤害量恢复生命(比例)")]
            RecoverLifeRate = 1,
        }

        /// <summary>
        /// 消除被动状态的依据。
        /// 默认消除所有符合条件的状态。
        /// </summary>
        public enum StateType
        {
            /// <summary>
            /// 对应被动状态ID
            /// </summary>
            [InspectorName("ID")]
            ID = 1,

            /// <summary>
            /// 对应战斗效果类型
            /// </summary>
            [InspectorName("效果类型")]
            EffectType = 2,

            /// <summary>
            /// 对应元素类型
            /// </summary>
            [InspectorName("元素类型")]
            ElementType = 3,

            /// <summary>
            /// 由持续效果类判断
            /// </summary>
            [InspectorName("增益")]
            Buff = 4,

            /// <summary>
            /// 由持续效果类判断
            /// </summary>
            [InspectorName("减益")]
            Debuff = 5,
        }

        public enum SpecialType
        {
            [InspectorName("逃跑")]
            Escape = 1,

            [InspectorName("保护")]
            Protect = 2,

            [InspectorName("消灭自己")]
            Suicide = 3,

            [InspectorName("呼叫怪物")]
            CallHelper = 4,
        }

        /// <summary>
        /// 控制效果的数量大于抵抗力时，角色进入被控状态。
        /// 枚举值较大者优先应用。
        /// </summary>
        public enum ControlType
        {
            None = 0,

            [InspectorName("恐慌(禁用技能)")]
            Panic = 1,

            [InspectorName("魅惑(投靠敌人)")]
            Charm = 2,

            [InspectorName("混乱(随机行为)")]
            Confusion = 3,

            [InspectorName("束缚(不能行动)")]
            Fetter = 4,
        }

        public enum GlobalType
        {
            [InspectorName("获得金钱变化(百分比)")]
            GoldRate = 1,
        }

        public enum ActorType
        {
            [InspectorName("生命")]
            Life = 1,

            [InspectorName("攻击力")]
            Attack = 2,

            [InspectorName("防御力")]
            Defence = 3,

            [InspectorName("速度")]
            Agility = 4,

            [InspectorName("命中")]
            Hit = 5,

            [InspectorName("回避")]
            Evasion = 6,

            [InspectorName("经验值")]
            Exp = 1001,

            [InspectorName("受到伤害")]
            ReceivedDamage = 1002,

            [InspectorName("抵抗力")]
            Resistance = 1003,

            /// <summary>
            /// 为角色附加技能
            /// </summary>
            [InspectorName("技能(ID)")]
            Skill = 2001,
        }

        public EffectType type0;
        public int type1;

        [SerializeField]
        private string statement;
        public string Statement => statement;

        public bool EqualType(EffectType type0, int type1)
        {
            return this.type0 == type0 && this.type1 == type1;
        }
    }
}
