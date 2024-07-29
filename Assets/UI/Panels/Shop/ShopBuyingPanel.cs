using System;
using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using IT = Static.CommonItemType;
using UIT = Static.ActorUsableItem.ItemType;

namespace UI
{
    //在商店购买物品的界面
    //无论简单物品或复杂物品，均显示物品名称、价格、持有数量、装备数量
    public class ShopBuyingPanel : MonoBehaviour
    {
        private struct ItemInfo
        {
            public ItemInfo(ICommodity commodity, int quantity, Action<ICommodity> action)
            {
                this.commodity = commodity;
                this.quantity = quantity;
                this.action = action;
            }

            public ICommodity commodity;
            public int quantity;
            public Action<ICommodity> action;
        }

        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject quantityPanelPrefab;

        [SerializeField]
        private ActorUsableItemStatistic usableItemStatistic;

        [SerializeField]
        private ActorNormalItemStatistic normalItemStatistic;

        [SerializeField]
        private ActorWeaponStatistic weaponStatistic;

        [SerializeField]
        private ActorArmorStatistic armorStatistic;

        private Static.Shop DataObject { get; set; }

        private UnityAction Callback { get; set; }

        private List<ItemInfo> Commodities { get; set; } = new();

        private InputCommand[] InputCommands { get; set; }

        public void Setup(Static.Shop dataObject, UnityAction callback)
        {
            DataObject = dataObject;
            Callback = callback;
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

            listBox.Initialize(1, 6, RefreshItem);
            listBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ShopBuyingPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ShopBuyingPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data != null)
            {
                ItemInfo info = (ItemInfo)data;
                info.action.Invoke(info.commodity);
            }
            else
            {
                HideAllStat();
            }
        }

        private void HideAllStat()
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(false);
        }

        private void ShowUsableItemStat(ICommodity commodity)
        {
            usableItemStatistic.gameObject.SetActive(true);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(false);
            usableItemStatistic.Refresh(commodity as ActorUsableItem);
        }

        private void ShowNormalItemStat(ICommodity commodity)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(true);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(false);
            normalItemStatistic.Refresh(commodity as ActorNormalItem);
        }

        private void ShowWeaponStat(ICommodity commodity)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(true);
            armorStatistic.gameObject.SetActive(false);
            weaponStatistic.Refresh(commodity as ActorWeapon);
        }

        private void ShowArmorStat(ICommodity commodity)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(true);
            armorStatistic.Refresh(commodity as ActorArmor);
        }

        private void MakeItemList()
        {
            foreach (var item in DataObject.items)
            {
                switch (item.type)
                {
                    case IT.ActorNormalItem:
                        Commodities.Add(
                            new(
                                new ActorNormalItem(item.id),
                                Party.ActorNormalItem.GetQuantity(item.id),
                                ShowNormalItemStat
                            )
                        );
                        break;
                    case IT.ActorRecoverItem:
                        Commodities.Add(
                            new(
                                new ActorUsableItem(UIT.RecoverItem, item.id),
                                Party.ActorRecoverItem.GetQuantity(item.id),
                                ShowUsableItemStat
                            )
                        );
                        break;
                    case IT.ActorAttackItem:
                        Commodities.Add(
                            new(
                                new ActorUsableItem(UIT.AttackItem, item.id),
                                Party.ActorAttackItem.GetQuantity(item.id),
                                ShowUsableItemStat
                            )
                        );
                        break;
                    case IT.ActorAuxiliaryItem:
                        Commodities.Add(
                            new(
                                new ActorUsableItem(UIT.AuxiliaryItem, item.id),
                                Party.ActorAuxiliaryItem.GetQuantity(item.id),
                                ShowUsableItemStat
                            )
                        );
                        break;
                    case IT.ActorWeapon:
                        Commodities.Add(
                            new(
                                new ActorWeapon(item.id),
                                Party.ActorWeapon.GetQuantity(item.id),
                                ShowWeaponStat
                            )
                        );
                        break;
                    case IT.ActorHeadArmor:
                        Commodities.Add(
                            new(
                                new ActorArmor(0, item.id),
                                Party.ActorHeadArmor.GetQuantity(item.id),
                                ShowArmorStat
                            )
                        );
                        break;
                    case IT.ActorBodyArmor:
                        Commodities.Add(
                            new(
                                new ActorArmor(1, item.id),
                                Party.ActorBodyArmor.GetQuantity(item.id),
                                ShowArmorStat
                            )
                        );
                        break;
                    case IT.ActorHandArmor:
                        Commodities.Add(
                            new(
                                new ActorArmor(2, item.id),
                                Party.ActorHandArmor.GetQuantity(item.id),
                                ShowArmorStat
                            )
                        );
                        break;
                    case IT.ActorFootArmor:
                        Commodities.Add(
                            new(
                                new ActorArmor(3, item.id),
                                Party.ActorFootArmor.GetQuantity(item.id),
                                ShowArmorStat
                            )
                        );
                        break;
                    case IT.ActorOrnamentArmor:
                        Commodities.Add(
                            new(
                                new ActorArmor(4, item.id),
                                Party.ActorOrnamentArmor.GetQuantity(item.id),
                                ShowArmorStat
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
                var item = (ItemInfo)listBox.SelectedItem;
                //打开数量面板并传递回调
                UIManager
                    .Instantiate(quantityPanelPrefab)
                    .GetComponent<QuantityPanel>()
                    .Setup(
                        item.commodity.Price,
                        Mathf.Min(
                            Party.Gold / item.commodity.Price,
                            Mathf.Max(Party.MaxItemQuantity - item.quantity, 0)
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
                var item = (ItemInfo)listBox.SelectedItem;
                item.commodity.Buy(quantity);
                Commodities[listBox.SelectedIndex] = new(
                    item.commodity,
                    item.quantity + quantity,
                    item.action
                );
            }

            listBox.Refresh();
        }

        private void Cancel()
        {
            Callback?.Invoke();
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem7 c)
            {
                if (data != null)
                {
                    ItemInfo info = (ItemInfo)data;
                    c.textComponent0.text = info.commodity.Name;
                    c.textComponent1.text = ResourceManager.Term.currencyUnit + ':';
                    c.textComponent2.text = info.commodity.Price.ToString();
                    c.textComponent3.text = ResourceManager.Term.holdingQuantity + ':';
                    c.textComponent4.text = info.quantity.ToString();
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
