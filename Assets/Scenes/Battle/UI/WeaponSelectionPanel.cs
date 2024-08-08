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

        [SerializeField]
        private GameObject selectedLayer;

        [SerializeField]
        private ListBox selectedListBox;

        private Actor CurrentActor { get; set; }
        private int CurrentQuantity { get; set; }
        private List<ActorWeapon> SelectedItems { get; set; }
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

        public void Setup(
            Actor actor,
            UnityAction cancelCallback,
            UnityAction finishCallback,
            int quantity = 1
        )
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            CurrentQuantity = quantity;
            var wl = CurrentActor.GetBattleWeapons();
            listBox.Initialize(1, wl.Count, RefreshItem, wl);
            SelectedItems = new();
            BattleManager.CurrentCommand.SelectedItems = new();

            if (CurrentQuantity > 1)
            {
                selectedListBox.Initialize(1, CurrentQuantity, RefreshSelectedItem, SelectedItems);
                selectedLayer.SetActive(true);
            }
            else
            {
                selectedLayer.SetActive(false);
            }
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is ActorWeapon item)
                {
                    c.textComponent0.text = item.Name;
                    if (item.IsCooling)
                        c.textComponent1.text = string.Format(
                            ResourceManager.Term.coolingTimeStatement,
                            item.CurrentCoolingTime
                        );
                    else
                        c.textComponent1.text = string.Empty;
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = string.Empty;
                }
            }
        }

        private void RefreshSelectedItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
                c.textComponent.text = data is ActorWeapon item ? item.Name : " ";
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

            //检查是否重复选择
            foreach (var selected in SelectedItems)
            {
                if (selected.ID == weapon.ID)
                    return;
            }
            SelectedItems.Add(weapon);
            BattleManager.CurrentCommand.SelectedItems.Add(new(weapon, weapon.GetUsage(0)));

            //选择的数量不够，先刷新列表
            if (SelectedItems.Count < CurrentQuantity)
            {
                selectedListBox.Refresh();
                return;
            }

            //数量足够，开始选择目标
            var usage = SelectedItems[0].GetUsage(0);
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
                                OnTargetPanelCancel,
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
                            OnTargetPanelCancel,
                            InvokeFinishCallback
                        );
                    break;
                default:
                    BattleManager.CurrentCommand.SelectedTarget = BattleManager.BestTarget(
                        CurrentActor,
                        usage.scope
                    );
                    BattleManager.CommandInputEnd();
                    InvokeFinishCallback();
                    break;
            }

            canvasGroup.alpha = 0;
        }

        private void OnTargetPanelCancel()
        {
            SelectedItems.Clear();
            selectedListBox.Refresh();
            BattleManager.CurrentCommand.SelectedItems.Clear();
            canvasGroup.alpha = 1;
        }

        public void Cancel()
        {
            if (SelectedItems.Count > 0)
            {
                SelectedItems.RemoveAt(SelectedItems.Count - 1);
                selectedListBox.Refresh();
                return;
            }
            BattleManager.CurrentCommand.SelectedItems.Clear();
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        public void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
