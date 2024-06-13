using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 物品的使用范围
    /// </summary>
    public enum UsedScope
    {
        None = 0,

        [InspectorName("自己")]
        Self = 1,

        [InspectorName("一个同伴")]
        OneFriend = 2,

        [InspectorName("一个同伴除了自己")]
        OneFriendExcludeSelf = 3,

        [InspectorName("所有同伴")]
        AllFriend = 4,

        [InspectorName("所有同伴除了自己")]
        AllFriendExcludeSelf = 5,

        [InspectorName("一个死去的同伴")]
        OneDeadFriend = 6,

        [InspectorName("所有死去的同伴")]
        AllDeadFriend = 7,

        [InspectorName("一个敌人")]
        OneEnemy = 8,

        [InspectorName("所有敌人")]
        AllEnemy = 9,

        [InspectorName("小扇形")]
        SmallSector = 101,

        [InspectorName("大扇形")]
        BigSector = 102,

        [InspectorName("小贯穿")]
        SmallRay = 201,

        [InspectorName("大贯穿")]
        BigRay = 202,

        [InspectorName("小圆形")]
        SmallCircle = 301,

        [InspectorName("大圆形")]
        BigCircle = 302,
    }
}
