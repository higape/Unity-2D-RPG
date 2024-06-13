using System;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 周期效果（持续效果）
    /// </summary>
    [Serializable]
    public class DurationState : BattleEffect
    {
        /// <summary>
        /// 元素类型
        /// </summary>
        public ElementType elementType;

        /// <summary>
        /// 优势，自身销毁别的状态
        /// </summary>
        [SerializeField]
        private int[] strengthIDs;

        /// <summary>
        /// 劣势，自身被别的状态销毁
        /// </summary>
        [SerializeField]
        private int[] weaknessIDs;

        /// <summary>
        /// 视觉对象
        /// </summary>
        public FrameAnimation displayObject;

        [NonSerialized]
        private List<DurationState> strengthList;

        public List<DurationState> StrengthList
        {
            get
            {
                if (strengthList == null)
                {
                    strengthList = new(strengthIDs.Length);
                    foreach (var id in strengthIDs)
                    {
                        strengthList.Add(Root.ResourceManager.DurationState.GetItem(id));
                    }
                }
                return strengthList;
            }
        }

        [NonSerialized]
        private List<DurationState> weaknessList;

        public List<DurationState> WeaknessList
        {
            get
            {
                if (weaknessList == null)
                {
                    weaknessList = new(weaknessIDs.Length);
                    foreach (var id in weaknessIDs)
                    {
                        weaknessList.Add(Root.ResourceManager.DurationState.GetItem(id));
                    }
                }
                return weaknessList;
            }
        }

        public bool IsStrongerThan(DurationState effect)
        {
            foreach (var i in StrengthList)
            {
                if (i == effect)
                    return true;
            }
            return false;
        }

        public bool IsWeakerThan(DurationState effect)
        {
            foreach (var i in WeaknessList)
            {
                if (i == effect)
                    return true;
            }
            return false;
        }
    }
}
