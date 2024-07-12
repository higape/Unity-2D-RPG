using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using SIT = Static.CommonItemType;
using UIT = Static.ActorUsableItem.ItemType;

namespace UI
{
    //在商店购买物品的界面
    //无论简单物品或复杂物品，均显示物品名称、价格、持有数量、装备数量
    public class ShopBuyingPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject quantityPanelPrefab;

        private Static.Shop DataObject { get; set; }
        private List<(ICommodity, int)> Commodities { get; set; } = new();
        private InputCommand[] InputCommands { get; set; }

        public void Setup(Static.Shop dataObject)
        {
            DataObject = dataObject;
            MakeItemList();
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Down, listBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Down, listBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Down, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            heading.text = ResourceManager.Term.buy;

            listBox.Initialize(1, 8, RefreshItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ShopBuyingPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ShopBuyingPanel));
        }

        private void MakeItemList()
        {
            foreach (var item in DataObject.items)
            {
                switch (item.type)
                {
                    case SIT.ActorNormalItem:
                        Commodities.Add(
                            (
                                new ActorNormalItem(item.id),
                                Party.ActorNormalItem.GetQuantity(item.id)
                            )
                        );
                        break;
                    case SIT.ActorRecoverItem:
                        Commodities.Add(
                            (
                                new ActorUsableItem(UIT.RecoverItem, item.id),
                                Party.ActorRecoverItem.GetQuantity(item.id)
                            )
                        );
                        break;
                    case SIT.ActorAttackItem:
                        Commodities.Add(
                            (
                                new ActorUsableItem(UIT.AttackItem, item.id),
                                Party.ActorAttackItem.GetQuantity(item.id)
                            )
                        );
                        break;
                    case SIT.ActorAuxiliaryItem:
                        Commodities.Add(
                            (
                                new ActorUsableItem(UIT.AuxiliaryItem, item.id),
                                Party.ActorAuxiliaryItem.GetQuantity(item.id)
                            )
                        );
                        break;
                    case SIT.ActorWeapon:
                        Commodities.Add(
                            (new ActorWeapon(item.id), Party.ActorWeapon.GetQuantity(item.id))
                        );
                        break;
                    case SIT.ActorHeadArmor:
                        Commodities.Add(
                            (new ActorArmor(0, item.id), Party.ActorHeadArmor.GetQuantity(item.id))
                        );
                        break;
                    case SIT.ActorBodyArmor:
                        Commodities.Add(
                            (new ActorArmor(1, item.id), Party.ActorBodyArmor.GetQuantity(item.id))
                        );
                        break;
                    case SIT.ActorHandArmor:
                        Commodities.Add(
                            (new ActorArmor(2, item.id), Party.ActorHandArmor.GetQuantity(item.id))
                        );
                        break;
                    case SIT.ActorFootArmor:
                        Commodities.Add(
                            (new ActorArmor(3, item.id), Party.ActorFootArmor.GetQuantity(item.id))
                        );
                        break;
                    case SIT.ActorOrnamentArmor:
                        Commodities.Add(
                            (
                                new ActorArmor(4, item.id),
                                Party.ActorOrnamentArmor.GetQuantity(item.id)
                            )
                        );
                        break;
                }
            }

            listBox.SetSource(Commodities);
        }

        private void Interact()
        {
            if (listBox.SelectedItem != null)
            {
                var item = ((ICommodity, int))listBox.SelectedItem;
                //打开数量面板并传递回调
                UIManager
                    .Instantiate(quantityPanelPrefab)
                    .GetComponent<QuantityPanel>()
                    .Setup(
                        item.Item1.Price,
                        Mathf.Min(
                            Party.Gold / item.Item1.Price,
                            Mathf.Max(Party.MaxItemQuantity - item.Item2, 0)
                        ),
                        QuantityCallback
                    );
            }
        }

        private void QuantityCallback(int quantity)
        {
            //执行购买
            if (listBox.SelectedItem != null)
            {
                var item = ((ICommodity, int))listBox.SelectedItem;
                item.Item1.Buy(quantity);
                Commodities[listBox.SelectedIndex] = (item.Item1, item.Item2 + quantity);
            }

            listBox.Refresh();
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem7 c)
            {
                if (data != null)
                {
                    c.textComponent0.text = (((ICommodity, int))data).Item1.Name;
                    c.textComponent1.text = ResourceManager.Term.price + ':';
                    c.textComponent2.text = (((ICommodity, int))data).Item1.Price.ToString();
                    c.textComponent3.text = ResourceManager.Term.holdingQuantity + ':';
                    c.textComponent4.text = (((ICommodity, int))data).Item2.ToString();
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                    c.textComponent2.text = " ";
                    c.textComponent3.text = " ";
                    c.textComponent4.text = " ";
                }
            }
        }
    }
}
