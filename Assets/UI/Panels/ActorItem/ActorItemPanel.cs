using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using HIT = Static.ActorUsableItem.ItemType;

namespace UI
{
    public class ActorItemPanel : MonoBehaviour
    {
        private delegate ActorUsableItem MakeCommodity(int id);

        [SerializeField]
        private ListBox typeListBox;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorUsableItemStatistic itemStatistic;

        [SerializeField]
        private GameObject humanListPrefab;

        private MakeCommodity CurrentAction { get; set; }

        private ActorSimpleStatusListPanel ActorListInstance { get; set; }

        private QuantityList.ListItem CurrentQuantityItem { get; set; }

        private ActorUsableItem CurrentItem { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
                new(InputCommand.ButtonPrevious, ButtonType.Down, typeListBox.SelectLeft),
                new(InputCommand.ButtonNext, ButtonType.Down, typeListBox.SelectRight),
            };

            (string, string)[] typeTexts = new (string, string)[]
            {
                (ResourceManager.Term.recoverItem, "recoverItem"),
                (ResourceManager.Term.attackItem, "attackItem"),
                (ResourceManager.Term.auxiliaryItem, "auxiliaryItem")
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);

            typeListBox.Initialize(typeTexts.Length, 1, RefreshType, typeTexts);
            typeListBox.RegisterSelectedItemChangeCallback(OnTypeChange);
            typeListBox.SelectFirst();
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorItemPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorItemPanel));
        }

        private void OnTypeChange(object data, int index)
        {
            QuantityList list;
            switch ((((string, string))typeListBox.SelectedItem).Item2)
            {
                case "recoverItem":
                    CurrentAction = (id) => new ActorUsableItem(HIT.RecoverItem, id);
                    list = Party.ActorRecoverItem;
                    break;
                case "attackItem":
                    CurrentAction = (id) => new ActorUsableItem(HIT.AttackItem, id);
                    list = Party.ActorAttackItem;
                    break;
                case "auxiliaryItem":
                    CurrentAction = (id) => new ActorUsableItem(HIT.AuxiliaryItem, id);
                    list = Party.ActorAuxiliaryItem;
                    break;
                default:
                    list = new();
                    break;
            }
            itemListBox.SetSource(list);
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
            if (itemListBox.SelectedItem != null)
            {
                CurrentQuantityItem = (QuantityList.ListItem)itemListBox.SelectedItem;
                CurrentItem = CurrentAction(((QuantityList.ListItem)itemListBox.SelectedItem).id);
                if (CurrentItem.UsedInMenu)
                {
                    //打开角色面板并传递回调
                    ActorListInstance = UIManager
                        .Instantiate(humanListPrefab)
                        .GetComponent<ActorSimpleStatusListPanel>();
                    ActorListInstance.SetCallback(OnActorInteract);
                }
                else
                {
                    //提示道具无法使用
                    UIManager.StartMessage(ResourceManager.Term.promptCannotUseItemInMenu, null);
                }
            }
        }

        private void OnActorInteract(Battler actor)
        {
            if (CurrentQuantityItem.quantity > 0)
            {
                var item = CurrentItem;
                var result = Mathc.ProcessItemEffect(
                    item.Usage,
                    actor,
                    actor,
                    actor.Atk + item.Attack,
                    actor.Hit,
                    1f
                );
                //在生效的情况下消耗道具
                if (item.Consumable && (result.Item1 > 0 || result.Item2 > 0))
                {
                    item.CostAndCool();
                    itemListBox.Refresh();
                    if (CurrentQuantityItem.quantity <= 0)
                        itemListBox.Reselect();
                }
                ActorListInstance.Refresh();
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshType(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, string))data).Item1;
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
