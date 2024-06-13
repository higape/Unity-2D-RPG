using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 用在装备上，用于存储特殊能力的效果值
    /// </summary>
    [Serializable]
    public class TraitData
    {
        [SerializeField]
        private int effectID;

        public int effectValue;

        [NonSerialized]
        private BattleEffect effect;

        public BattleEffect Effect =>
            effect ??= Root.ResourceManager.BattleEffect.GetItem(effectID);

        public string GetDescription() => string.Format(Effect.Description, effectValue.ToString());

        public bool EqualType(BattleEffect.EffectType type0, int type1)
        {
            return Effect.type0 == type0 && Effect.type1 == type1;
        }
    }
}
