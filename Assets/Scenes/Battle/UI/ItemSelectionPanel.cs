using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
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
        private ListBox typeListBox;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorUsableItemStatistic itemStatistic;

        [SerializeField]
        private GameObject actorPanelPrefab;

        [SerializeField]
        private GameObject enemyPanelPrefab;

        private GetItemByID CurrentAction { get; set; }

        private Actor CurrentActor { get; set; }

        private UnityAction CancelCallback { get; set; }

        private UnityAction FinishCallback { get; set; }

        private (string, UIT)[] TextTypePairs { get; set; }

        private bool TypeLimit { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, itemListBox.SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, itemListBox.SelectRight),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
                new(
                    InputCommand.ButtonPrevious,
                    ButtonType.Down,
                    () =>
                    {
                        if (!TypeLimit)
                            typeListBox.SelectLeft();
                    }
                ),
                new(
                    InputCommand.ButtonNext,
                    ButtonType.Down,
                    () =>
                    {
                        if (!TypeLimit)
                            typeListBox.SelectRight();
                    }
                ),
            };

            TextTypePairs = new (string, UIT)[]
            {
                (ResourceManager.Term.recoverItem, UIT.RecoverItem),
                (ResourceManager.Term.attackItem, UIT.AttackItem),
                (ResourceManager.Term.auxiliaryItem, UIT.AuxiliaryItem)
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);

            typeListBox.Initialize(TextTypePairs.Length, 1, RefreshType, TextTypePairs);
            typeListBox.RegisterSelectedItemChangeCallback(OnTypeChange);
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
            bool typeLimit
        )
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            TypeLimit = typeLimit;

            int index = -1;
            for (int i = 0; i < TextTypePairs.Length; i++)
                if (TextTypePairs[i].Item2 == itemType)
                    index = i;
            typeListBox.Select(index);

            BattleManager.CurrentCommand.SelectedItems = new();
        }

        private void OnTypeChange(object data, int index)
        {
            UIT itemType = (((string, UIT))typeListBox.SelectedItem).Item2;
            CurrentAction = (id) => new ActorUsableItem(itemType, id);
            itemListBox.SetSource(Party.GetActorItemListInBattle(itemType));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStatistic.Refresh(CurrentAction(item.id));
            }
            else
            {
                itemStatistic.Refresh(null);
            }
        }

        private void Interact()
        {
            if (itemListBox.SelectedItem is not QuantityList.ListItem quantityItem)
                return;
            var weapon = CurrentAction(quantityItem.id);
            if (weapon == null)
                return;
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

        public void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        public void Cancel()
        {
            BattleManager.CurrentCommand.SelectedItems = null;
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        private void RefreshType(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, UIT))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is QuantityList.ListItem item)
                {
                    var d = CurrentAction(item.id);
                    c.textComponent0.text = d.Name;
                    c.textComponent1.text = item.quantity.ToString();
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                }
            }
        }
    }
}
