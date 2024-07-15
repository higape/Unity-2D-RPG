using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 特性，为装备附加一些特殊效果
    /// </summary>
    [Serializable]
    public class Trait : BattleEffect
    {
        [SerializeField]
        private string statement;
    }
}
