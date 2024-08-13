using System;
using Root;

namespace Static
{
    /// <summary>
    /// 为 BattleEffect 提供效果参数
    /// </summary>
    [Serializable]
    public class BattleEffectData
    {
        /// <summary>
        /// 战斗效果ID或持续效果ID
        /// </summary>
        public int effectID;

        /// <summary>
        /// 效果值
        /// </summary>
        public int effectValue;

        /// <summary>
        /// 持续回合，值>0时添加为持续效果
        /// </summary>
        public int duration;

        /// <summary>
        /// 将效果限定于目标类型
        /// </summary>
        public Battler.BattlerType targetType;

        public BattleEffect GetBattleEffect() => ResourceManager.BattleEffect.GetItem(effectID);

        public DurationState GetDurationState() => ResourceManager.DurationState.GetItem(effectID);

        public string GetStatement()
        {
            string text;
            if (duration <= 0)
            {
                var be = GetBattleEffect();
                if (be.type0 == BattleEffect.EffectType.Cost && be.type1 == 2)
                    text = ResourceManager.ActorNormalItem.GetItem(effectValue).Name;
                else if (be.type0 == BattleEffect.EffectType.RemoveState && be.type1 == 1)
                    text = ResourceManager.DurationState.GetItem(effectValue).Name;
                else if (be.type0 == BattleEffect.EffectType.RemoveState && be.type1 == 3)
                    text = ResourceManager.Term.GetText((ElementType)effectValue);
                else if (be.type0 == BattleEffect.EffectType.Special && be.type1 == 4)
                    text = ResourceManager.Enemy.GetItem(effectValue).Name;
                else
                    text = effectValue.ToString();
                return string.Format(be.Statement, text);
            }
            else
            {
                return string.Format(
                    ResourceManager.Term.durationStateStatement,
                    GetDurationState().GetStatement(effectValue),
                    duration
                );
            }
        }
    }
}
