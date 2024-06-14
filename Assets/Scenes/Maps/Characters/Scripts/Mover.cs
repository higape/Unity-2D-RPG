using System;
using System.Collections.Generic;
using Static;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 根据指令可以将对象向指定方向移动一单位。
    /// 收到多个指令时，最后的指令有效。
    /// 未结束的指令不会被打断（少数情况除外）。
    /// </summary>
    [AddComponentMenu("地图/移动器")]
    [RequireComponent(typeof(RectangleCollider))]
    public sealed class Mover : MonoBehaviour
    {
        public static Vector3 DirectionToVector3(DirectionType direction) =>
            direction switch
            {
                DirectionType.Right => Vector3.right,
                DirectionType.UpRight => new Vector3(1, 1, 0),
                DirectionType.Up => Vector3.up,
                DirectionType.UpLeft => new Vector3(-1, 1, 0),
                DirectionType.Left => Vector3.left,
                DirectionType.DownLeft => new Vector3(-1, -1, 0),
                DirectionType.Down => Vector3.down,
                DirectionType.DownRight => new Vector3(1, -1, 0),
                _ => Vector3.zero,
            };

        public static DirectionType Vector3ToDirection(Vector3 vector)
        {
            // 4 direction
            if (vector.x > 0)
                return DirectionType.Right;
            else if (vector.x < 0)
                return DirectionType.Left;
            else if (vector.y > 0)
                return DirectionType.Up;
            else
                return DirectionType.Down;

            // 8 direction
            /* if (vector.y > 0)
            {
                if (vector.x > 0)
                    return DirectionType.UpRight;
                else if (vector.x < 0)
                    return DirectionType.UpLeft;
                else
                    return DirectionType.Up;
            }
            else if (vector.y < 0)
            {
                if (vector.x > 0)
                    return DirectionType.DownRight;
                else if (vector.x < 0)
                    return DirectionType.DownLeft;
                else
                    return DirectionType.Down;
            }
            else
            {
                if (vector.x > 0)
                    return DirectionType.Right;
                else if (vector.x < 0)
                    return DirectionType.Left;
            }

            Debug.LogWarning("矢量未能正确转换成方向枚举值");
            return DirectionType.Down; */
        }

        public static DirectionType DirectionTurnBack(DirectionType direction)
        {
            int newDirection = (int)direction + 4;
            if (newDirection >= 8)
                newDirection -= 8;
            return (DirectionType)newDirection;
        }

        public static DirectionType RandomDirection()
        {
            int direction = UnityEngine.Random.Range(0, 4) * 2;
            return (DirectionType)direction;
        }

        public enum DirectionType
        {
            [InspectorName("→")]
            Right = 0,

            [InspectorName("↗")]
            UpRight = 1,

            [InspectorName("↑")]
            Up = 2,

            [InspectorName("↖")]
            UpLeft = 3,

            [InspectorName("←")]
            Left = 4,

            [InspectorName("↙")]
            DownLeft = 5,

            [InspectorName("↓")]
            Down = 6,

            [InspectorName("↘")]
            DownRight = 7,
        }

        private class MovePlan
        {
            public DirectionType direction;
            public bool isThrough;
            // public bool isTry;
        }

        public const float MaxMoveTime = 1f;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [Tooltip("行走图")]
        [SerializeField]
        private CharacterSkin spriteSkin;

        [Tooltip("原地踏步")]
        [SerializeField]
        private bool step;

        [Tooltip("角色的方向")]
        [SerializeField]
        private DirectionType direction = DirectionType.Down;

        [Tooltip("移动速度(单位/秒)")]
        [SerializeField]
        private float speed = 2f;

        [Tooltip("启用活动范围")]
        [SerializeField]
        private bool enableMoveScope;

        [Tooltip("活动范围会限制角色的活动区域，跟随者不受限制")]
        [SerializeField]
        private Rect moveScope;

        [Tooltip("跟随者必须跟着此角色，不能自由活动")]
        [SerializeField]
        private Mover follower;

        [SerializeField]
        private bool enableMoveRoute;

        [SerializeField]
        private MoveCommandSet moveRoute;

        private float realMoveTime;
        private bool isSteping;

        public SpriteRenderer Renderer => spriteRenderer;

        /// <summary>
        /// 用于显示的 Sprite 集合
        /// </summary>
        public CharacterSkin Skin
        {
            get => spriteSkin;
            set
            {
                spriteSkin = value;
                IsNeedRefresh = true;
            }
        }

        public bool Step
        {
            get => step;
            set
            {
                if (step != value)
                {
                    step = value;
                    IsSteping = false;
                }
            }
        }

        /// <summary>
        /// 角色的方向
        /// </summary>
        public DirectionType Direction
        {
            get => direction;
            set
            {
                direction = value;
                IsNeedRefresh = true;
            }
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public bool EnableMoveScope
        {
            get => enableMoveScope;
            set => enableMoveScope = value;
        }

        public MoveCommandSet OriginalMoveRoute => moveRoute;

        /// <summary>
        /// 需要继续移动，为true时拒绝其它移动指令
        /// </summary>
        public bool IsMoveContinue { get; set; }

        /// <summary>
        /// 角色正在移动
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// 角色正在踏步，用于视觉演出
        /// </summary>
        private bool IsSteping
        {
            get => isSteping;
            set
            {
                if (isSteping != value)
                {
                    isSteping = value;
                    IsNeedRefresh = true;
                }
            }
        }

        /// <summary>
        /// 是否需要刷新视觉演出
        /// </summary>
        private bool IsNeedRefresh { get; set; }

        /// <summary>
        /// 角色是否可以立即移动
        /// </summary>
        public bool CanMove => !(IsMoving || IsMoveContinue);

        /// <summary>
        /// 设置为true将只接受跟随的移动指令
        /// </summary>
        public bool IsFollow { get; set; }

        public Mover Follower
        {
            get => follower;
            set => follower = value;
        }

        /// <summary>
        /// 惯性方向(惯性会影响移动)
        /// </summary>
        public DirectionType InertiaDirection { get; set; }

        /// <summary>
        /// 惯性速度(单位/秒)
        /// </summary>
        public float InertiaSpeed { get; set; }

        /// <summary>
        /// 移动一单位所需的时间，每次移动时重新计算
        /// </summary>
        public float MoveTime
        {
            get => realMoveTime;
            private set => realMoveTime = Math.Clamp(value, 0, MaxMoveTime);
        }

        /// <summary>
        /// 更新角色动作的时机
        /// </summary>
        private float StepTime { get; set; }

        /// <summary>
        /// 更新角色动作的时机
        /// </summary>
        private float RecoverTime { get; set; }

        /// <summary>
        /// 计时
        /// </summary>
        private float TimeCount { get; set; }

        /// <summary>
        /// 每秒移动向量
        /// </summary>
        private Vector3 TransitionPerSecond { get; set; }

        /// <summary>
        /// 角色的位置
        /// </summary>
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        /// <summary>
        /// 上一个位置
        /// </summary>
        public Vector3 LastPosition { get; set; }

        /// <summary>
        /// 角色前方一格位置
        /// </summary>
        public Vector3 ForwardPosition => Position + DirectionToVector3(Direction);

        /// <summary>
        /// 角色前方两格位置
        /// </summary>
        public Vector3 ForwardTwoPosition => Position + DirectionToVector3(Direction) * 2;

        /// <summary>
        /// 最后收到的移动指令。
        /// 收到多个指令时，只执行最后的指令。
        /// </summary>
        private MovePlan LastCommand { get; set; }

        public MoveCommandInterpreter CurrentMoveRoute { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public float WaitCount { get; set; }

        private RectangleCollider Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<RectangleCollider>();
            ResetMoveRoute();
        }

        public void ResetMoveRoute()
        {
            if (enableMoveRoute && moveRoute.commands.Length > 0)
            {
                CurrentMoveRoute = new(
                    this,
                    moveRoute.commands,
                    moveRoute.repeat,
                    moveRoute.skippable,
                    null
                );
            }
            else
            {
                CurrentMoveRoute = null;
            }
        }

        private void OnEnable()
        {
            if (follower != null)
            {
                follower.IsFollow = true;
            }
        }

        private void OnDisable()
        {
            if (follower != null)
            {
                follower.IsFollow = false;
            }

            if (IsMoving)
            {
                EndMove();
            }
        }

        private void Start()
        {
            LastPosition = transform.position;
        }

        private void Update()
        {
            if (CommandInterpreter.Pause)
                return;

            //如果没事做
            if (
                !(
                    UpdateMove()
                    || RouteMove()
                    || MoveByLastCommand()
                    || InertiaMove()
                    || UpdateStepAnimation()
                )
            )
                //重置计时
                TimeCount = 0;

            //检查刷新
            if (IsNeedRefresh)
                RefreshRenderer();
        }

        private bool UpdateMove()
        {
            if (IsMoving)
            {
                TimeCount += Time.deltaTime;
                //更新图像位置
                spriteRenderer.transform.Translate(TransitionPerSecond * Time.deltaTime);

                //移动结束
                if (TimeCount >= MoveTime)
                    EndMove();
                //更新走路动画
                else if (TimeCount >= RecoverTime)
                    IsSteping = false;
                //更新走路动画
                else if (TimeCount >= StepTime)
                    IsSteping = true;

                return true;
            }
            return false;
        }

        private void EndMove()
        {
            TimeCount = 0;
            IsMoving = false;
            IsSteping = false;
            InertiaDirection = Direction;
            spriteRenderer.transform.position = Position;

            if (!IsFollow)
            {
                //应用地形效果
                IsMoveContinue = TerrainManager.GetMoveContinue(Position);
                InertiaSpeed = TerrainManager.GetInertiaSpeed(Position);

                //触发踩踏
                TreadleTriggerBase.Tread(Position, gameObject);
            }
        }

        private bool UpdateStepAnimation()
        {
            if (Step)
            {
                TimeCount += Time.deltaTime;

                if (TimeCount >= 1 / Speed)
                {
                    TimeCount = 0;
                    IsSteping = !IsSteping;
                }

                return true;
            }

            return false;
        }

        public bool MoveInNoTime(DirectionType direction)
        {
            if (CanMove && !IsFollow)
            {
                Direction = direction;

                //检查地形通行和碰撞
                if (
                    TerrainManager.CanPass(ForwardPosition, Collider.Type)
                    && !Collider.Contact(ForwardPosition, true)
                )
                {
                    ExecuteMove();
                }

                return IsMoving;
            }
            return false;
        }

        /// <summary>
        /// 准备移动，同时出现多个指令时，最后的指令有效。
        /// </summary>
        public void ReadyMove(DirectionType direction)
        {
            if (CanMove)
                LastCommand = new() { direction = direction, };
        }

        /// <summary>
        /// 按最后的指令移动。
        /// </summary>
        private bool MoveByLastCommand()
        {
            if (LastCommand != null && CanMove && !IsFollow)
            {
                Direction = LastCommand.direction;
                LastCommand = null;

                //检查地形通行和碰撞
                if (
                    TerrainManager.CanPass(ForwardPosition, Collider.Type)
                    && !Collider.Contact(ForwardPosition, true)
                )
                {
                    ExecuteMove();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试移动
        /// </summary>
        /// <returns>是否移动成功</returns>
        public bool TryMove(DirectionType direction)
        {
            if (CanMove && !IsFollow)
            {
                Vector3 newPosition = Position + DirectionToVector3(direction);

                //检查移动范围
                if (enableMoveScope && !moveScope.Contains(newPosition))
                {
                    return false;
                }

                //检查地形通行和碰撞，不触发碰撞事件
                if (
                    TerrainManager.CanPass(newPosition, Collider.Type)
                    && !Collider.Contact(newPosition, false)
                )
                {
                    //确定移动成功才刷新方向
                    Direction = direction;
                    ExecuteMove();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 惯性移动
        /// </summary>
        private bool InertiaMove()
        {
            if (InertiaSpeed != 0 && !IsFollow)
            {
                //控制角色动画
                IsMoveContinue = true;
                //刷新角色方向
                Direction = InertiaDirection;
                //检查地形碰撞和角色碰撞
                if (
                    TerrainManager.CanPass(ForwardPosition, Collider.Type)
                    && !Collider.Contact(ForwardPosition, true)
                )
                {
                    //更新角色位置，并且图像留在原地
                    Vector3 sp = spriteRenderer.transform.position;
                    LastPosition = Position;
                    Position = ForwardPosition;
                    spriteRenderer.transform.position = sp;

                    LastCommand = null;
                    TimeCount = 0;
                    IsMoving = true;
                    float moveSpeedRate = TerrainManager.GetMoveSpeedRate(Position);

                    MoveTime = 1f / (InertiaSpeed * moveSpeedRate);

                    //设置动画
                    TransitionPerSecond = (Position - spriteRenderer.transform.position) / MoveTime;
                    StepTime = MoveTime / 4;
                    RecoverTime = StepTime * 3;
                    //移动跟随者
                    if (follower != null)
                    {
                        follower.Follow(LastPosition, IsMoveContinue, MoveTime);
                    }
                }
                else
                {
                    //取消惯性和持续移动状态
                    IsMoveContinue = false;
                    InertiaSpeed = 0;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 跟随。以同样速度被强行拖走。
        /// </summary>
        public void Follow(Vector3 newPosition, bool isMoveContinue, float time)
        {
            if (IsFollow && newPosition != Position)
            {
                //控制行走动画
                IsMoveContinue = isMoveContinue;
                //计算方向
                Direction = Vector3ToDirection(newPosition - Position);

                //更新角色位置，并且图像留在原地
                Vector3 sp = spriteRenderer.transform.position;
                LastPosition = Position;
                Position = newPosition;
                spriteRenderer.transform.position = sp;

                LastCommand = null;
                TimeCount = 0;
                IsMoving = true;

                MoveTime = time;
                //设置动画
                TransitionPerSecond = (Position - spriteRenderer.transform.position) / MoveTime;
                StepTime = MoveTime / 4;
                RecoverTime = StepTime * 3;

                //移动跟随者
                if (follower != null)
                {
                    follower.Follow(LastPosition, IsMoveContinue, MoveTime);
                }
            }
        }

        private void ExecuteMove()
        {
            //更新角色位置，并且图像留在原地
            Vector3 sp = spriteRenderer.transform.position;
            LastPosition = Position;
            Position = ForwardPosition;
            spriteRenderer.transform.position = sp;

            LastCommand = null;
            TimeCount = 0;
            IsMoving = true;
            MoveTime = CalculateMoveTime();

            //设置动画
            TransitionPerSecond = (Position - spriteRenderer.transform.position) / MoveTime;
            StepTime = MoveTime / 4;
            RecoverTime = StepTime * 3;
            //移动跟随者
            if (follower != null)
            {
                follower.Follow(LastPosition, IsMoveContinue, MoveTime);
            }
        }

        public float CalculateMoveTime()
        {
            float moveTime;
            float moveSpeedRate = TerrainManager.GetMoveSpeedRate(Position);

            if (InertiaSpeed == 0)
            {
                moveTime = 1f / (Speed * moveSpeedRate);
            }
            else if (Direction == InertiaDirection)
            {
                moveTime = 1f / ((Speed + InertiaSpeed) * moveSpeedRate);
            }
            else if (Speed > InertiaSpeed)
            {
                moveTime = 1f / ((Speed - InertiaSpeed) * moveSpeedRate);
            }
            else
            {
                moveTime = MaxMoveTime;
            }

            return moveTime;
        }

        public void SetMoveScope(Rect rect)
        {
            moveScope = rect;
        }

        public void SetMoveRoute(
            IList<CommandSet.Command> commandSet,
            bool repeat,
            bool skippable,
            UnityAction callback
        )
        {
            //如果有已存在但未完成的移动路径，执行其回调然后舍弃
            CurrentMoveRoute?.Callback?.Invoke();
            WaitCount = 0;
            CurrentMoveRoute = new(this, commandSet, repeat, skippable, callback);
            enableMoveRoute = true;
        }

        /// <summary>
        /// 按照路径移动
        /// </summary>
        private bool RouteMove()
        {
            if (enableMoveRoute && CurrentMoveRoute != null && CanMove && !IsFollow)
            {
                if (WaitCount > 0)
                {
                    WaitCount -= Time.deltaTime;
                    if (WaitCount >= 0)
                    {
                        return true;
                    }
                    else if (CurrentMoveRoute.CurrentIndex >= CurrentMoveRoute.Commands.Count)
                    {
                        CurrentMoveRoute.EndRouteMove();
                    }
                }

                if (CurrentMoveRoute != null)
                {
                    CurrentMoveRoute.ProcessMoveCommand();
                    return true;
                }
            }
            return false;
        }

        public void LookAtPlayer()
        {
            Direction = Vector3ToDirection(PlayerParty.PlayerPosition - Position);
            RefreshRenderer();
        }

        public void BackToPlayer()
        {
            Direction = Vector3ToDirection(Position - PlayerParty.PlayerPosition);
            RefreshRenderer();
        }

        /// <summary>
        /// 刷新渲染器
        /// </summary>
        private void RefreshRenderer()
        {
            Sprite skinSprite;
            if (IsSteping && (!IsMoveContinue || (Step && !IsMoving)))
            {
                skinSprite = Direction switch
                {
                    DirectionType.Right or DirectionType.UpRight => Skin.walkingRight,
                    DirectionType.Up or DirectionType.UpLeft => Skin.walkingUp,
                    DirectionType.Left or DirectionType.DownLeft => Skin.walkingLeft,
                    DirectionType.Down or DirectionType.DownRight => Skin.walkingDown,
                    _ => null,
                };
            }
            else
            {
                skinSprite = Direction switch
                {
                    DirectionType.Right or DirectionType.UpRight => Skin.idleRight,
                    DirectionType.Up or DirectionType.UpLeft => Skin.idleUp,
                    DirectionType.Left or DirectionType.DownLeft => Skin.idleLeft,
                    DirectionType.Down or DirectionType.DownRight => Skin.idleDown,
                    _ => null,
                };
            }
            spriteRenderer.sprite = skinSprite;
            IsNeedRefresh = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (
                enableMoveScope
                && ReferenceEquals(UnityEditor.Selection.activeGameObject, gameObject)
            )
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(
                    new Vector2(
                        moveScope.x + moveScope.width / 2f - 0.5f,
                        moveScope.y + moveScope.height / 2f - 0.5f
                    ),
                    new Vector2(moveScope.width, moveScope.height)
                );
            }
        }

        /// <summary>
        /// 根据图像和方向设置渲染器的初始图像
        /// </summary>
        [ContextMenu("刷新渲染图像")]
        public void SetupSpriteRenderer()
        {
            if (Application.isEditor)
            {
                if (spriteRenderer != null && Skin != null)
                {
                    spriteRenderer.sprite = Direction switch
                    {
                        DirectionType.Right or DirectionType.UpRight => Skin.idleRight,
                        DirectionType.Up or DirectionType.UpLeft => Skin.idleUp,
                        DirectionType.Left or DirectionType.DownLeft => Skin.idleLeft,
                        DirectionType.Down or DirectionType.DownRight => Skin.idleDown,
                        _ => null,
                    };
                }
                else
                {
                    Debug.LogWarning("刷新渲染图像失败。");
                }
            }
        }
#endif
    }
}
