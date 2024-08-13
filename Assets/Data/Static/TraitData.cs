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
        private int traitID;

        public int traitValue;

        [NonSerialized]
        private Trait trait;

        public Trait Trait => trait ??= Root.ResourceManager.Trait.GetItem(traitID);

        public string GetStatement()
        {
            string text;
            if (Trait.EqualType(BattleEffect.EffectType.AdditionType, 1))
                text = Root.ResourceManager.Skill.GetItem(traitValue).Name;
            else if (traitValue >= 0)
                text = '+' + traitValue.ToString();
            else
                text = traitValue.ToString();
            return string.Format(Trait.Statement, text);
        }
    }
}
