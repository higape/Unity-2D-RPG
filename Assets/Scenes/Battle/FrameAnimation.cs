using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    /// <summary>
    /// 帧动画，逐张播放精灵图，可以循环。
    /// </summary>
    public class FrameAnimation : MonoBehaviour
    {
        private const float FrameTime = 1f / 60f;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Static.FrameAnimation DataObject { get; set; }

        private UnityAction Callback { get; set; }

        private bool IsCallbackInvoked { get; set; }

        private float TimeCount { get; set; }

        private int FrameIndex { get; set; }

        private bool IsCycle { get; set; }

        private Static.FrameAnimation.FrameData[] Frames => DataObject.frames;

        private void Start()
        {
            RefreshFrame();
        }

        private void Update()
        {
            TimeCount += Time.deltaTime;

            if (TimeCount >= DataObject.callbackFrame * FrameTime)
                InvokeCallback();

            if (TimeCount >= Frames[FrameIndex].nextFrame * FrameTime)
                if (++FrameIndex < Frames.Length)
                    RefreshFrame();
                else if (IsCycle)
                    Replay();
                else
                    Destroy(gameObject);
        }

        public void Setup(Static.FrameAnimation dataObject, bool isCycle, UnityAction callback)
        {
            DataObject = dataObject;
            IsCycle = isCycle;
            Callback = callback;
            IsCallbackInvoked = false;
            TimeCount = 0;
            FrameIndex = 0;
        }

        private void Replay()
        {
            TimeCount = 0;
            FrameIndex = 0;
            RefreshFrame();
        }

        private void RefreshFrame()
        {
            spriteRenderer.sprite = Frames[FrameIndex].sprite;
        }

        private void InvokeCallback()
        {
            if (!IsCallbackInvoked && Callback != null)
            {
                IsCallbackInvoked = true;
                Callback.Invoke();
            }
        }

        // Called by event
        public void DestroyGameObject()
        {
            //触发未触发的回调
            if (TimeCount < DataObject.callbackFrame * FrameTime)
                InvokeCallback();
            Destroy(gameObject);
        }
    }
}
