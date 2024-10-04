using Root;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    /// <summary>
    /// 玩家控制器。
    /// 仅控制地图角色。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// 大于0表示有 Action 需要玩家等待。
        /// 由NPC行为系统控制此值以实现玩家在剧情中不能操作角色。
        /// </summary>
        public static int WaitCount { get; set; } = 0;

        private static Mover PlayerMover => PlayerParty.PlayerMover;

        [SerializeField]
        private GameObject mainMenuPrefab;

        private void OnEnable()
        {
            InputManagementSystem.AddAction(nameof(PlayerController), UpdateInput);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveAction(nameof(PlayerController));
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        private void UpdateInput()
        {
            if (!CommandInterpreter.Pause && WaitCount == 0)
            {
                if (!PlayerMover.IsMoveContinue)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        InteractHandler();
                        return;
                    }
                    else if (Input.GetButtonDown("Cancel"))
                    {
                        CancelHandler();
                        return;
                    }
                    else if (Input.GetButtonDown("MainMenu"))
                    {
                        MainMenuHandler();
                        return;
                    }
                    else if (Input.GetButtonDown("SubMenu"))
                    {
                        SubMenuHandler();
                        return;
                    }
                }

                if (PlayerMover.CanMove)
                {
                    float h = Input.GetAxis("Horizontal");
                    if (h != 0)
                    {
                        HorizontalHandler(h);
                        return;
                    }

                    float v = Input.GetAxis("Vertical");
                    if (v != 0)
                    {
                        VerticalHandler(v);
                        return;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        private void OnSceneChange(Scene current, Scene next)
        {
#if UNITY_EDITOR
            if (WaitCount != 0)
                Debug.LogWarning($"切换场景时要求玩家等待计数为:{WaitCount}");
#endif
            WaitCount = 0;
        }

        protected void VerticalHandler(float axisValue)
        {
            if (axisValue > 0)
            {
                if (PlayerMover.CanMove)
                    PlayerMover.ReadyMove(Mover.DirectionType.Up);
            }
            else if (axisValue < 0)
            {
                if (PlayerMover.CanMove)
                    PlayerMover.ReadyMove(Mover.DirectionType.Down);
            }
        }

        protected void HorizontalHandler(float axisValue)
        {
            if (axisValue < 0)
            {
                if (PlayerMover.CanMove)
                    PlayerMover.ReadyMove(Mover.DirectionType.Left);
            }
            else if (axisValue > 0)
            {
                if (PlayerMover.CanMove)
                    PlayerMover.ReadyMove(Mover.DirectionType.Right);
            }
        }

        protected bool InteractHandler()
        {
            if (PlayerMover.CanMove)
            {
                //与前方交互
                if (InteractTriggerBase.InteractForward(PlayerMover.ForwardPosition, false))
                {
                    return true;
                }

                //如果有柜台，再尝试往前交互
                if (
                    TerrainManager.IsCounter(PlayerMover.ForwardPosition)
                    && InteractTriggerBase.InteractForward(PlayerMover.ForwardTwoPosition, true)
                )
                {
                    return true;
                }

                //与自己所在位置交互
                if (InteractTriggerBase.InteractUnder(PlayerMover.Position))
                {
                    return true;
                }
            }

            return false;
        }

        protected bool CancelHandler()
        {
            return true;
        }

        protected bool MainMenuHandler()
        {
            UIManager
                .Instantiate(mainMenuPrefab)
                .GetComponent<UI.MainMenuPanel>()
                .Setup(() => CommandInterpreter.Pause = false);
            CommandInterpreter.Pause = true;
            return true;
        }

        protected bool SubMenuHandler()
        {
            return true;
        }
    }
}
