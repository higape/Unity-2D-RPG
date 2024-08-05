using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UIT = Static.ActorUsableItem.ItemType;

namespace Battle
{
    /// <summary>
    /// 选择武器的面板
    /// </summary>
    public class ItemSelectionPanel : MonoBehaviour
    {
        private delegate ActorUsableItem GetItemByID(int id);

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorUsableItemStatistic itemStat;

        [SerializeField]
        private GameObject actorPanelPrefab;

        [SerializeField]
        private GameObject enemyPanelPrefab;

        private GetItemByID CurrentAction { get; set; }

        private Actor CurrentActor { get; set; }

        private UnityAction CancelCallback { get; set; }

        private UnityAction FinishCallback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, itemListBox.SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, itemListBox.SelectRight),
                new(InputCommand.ButtonPrevious, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ItemSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ItemSelectionPanel));
        }

        public void Setup(
            Actor actor,
            UnityAction cancelCallback,
            UnityAction finishCallback,
            UIT itemType
        )
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            CurrentAction = (id) => new ActorUsableItem(itemType, id);
            heading.text = ResourceManager.Term.GetText(itemType);
            itemListBox.SetSource(Party.GetActorItemListInBattle(itemType));
            BattleManager.CurrentCommand.SelectedItems = new();
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem3 c)
            {
                if (data is QuantityList.ListItem quantityItem)
                {
                    var item = CurrentAction(quantityItem.id);
                    c.textComponent0.text = item.Name;
                    c.textComponent1.text = quantityItem.quantity.ToString();
                    if (item.IsCooling)
                        c.textComponent2.text = string.Format(
                            ResourceManager.Term.coolingTimeStatement,
                            item.CurrentCoolingTime
                        );
                    else
                        c.textComponent2.text = string.Empty;
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                    c.textComponent2.text = string.Empty;
                }
            }
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStat.Refresh(CurrentAction(item.id));
            }
            else
            {
                itemStat.Refresh(null);
            }
        }

        private void Interact()
        {
            if (itemListBox.SelectedItem is not QuantityList.ListItem quantityItem)
                return;

            var weapon = CurrentAction(quantityItem.id);
            if (weapon == null)
                return;

            if (weapon.IsCooling)
            {
                UIManager.StartMessage(ResourceManager.Term.promptItemIsCooling, null);
                return;
            }

            var usage = weapon.Usage;
            BattleManager.CurrentCommand.SelectedItems.Add(new(weapon, usage));

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
            BattleManager.CurrentCommand.SelectedItems.Clear();
            canvasGroup.alpha = 1;
        }

        public void Cancel()
        {
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
