using System;
using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    //供玩家选择道具数量并显示价格
    public class QuantityPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private TextMeshProUGUI quantityContent;

        [SerializeField]
        private TextMeshProUGUI totalPriceLabel;

        [SerializeField]
        private TextMeshProUGUI totalPriceContent;

        private int UnitPrice { get; set; }
        private int Quantity { get; set; }
        private int MinQuantity => 0;
        private int MaxQuantity { get; set; }
        private UnityAction<int> Callback { get; set; }
        private InputCommand[] InputCommands { get; set; }

        public void Setup(int unitPrice, int maxQuantity, UnityAction<int> callback)
        {
            UnitPrice = unitPrice;
            MaxQuantity = maxQuantity;
            Callback = callback;
            if (MaxQuantity > 0)
                Quantity = 1;
            else
                Quantity = 0;
            Refresh();
        }

        private void Awake()
        {
            heading.text = ResourceManager.Term.selectQuantity;
            totalPriceLabel.text = ResourceManager.Term.totalPrice;
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, () => AddQuantity(1)),
                new(InputCommand.ButtonDown, ButtonType.Press, () => ReduceQuantity(1)),
                new(InputCommand.ButtonLeft, ButtonType.Press, () => ReduceQuantity(10)),
                new(InputCommand.ButtonRight, ButtonType.Press, () => AddQuantity(10)),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(QuantityPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(QuantityPanel));
        }

        private void AddQuantity(int value)
        {
            Quantity = Mathf.Clamp(Quantity + value, MinQuantity, MaxQuantity);
            Refresh();
        }

        private void ReduceQuantity(int value)
        {
            Quantity = Mathf.Clamp(Quantity - value, MinQuantity, MaxQuantity);
            Refresh();
        }

        private void Interact()
        {
            Callback?.Invoke(Quantity);
            Destroy(gameObject);
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void Refresh()
        {
            quantityContent.text = Quantity.ToString();
            totalPriceContent.text = (UnitPrice * Quantity).ToString();
        }
    }
}
