using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// 将方法添加到事件，使对象具有惯性。
    /// </summary>
    [AddComponentMenu("地图/惯性Setter")]
    public class InertiaSetter : TreadleTriggerBase
    {
        [Tooltip("移动方向")]
        [SerializeField]
        private Mover.DirectionType inertiaDirection;

        [Tooltip("移动速度(秒)")]
        [SerializeField]
        private float inertiaSpeed = 0.25f;

        [Tooltip("强制移动角色")]
        [SerializeField]
        private bool force;

        public Mover.DirectionType InertiaDirection
        {
            get => inertiaDirection;
            set => inertiaDirection = value;
        }

        public float InertiaSpeed
        {
            get => inertiaSpeed;
            set => inertiaSpeed = value;
        }

        public bool Force
        {
            get => force;
            set => force = value;
        }

        private List<(AreaBase, Mover)> CapturedCharacter { get; set; } = new();

        private void Update()
        {
            Rect rect = Area.CurrentRect;
            int i = 0;

            while (i < CapturedCharacter.Count)
            {
                var character = CapturedCharacter[i];

                //如果角色在范围内，赋予惯性
                if (character.Item1.Overlap(rect))
                {
                    SetInertia(character.Item2);
                    i++;
                }
                else
                {
                    CapturedCharacter.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 由事件调用，赋予角色惯性
        /// </summary>
        protected override void OnHandle(GameObject character)
        {
            var mover = character.GetComponent<Mover>();
            SetInertia(mover);
            CapturedCharacter.Add((character.GetComponent<AreaBase>(), mover));
        }

        private void SetInertia(Mover mover)
        {
            mover.InertiaDirection = InertiaDirection;
            mover.InertiaSpeed = InertiaSpeed;
            if (Force)
                mover.IsMoveContinue = true;
        }
    }
}
