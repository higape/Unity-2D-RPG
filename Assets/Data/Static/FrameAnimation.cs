using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Static
{
    /// <summary>
    /// 逐逐播放的动画。
    /// 暂时忽略声音。
    /// </summary>
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(FrameAnimation))]
    public class FrameAnimation : ScriptableObject
    {
        [Serializable]
        public class FrameData
        {
            /// <summary>
            /// 下一帧图像显示的时机
            /// </summary>
            [Tooltip("下一帧到来的时间点")]
            public int nextFrame;

            /// <summary>
            /// 要显示的sprite，需要设置好中心点
            /// </summary>
            public Sprite sprite;
        }

        public FrameData[] frames;

        [Tooltip("触发回调的时机")]
        public int callbackFrame;
    }
}
