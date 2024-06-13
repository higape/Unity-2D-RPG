using System.Collections;
using System.Collections.Generic;
using Battle;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 管理一个子弹的生命周期，从开火、飞行、爆炸到发送回调。
    /// </summary>
    public class BulletProcessor
    {
        public BulletProcessor(
            Static.Bullet dataObject,
            Vector3 startPosition,
            Vector3 endPosition,
            List<Battler> targets,
            UnityAction<List<Battler>> callback
        )
        {
            DataObject = dataObject;
            StartPosition = startPosition;
            EndPosition = endPosition;
            Targets = targets;
            Callback = callback;

            if (StartPosition.x > EndPosition.x)
                ScaleXCFT = 1f;
            else
                ScaleXCFT = -1f;
        }

        private Static.Bullet DataObject { get; set; }
        public Vector3 StartPosition { get; private set; }
        public Vector3 EndPosition { get; private set; }
        private List<Battler> Targets { get; set; }
        private float ScaleXCFT { get; set; }

        /// <summary>
        /// 爆炸时执行的回调，用于通知Usage执行效果。
        /// </summary>
        private UnityAction<List<Battler>> Callback { get; set; }

        public void StartAnimation()
        {
            //播放一个动画
            if (DataObject.readyAnimation != null)
            {
                GameObject instance = BattleManager.CreateAnimation(StartPosition);
                Vector3 scale = instance.transform.localScale;
                instance.transform.localScale = new Vector3(scale.x * ScaleXCFT, scale.y, scale.z);
                instance
                    .GetComponent<FrameAnimation>()
                    .Setup(DataObject.readyAnimation, false, OnReadyTrigger);
            }
            else
            {
                OnReadyTrigger();
            }
        }

        private void OnReadyTrigger()
        {
            //创建一个可以移动的子弹实例
            if (DataObject.flyingAnimation != null)
            {
                GameObject instance = BattleManager.CreateBullet(StartPosition);
                Vector3 scale = instance.transform.localScale;
                instance.transform.localScale = new Vector3(scale.x * ScaleXCFT, scale.y, scale.z);
                instance
                    .GetComponent<FrameAnimation>()
                    .Setup(DataObject.flyingAnimation, true, null);
                instance.GetComponent<Bullet>().Setup(StartPosition, EndPosition, OnFlyingTrigger);
            }
            else
            {
                OnFlyingTrigger();
            }
        }

        private void OnFlyingTrigger()
        {
            //播放一个动画
            if (DataObject.collideAnimation != null)
            {
                GameObject instance = BattleManager.CreateAnimation(EndPosition);
                Vector3 scale = instance.transform.localScale;
                instance.transform.localScale = new Vector3(scale.x * ScaleXCFT, scale.y, scale.z);
                instance
                    .GetComponent<FrameAnimation>()
                    .Setup(DataObject.collideAnimation, false, OnCollideTrigger);
            }
            else
            {
                OnCollideTrigger();
            }
        }

        private void OnCollideTrigger()
        {
            Callback?.Invoke(Targets);
        }
    }
}
