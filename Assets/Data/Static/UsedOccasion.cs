using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 物品的使用场合
    /// </summary>
    [Flags]
    public enum UsedOccasion
    {
        None = 0b_0000_0000,

        [InspectorName("菜单")]
        Menu = 0b_0000_0001,

        [InspectorName("战斗")]
        Battle = 0b_0000_0010,
    }
}
