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

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
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

        public void Setup(int slotIndex, Actor actor, UnityAction<object> callback)
        {
            SlotIndex = slotIndex;
            Callback = callback;

            List<QuantityList.ListItem> fl = new();
            var partyItems = Party.GetActorArmorList(slotIndex);
            var dataItems = ResourceManager.GetActorArmorList(slotIndex);

            foreach (var item in partyItems)
            {
                if (dataItems.GetItem(item.id).CanEquip(actor))
                    fl.Add(item);
            }

            listBox.Initialize(1, 8, RefreshItem, fl);
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
            Destroy(gameObject);
        }
    }
}
