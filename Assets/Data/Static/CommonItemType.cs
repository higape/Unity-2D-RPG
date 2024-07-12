using UnityEngine;

namespace Static
{
    //分类与资源的存储有关
    public enum CommonItemType
    {
        [InspectorName("普通道具")]
        ActorNormalItem = 0,

        [InspectorName("恢复道具")]
        ActorRecoverItem = 1,

        [InspectorName("攻击道具")]
        ActorAttackItem = 2,

        [InspectorName("辅助道具")]
        ActorAuxiliaryItem = 3,

        [InspectorName("武器")]
        ActorWeapon = 100,

        [InspectorName("头部防具")]
        ActorHeadArmor = 201,

        [InspectorName("身体防具")]
        ActorBodyArmor = 202,

        [InspectorName("手部防具")]
        ActorHandArmor = 203,

        [InspectorName("腿部防具")]
        ActorFootArmor = 204,

        [InspectorName("饰品")]
        ActorOrnamentArmor = 205,
    }
}
