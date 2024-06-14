using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class DisplayBattler : MonoBehaviour
    {
        /// <summary>
        /// 自身站位，子弹命中位置
        /// </summary>
        public abstract Vector3 Position { get; }

        /// <summary>
        /// 子弹发射位置
        /// </summary>
        public virtual Vector3 FirePosition => transform.position;

        private GameObject Cursor { get; set; }

        private Dictionary<Static.FrameAnimation, GameObject> PrefabObjectPairs = new();

        public abstract void GoToDie();

        public void ShowCursor(GameObject prefab)
        {
            if (Cursor != null)
                Destroy(Cursor);
            Cursor = Instantiate(prefab, transform);
        }

        public void HideCursor()
        {
            if (Cursor != null)
                Destroy(Cursor);
        }

        /// <summary>
        /// 刷新状态视觉效果
        /// </summary>
        public void RefreshDurationState(List<Dynamic.DurationState> states)
        {
            bool hasState;
            List<Static.FrameAnimation> removedKeys = new();

            //遍历已存在的视觉对象，检查支持状态列表里是否有支撑它们的状态
            foreach (var key in PrefabObjectPairs.Keys)
            {
                hasState = false;
                foreach (var state in states)
                {
                    if (ReferenceEquals(key, state.DisplayObject))
                    {
                        hasState = true;
                        break;
                    }
                }

                if (!hasState)
                {
                    removedKeys.Add(key);
                }
            }

            //销毁无状态支撑的视觉对象
            foreach (var key in removedKeys)
            {
                var obj = PrefabObjectPairs[key];
                PrefabObjectPairs.Remove(key);
                Destroy(obj);
            }

            //添加应有的视觉对象
            foreach (var state in states)
            {
                if (
                    state.DisplayObject != null
                    && !PrefabObjectPairs.ContainsKey(state.DisplayObject)
                )
                {
                    var obj = Instantiate(BattleManager.BattlerStatePrefab, transform);
                    obj.GetComponent<Root.FrameAnimation>().Setup(state.DisplayObject, true, null);
                    PrefabObjectPairs.Add(state.DisplayObject, obj);
                }
            }
        }
    }
}
