using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 踩踏触发器
    /// </summary>
    [RequireComponent(typeof(AreaBase))]
    public abstract class TreadleTriggerBase : MonoBehaviour
    {
        private static List<TreadleTriggerBase> Triggers { get; set; } = new();

        public static void Tread(Vector3 newPosition, GameObject character)
        {
            foreach (var c in Triggers)
            {
                if (
                    c.Area.Overlap(newPosition)
                    && (
                        !c.onlyPlayer
                        || (c.onlyPlayer && ReferenceEquals(PlayerParty.Player, character))
                    )
                )
                {
                    c.OnHandle(character);
                }
            }
        }

        [Tooltip("仅玩家可以触发")]
        [SerializeField]
        private bool onlyPlayer = true;

        public AreaBase Area { get; private set; }

        protected void Awake()
        {
            Area = GetComponent<AreaBase>();
        }

        protected void OnEnable()
        {
            Triggers.Add(this);
        }

        protected void OnDisable()
        {
            Triggers.Remove(this);
        }

        protected virtual void OnHandle(GameObject character) { }
    }
}
