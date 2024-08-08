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

        [SerializeField]
        private GameObject selectedLayer;

        [SerializeField]
        private ListBox selectedListBox;

        private GetItemByID CurrentAction { get; set; }

        private Actor CurrentActor { get; set; }

        private int CurrentQuantity { get; set; }

        private List<ActorUsableItem> SelectedItems { get; set; }

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
            UIT itemType,
            int quantity = 1
        )
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            CurrentQuantity = quantity;
            CurrentAction = (id) => new ActorUsableItem(itemType, id);
            heading.text = ResourceManager.Term.GetText(itemType);
            itemListBox.SetSource(Party.GetActorItemListInBattle(itemType));
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

        private void Setup() { }

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
                itemStat.Refresh(CurrentAction(item.id));
            else
                itemStat.Refresh(null);
        }

        private void RefreshSelectedItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
                c.textComponent.text = data is ActorUsableItem item ? item.Name : " ";
        }

        private void Interact()
        {
            if (itemListBox.SelectedItem is not QuantityList.ListItem quantityItem)
                return;

            var item = CurrentAction(quantityItem.id);
            if (item == null)
                return;

            if (item.IsCooling)
            {
                UIManager.StartMessage(ResourceManager.Term.promptItemIsCooling, null);
                return;
            }

            //检查是否重复选择
            foreach (var selected in SelectedItems)
            {
                if (selected.ID == item.ID)
                    return;
            }
            SelectedItems.Add(item);
            BattleManager.CurrentCommand.SelectedItems.Add(new(item, item.Usage));

            //选择的数量不够，先刷新列表
            if (SelectedItems.Count < CurrentQuantity)
            {
                selectedListBox.Refresh();
                return;
            }

            //数量足够，开始选择目标
            var usage = SelectedItems[0].Usage;
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
