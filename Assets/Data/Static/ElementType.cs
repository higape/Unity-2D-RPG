using UnityEngine;

namespace Static
{
    /// <summary>
    /// 元素类型的枚举
    /// </summary>
    public enum ElementType
    {
        [InspectorName("通常")]
        Normal = 0,

        [InspectorName("腐蚀")]
        Corrosion = 1,

        [InspectorName("火焰")]
        Fire = 2,

        [InspectorName("冷气")]
        Ice = 3,

        [InspectorName("电气")]
        Electricity = 4,

        [InspectorName("音波")]
        Wave = 5,

        [InspectorName("光线")]
        Ray = 6,

        [InspectorName("毒气")]
        Gas = 7,
    }
}
