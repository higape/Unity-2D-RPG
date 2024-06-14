using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 玩家交互触发器，接收 PlayerController 的请求
    /// </summary>
    [RequireComponent(typeof(AreaBase))]
    [RequireComponent(typeof(RectangleCollider))]
    public abstract class InteractTriggerBase : MonoBehaviour
    {
        private static List<InteractTriggerBase> Triggers { get; set; } = new();

        /// <summary>
        /// 尝试与NPC交互，如果有发生交互，返回true。
        /// </summary>
        public static bool InteractForward(Vector3 position, bool isRemote)
        {
            foreach (var c in Triggers)
            {
                if (
                    c.Area.Overlap(position)
                    && c.Collider.IsContactWithPlayer
                    && (c.allowRemote || !isRemote)
                )
                {
                    c.OnInteract();
                    return true;
                }
            }
            return false;
        }

        public static bool InteractUnder(Vector3 position)
        {
            foreach (var c in Triggers)
            {
                if (c.Area.Overlap(position) && !c.Collider.IsContactWithPlayer)
                {
                    c.OnInteract();
                    return true;
                }
            }
            return false;
        }

        [SerializeField]
        [Tooltip("允许隔着柜台对话")]
        protected bool allowRemote;

        protected AreaBase Area { get; set; }
        protected RectangleCollider Collider { get; set; }

        protected void Awake()
        {
            Area = GetComponent<AreaBase>();
            Collider = GetComponent<RectangleCollider>();
        }

        protected void OnEnable()
        {
            Triggers.Add(this);
        }

        protected void OnDisable()
        {
            Triggers.Remove(this);
        }

        /// <summary>
        /// 被交互后调用的方法。
        /// </summary>
        protected virtual void OnInteract() { }
    }
}
