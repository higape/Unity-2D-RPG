using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ActorArmorList : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        private UnityAction<object> Callback { get; set; }

        private int SlotIndex { get; set; }

        private ActorArmorStatistic ArmorStatistic { get; set; }

        private InputCommand[] InputCommands { get; set; }

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
            InputManagementSystem.AddCommands(nameof(ActorArmorList), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorArmorList));
        }

        public void Setup(
            int slotIndex,
            Actor actor,
            UnityAction<object> callback,
            ActorArmorStatistic statistic
        )
        {
            SlotIndex = slotIndex;
            Callback = callback;
            ArmorStatistic = statistic;

            List<object> fl = new() { -1 };
            var partyItems = Party.GetActorArmorList(slotIndex);
            var dataItems = ResourceManager.GetActorArmorList(slotIndex);

            foreach (var item in partyItems)
            {
                if (dataItems.GetItem(item.id).CanEquip(actor))
                    fl.Add(item);
            }

            listBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            listBox.Initialize(1, 8, RefreshItem, fl);
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                ArmorStatistic.Refresh(new(SlotIndex, item.id));
            }
            else
            {
                ArmorStatistic.Refresh(null);
            }
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is QuantityList.ListItem item)
                {
                    c.textComponent0.text = ResourceManager
                        .GetActorArmorList(SlotIndex)
                        .GetItem(item.id)
                        .Name;
                    c.textComponent1.text = item.quantity.ToString();
                }
                else if (data is int)
                {
                    c.textComponent0.text = Root.ResourceManager.Term.notEquip;
                    c.textComponent1.text = " ";
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                }
            }
        }

        private void Interact()
        {
            Callback?.Invoke(listBox.SelectedItem);
            Destroy(gameObject);
        }

        private void Cancel()
        {
            Callback?.Invoke(null);
            Destroy(gameObject);
        }
    }
}
