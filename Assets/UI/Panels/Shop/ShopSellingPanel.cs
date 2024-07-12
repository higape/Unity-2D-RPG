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

        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject quantityPanelPrefab;

        private SIT ItemType { get; set; }
        private List<(ICommodity, int)> Commodities { get; set; } = new();
        private MakeCommodity CurrentAction { get; set; }
        private InputCommand[] InputCommands { get; set; }

        public void Setup(SIT itemType)
        {
            ItemType = itemType;
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

            heading.text = ResourceManager.Term.sell;
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
            const int row = 8;

            switch (ItemType)
            {
                case SIT.ActorNormalItem:
                    CurrentAction = (id) => new ActorNormalItem(id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorNormalItem);
                    break;
                case SIT.ActorRecoverItem:
                    CurrentAction = (id) => new ActorUsableItem(UIT.RecoverItem, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorRecoverItem);
                    break;
                case SIT.ActorAttackItem:
                    CurrentAction = (id) => new ActorUsableItem(UIT.AttackItem, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorAttackItem);
                    break;
                case SIT.ActorAuxiliaryItem:
                    CurrentAction = (id) => new ActorUsableItem(UIT.AuxiliaryItem, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorAuxiliaryItem);
                    break;
                case SIT.ActorWeapon:
                    CurrentAction = (id) => new ActorWeapon(id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorWeapon);
                    break;
                case SIT.ActorHeadArmor:
                    CurrentAction = (id) => new ActorArmor(0, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorHeadArmor);
                    break;
                case SIT.ActorBodyArmor:
                    CurrentAction = (id) => new ActorArmor(1, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorBodyArmor);
                    break;
                case SIT.ActorHandArmor:
                    CurrentAction = (id) => new ActorArmor(2, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorHandArmor);
                    break;
                case SIT.ActorFootArmor:
                    CurrentAction = (id) => new ActorArmor(3, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorFootArmor);
                    break;
                case SIT.ActorOrnamentArmor:
                    CurrentAction = (id) => new ActorArmor(4, id);
                    listBox.Initialize(col, row, RefreshItem, Party.ActorOrnamentArmor);
                    break;
            }
        }

        private void Interact()
        {
            if (listBox.SelectedItem is QuantityList.ListItem item)
            {
                //打开数量面板并传递回调
                UIManager
                    .Instantiate(quantityPanelPrefab)
                    .GetComponent<QuantityPanel>()
                    .Setup(CurrentAction(item.id).SellingPrice, item.quantity, QuantityCallback);
            }
        }

        private void QuantityCallback(int quantity)
        {
            //执行卖出
            if (listBox.SelectedItem is QuantityList.ListItem item)
            {
                CurrentAction(item.id).Sell(quantity);
            }

            listBox.Refresh();
            listBox.Reselect();
        }

        private void Cancel()
        {
            Destroy(gameObject);
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
                    var d = CurrentAction(item.id);
                    c.textComponent0.text = d.Name;
                    c.textComponent1.text = ResourceManager.Term.unitPrice + ':';
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
    }
}
