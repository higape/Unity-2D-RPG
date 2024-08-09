using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 子弹，包含3个帧动画
    /// </summary>
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(Bullet))]
    public class Bullet : ScriptableObject
    {
        /// <summary>
        /// 子弹制造方式
        /// </summary>
        public enum CreateType
        {
            [InspectorName("一个全部")]
            OneAll = 0,

            [InspectorName("每个")]
            Every = 1
        }

        /// <summary>
        /// 命中类型
        /// </summary>
        public enum HitType
        {
            [InspectorName("普通")]
            Normal = 0,

            [InspectorName("击退")]
            Retreat = 1
        }

        /// <summary>
        /// 开火特效
        /// </summary>
        public FrameAnimation readyAnimation;

        /// <summary>
        /// 飞行的子弹
        /// </summary>
        public FrameAnimation flyingAnimation;

        /// <summary>
        /// 命中特效
        /// </summary>
        public FrameAnimation collideAnimation;

        public CreateType createType = CreateType.Every;
    }
}
