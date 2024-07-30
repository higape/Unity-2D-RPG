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
    //在商店卖出物品的界面
    //对于简单物品，显示物品名称、价格、持有数量、装备数量
    //对于复杂物品，显示物品名称、价格
    public class ShopSellingPanel : MonoBehaviour
    {
        private delegate ICommodity MakeCommodity(int id);
        private delegate void RefreshStat(object item);

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

        private SIT ItemType { get; set; }

        private UnityAction Callback { get; set; }

        private List<(ICommodity, int)> Commodities { get; set; } = new();

        private MakeCommodity ItemAction { get; set; }

        private RefreshStat StatAction { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(SIT itemType, UnityAction callback)
        {
            ItemType = itemType;
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
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ShopSellingPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ShopSellingPanel));
        }

        private void MakeItemList()
        {
            const int col = 1;
            const int row = 6;

            listBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);

            switch (ItemType)
            {
                case SIT.ActorNormalItem:
                    ItemAction = (id) => new ActorNormalItem(id);
                    StatAction = ShowNormalItemStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorNormalItem);
                    break;
                case SIT.ActorRecoverItem:
                    ItemAction = (id) => new ActorUsableItem(UIT.RecoverItem, id);
                    StatAction = ShowUsableItemStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorRecoverItem);
                    break;
                case SIT.ActorAttackItem:
                    ItemAction = (id) => new ActorUsableItem(UIT.AttackItem, id);
                    StatAction = ShowUsableItemStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorAttackItem);
                    break;
                case SIT.ActorAuxiliaryItem:
                    ItemAction = (id) => new ActorUsableItem(UIT.AuxiliaryItem, id);
                    StatAction = ShowUsableItemStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorAuxiliaryItem);
                    break;
                case SIT.ActorWeapon:
                    ItemAction = (id) => new ActorWeapon(id);
                    StatAction = ShowWeaponStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorWeapon);
                    break;
                case SIT.ActorHeadArmor:
                    ItemAction = (id) => new ActorArmor(0, id);
                    StatAction = ShowArmorStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorHeadArmor);
                    break;
                case SIT.ActorBodyArmor:
                    ItemAction = (id) => new ActorArmor(1, id);
                    StatAction = ShowArmorStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorBodyArmor);
                    break;
                case SIT.ActorHandArmor:
                    ItemAction = (id) => new ActorArmor(2, id);
                    StatAction = ShowArmorStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorHandArmor);
                    break;
                case SIT.ActorFootArmor:
                    ItemAction = (id) => new ActorArmor(3, id);
                    StatAction = ShowArmorStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorFootArmor);
                    break;
                case SIT.ActorOrnamentArmor:
                    ItemAction = (id) => new ActorArmor(4, id);
                    StatAction = ShowArmorStat;
                    listBox.Initialize(col, row, RefreshItem, Party.ActorOrnamentArmor);
                    break;
            }
        }

        private void RefreshBlank(TextItem7 c)
        {
            c.textComponent0.text = " ";
            c.textComponent1.text = " ";
            c.textComponent2.text = " ";
            c.textComponent3.text = " ";
            c.textComponent4.text = " ";
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem7 c)
            {
                if (data is QuantityList.ListItem item)
                {
                    var d = ItemAction(item.id);
                    c.textComponent0.text = d.Name;
                    c.textComponent1.text = ResourceManager.Term.currencyUnit + ':';
                    c.textComponent2.text = d.SellingPrice.ToString();
                    c.textComponent3.text = ResourceManager.Term.holdingQuantity + ':';
                    c.textComponent4.text = item.quantity.ToString();
                }
                else
                {
                    RefreshBlank(c);
                }
            }
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data != null)
            {
                var info = (QuantityList.ListItem)data;
                StatAction(ItemAction(info.id));
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

        private void ShowUsableItemStat(object item)
        {
            usableItemStatistic.gameObject.SetActive(true);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(false);
            usableItemStatistic.Refresh(item as ActorUsableItem);
        }

        private void ShowNormalItemStat(object item)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(true);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(false);
            normalItemStatistic.Refresh(item as ActorNormalItem);
        }

        private void ShowWeaponStat(object item)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(true);
            armorStatistic.gameObject.SetActive(false);
            weaponStatistic.Refresh(item as ActorWeapon);
        }

        private void ShowArmorStat(object item)
        {
            usableItemStatistic.gameObject.SetActive(false);
            normalItemStatistic.gameObject.SetActive(false);
            weaponStatistic.gameObject.SetActive(false);
            armorStatistic.gameObject.SetActive(true);
            armorStatistic.Refresh(item as ActorArmor);
        }

        private void Interact()
        {
            if (listBox.SelectedItem is QuantityList.ListItem item)
            {
                //打开数量面板并传递回调
                UIManager
                    .Instantiate(quantityPanelPrefab)
                    .GetComponent<QuantityPanel>()
                    .Setup(ItemAction(item.id).SellingPrice, item.quantity, QuantityCallback);
            }
        }

        private void QuantityCallback(int quantity)
        {
            //执行卖出
            if (listBox.SelectedItem is QuantityList.ListItem item)
            {
                ItemAction(item.id).Sell(quantity);
            }

            listBox.Refresh();
            listBox.Reselect();
        }

        private void Cancel()
        {
            Callback?.Invoke();
            Destroy(gameObject);
        }
    }
}
