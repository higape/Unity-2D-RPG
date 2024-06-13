using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ShopPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI goldLabel;

        [SerializeField]
        private TextMeshProUGUI goldContent;

        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox commandListBox;

        [SerializeField]
        private GameObject buyingPanelPrefab;

        [SerializeField]
        private GameObject itemTypePanelPrefab;

        private Static.Shop DataObject { get; set; }
        private UnityAction DestroyCallback { get; set; }
        private List<(ICommodity, int)> Commodities { get; set; } = new();
        private InputCommand[] InputCommands { get; set; }

        public void Setup(int id, UnityAction destroyCallback)
        {
            DestroyCallback = destroyCallback;
            DataObject = ResourceManager.Shop.GetItem(id);
            heading.text = DataObject.Name;
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonLeft, ButtonType.Press, commandListBox.SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, commandListBox.SelectRight),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            goldLabel.text = ResourceManager.Term.currencyUnit + ':';
            RefreshGold(Party.Gold);
            Party.RegisterGoldChangeCallback(RefreshGold);

            (string, string)[] commandTexts = new (string, string)[]
            {
                (ResourceManager.Term.buy, "buy"),
                (ResourceManager.Term.sell, "sell"),
            };

            commandListBox.Initialize(commandTexts.Length, 1, RefreshCommand, commandTexts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ShopPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ShopPanel));
        }

        private void OnDestroy()
        {
            DestroyCallback?.Invoke();
            Party.UnregisterGoldChangeCallback(RefreshGold);
        }

        private void RefreshGold(int gold)
        {
            goldContent.text = gold.ToString();
        }

        private void Interact()
        {
            if (commandListBox.SelectedItem != null)
            {
                switch ((((string, string))commandListBox.SelectedItem).Item2)
                {
                    case "buy":
                        UIManager
                            .Instantiate(buyingPanelPrefab)
                            .GetComponent<ShopBuyingPanel>()
                            .Setup(DataObject);
                        break;
                    case "sell":
                        UIManager.Instantiate(itemTypePanelPrefab);
                        break;
                }
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshCommand(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, string))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
