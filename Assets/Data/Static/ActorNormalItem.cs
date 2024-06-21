using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 角色普通物品，无法使用，通常附带一些文字信息。
    /// </summary>
    [Serializable]
    public class ActorNormalItem : DescriptionItem
    {
        public int price;
    }
}
