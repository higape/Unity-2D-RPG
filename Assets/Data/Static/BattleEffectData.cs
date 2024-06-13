using System;
using UnityEngine;

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

        public BattleEffect GetBattleEffect() =>
            Root.ResourceManager.BattleEffect.GetItem(effectID);

        public DurationState GetDurationState() =>
            Root.ResourceManager.DurationState.GetItem(effectID);
    }
}
