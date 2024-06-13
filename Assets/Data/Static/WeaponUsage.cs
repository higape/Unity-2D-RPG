using System;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    [Serializable]
    public class WeaponUsage : NameItem
    {
        /// <summary>
        /// 使用数组是为了允许花样发射子弹
        /// </summary>
        [SerializeField]
        private Bullet[] bullets;

        public ElementType element;

        public UsedScope scope;

        /// <summary>
        /// 攻击次数，也作为弹药消耗数量
        /// </summary>
        public int attackCount;

        public int waitTime;

        public BattleEffectData[] effects;

        public string log;

        public string Log => log;

        /// <summary>
        /// 获取子弹数据
        /// </summary>
        /// <param name="emitCount">此参数允许连续发射不同外观的子弹</param>
        public Bullet GetBullet(int emitCount)
        {
            while (emitCount > bullets.Length)
                emitCount -= bullets.Length;
            if (emitCount >= 0)
                return bullets[emitCount];
            Debug.LogError($"索引值超出范围,值:{emitCount}");
            return null;
        }
    }
}
