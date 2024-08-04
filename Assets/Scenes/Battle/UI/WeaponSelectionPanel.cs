using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    /// <summary>
    /// 选择武器的面板
    /// </summary>
    public class WeaponSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private ActorWeaponStatistic itemStatistic;

        [SerializeField]
        private GameObject actorPanelPrefab;

        [SerializeField]
        private GameObject enemyPanelPrefab;

        private Actor CurrentActor { get; set; }
        private UnityAction CancelCallback { get; set; }
        private UnityAction FinishCallback { get; set; }
        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            listBox.RegisterSelectedItemChangeCallback(
                (data, index) => itemStatistic.Refresh(data as ActorWeapon)
            );
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(WeaponSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(WeaponSelectionPanel));
        }

        public void Setup(Actor actor, UnityAction cancelCallback, UnityAction finishCallback)
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            var wl = actor.GetBattleWeapons();
            listBox.Initialize(1, wl.Count, RefreshItem, wl);
        }

        private void Interact()
        {
            if (listBox.SelectedItem is not ActorWeapon weapon)
                return;

            if (weapon.IsCooling)
            {
                UIManager.StartMessage(ResourceManager.Term.promptWeaponIsCooling, null);
                return;
            }

            var usage = weapon.GetUsage(0);
            BattleManager.CurrentCommand.SelectedItems = new() { new(weapon, usage) };

            switch (usage.scope)
            {
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriend:
                case Static.UsedScope.AllFriendExcludeSelf:
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    var actorTargets = BattleManager.GetActorToActorTargets(
                        CurrentActor,
                        usage.scope
                    );
                    if (actorTargets.Length > 0)
                    {
                        UIManager
                            .Instantiate(actorPanelPrefab)
                            .GetComponent<ActorSelectionPanel>()
                            .Setup(
                                CurrentActor,
                                actorTargets,
                                usage.scope,
                                () => canvasGroup.alpha = 1,
                                InvokeFinishCallback
                            );
                    }
                    else
                    {
                        //提示没有可选目标
                        UIManager.StartMessage(ResourceManager.Term.promptNoSelectableTarget, null);
                    }
                    break;
                case Static.UsedScope.OneEnemy:
                case Static.UsedScope.AllEnemy:
                case Static.UsedScope.SmallSector:
                case Static.UsedScope.BigSector:
                case Static.UsedScope.SmallRay:
                case Static.UsedScope.BigRay:
                case Static.UsedScope.SmallCircle:
                case Static.UsedScope.BigCircle:
                    UIManager
                        .Instantiate(enemyPanelPrefab)
                        .GetComponent<EnemySelectionPanel>()
                        .Setup(
                            CurrentActor,
                            usage.scope,
                            () => canvasGroup.alpha = 1,
                            InvokeFinishCallback
                        );
                    break;
                default:
                    BattleManager.CurrentCommand.SelectedTarget = BattleManager.BestTarget(
                        CurrentActor,
                        usage.scope
                    );
                    InvokeFinishCallback();
                    break;
            }

            canvasGroup.alpha = 0;
        }

        public void Cancel()
        {
            BattleManager.CurrentCommand.SelectedItems = null;
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        public void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                c.textComponent.text = (data as ActorWeapon).Name;
            }
        }
    }
}
