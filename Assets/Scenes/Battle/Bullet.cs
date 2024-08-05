using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    /// <summary>
    /// 使对象从设置的一个位置移动到另一个位置。
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        /// <summary>
        /// 子弹移动速度，单位: Unity单位/秒
        /// </summary>
        public const float BulletSpeed = 30;
        private Vector3 StartPosition { get; set; }
        private Vector3 EndPosition { get; set; }
        private UnityAction Callback { get; set; }
        private float TimeCount { get; set; }
        private float EndTime { get; set; }

        /// <summary>
        /// 每秒移动距离
        /// </summary>
        private Vector3 DistancePerSecond { get; set; }

        private void Update()
        {
            TimeCount += Time.deltaTime;

            if (TimeCount >= EndTime)
                FinishMove();
            else
                transform.position = StartPosition + DistancePerSecond * TimeCount;
        }

        public void Setup(Vector3 start, Vector3 end, UnityAction callback)
        {
            StartPosition = start;
            EndPosition = end;
            Callback = callback;
            EndTime = Mathf.Abs(EndPosition.x - StartPosition.x) / BulletSpeed;

            if (EndTime == 0)
                DistancePerSecond = new Vector3(0, 0, 0);
            else
                DistancePerSecond = (EndPosition - StartPosition) / EndTime;

            transform.position = StartPosition;
        }

        private void FinishMove()
        {
            Callback?.Invoke();
            Destroy(gameObject);
        }
    }
}
