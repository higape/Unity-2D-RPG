using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 矩形碰撞器，碰撞范围可以是点或矩形。
    /// </summary>
    [AddComponentMenu("地图/矩形碰撞器")]
    [RequireComponent(typeof(AreaBase))]
    public class RectangleCollider : MonoBehaviour
    {
        /// <summary>
        /// [self, other]
        /// true表示有碰撞
        /// </summary>
        private static readonly bool[,] CollisionMatrix = new bool[5, 5]
        {
            { false, false, false, false, false },
            { false, false, false, false, true },
            { false, false, false, false, true },
            { false, false, false, false, true },
            { false, true, true, true, true }
        };

        /// <summary>
        /// 用于判断能否通过障碍物和地形
        /// </summary>
        public enum ColliderType
        {
            None = 0,
            Player = 1,
            PlayerBoat = 2,
            PlayerShip = 3,
            People = 4,
        }

        /// <summary>
        /// 推动时间倍数
        /// </summary>
        public const float PushTimeRate = 2f;

        private static List<RectangleCollider> Colliders { get; set; } = new();

        [SerializeField]
        private ColliderType type = ColliderType.People;

        [SerializeField]
        private bool pushable = false;

        public ColliderType Type
        {
            get => type;
            set => type = value;
        }

        public bool IsContactWithPlayer =>
            CollisionMatrix[(int)Type, (int)ColliderType.Player]
            || CollisionMatrix[(int)Type, (int)ColliderType.PlayerBoat]
            || CollisionMatrix[(int)Type, (int)ColliderType.PlayerShip];

        private AreaBase Area { get; set; }

        private void Awake()
        {
            Area = GetComponent<AreaBase>();
        }

        /// <summary>
        /// 重写时记得调用
        /// </summary>
        protected void OnEnable()
        {
            Colliders.Add(this);
        }

        /// <summary>
        /// 重写时记得调用
        /// </summary>
        protected void OnDisable()
        {
            Colliders.Remove(this);
        }

        /// <summary>
        /// 检查是否与其它碰撞器接触，触发效果。
        /// </summary>
        /// <returns>true-有接触。</returns>
        public bool Contact(Vector3 newPosition, bool enableContact)
        {
            Rect rect =
                new()
                {
                    x = newPosition.x,
                    y = newPosition.y,
                    width = Area.Width,
                    height = Area.Height
                };

            foreach (var c in Colliders)
            {
                if (ReferenceEquals(this, c))
                    continue;
                if (c.Area.Overlap(rect) && CollisionMatrix[(int)Type, (int)c.Type])
                {
                    if (enableContact)
                        c.ProcessContact(gameObject);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 接触时触发的效果。
        /// 不要在此方法中添加或移除碰撞器。
        /// </summary>
        private void ProcessContact(GameObject character)
        {
            if (pushable)
                OnPush(character);
        }

        /// <summary>
        /// 用惯性实现推动
        /// </summary>
        private void OnPush(GameObject character)
        {
            if (
                this.TryGetComponent<Mover>(out var myMover)
                && character.TryGetComponent<Mover>(out var itsMover)
            )
            {
                //备份移动器状态
                float reserveSpeed = myMover.Speed;
                Mover.DirectionType reserveInertiaDirection = myMover.InertiaDirection;
                float reserveInertiaSpeed = myMover.InertiaSpeed;
                //赋予状态
                myMover.Speed = itsMover.Speed;
                myMover.InertiaDirection = Mover.DirectionTurnBack(itsMover.Direction);
                myMover.InertiaSpeed = itsMover.Speed * PushTimeRate;
                //尝试移动
                if (myMover.TryMove(itsMover.Direction))
                {
                    //移动成功
                    itsMover.InertiaDirection = myMover.InertiaDirection;
                    itsMover.InertiaSpeed = myMover.InertiaSpeed;
                    itsMover.ReadyMove(itsMover.Direction);
                }
                //还原移动器状态
                myMover.Speed = reserveSpeed;
                myMover.InertiaDirection = reserveInertiaDirection;
                myMover.InertiaSpeed = reserveInertiaSpeed;
            }
        }
    }
}
