using System;
using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ActorWeaponList : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        private UnityAction<object> Callback { get; set; }

        private ActorWeaponStatistic WeaponStatistic { get; set; }

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
            InputManagementSystem.AddCommands(nameof(ActorWeaponList), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorWeaponList));
        }

        public void Setup(Actor actor, UnityAction<object> callback, ActorWeaponStatistic statistic)
        {
            Callback = callback;
            WeaponStatistic = statistic;

            List<QuantityList.ListItem> fl = new();

            foreach (var item in Party.ActorWeapon)
            {
                if (ResourceManager.ActorWeapon.GetItem(item.id).CanEquip(actor.ID))
                    fl.Add(item);
            }

            listBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            listBox.Initialize(1, 8, RefreshItem, fl);
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                WeaponStatistic.Refresh(new(item.id));
            }
            else
            {
                WeaponStatistic.Refresh(null);
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

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is QuantityList.ListItem item)
                {
                    c.textComponent0.text = Root.ResourceManager.ActorWeapon.GetItem(item.id).Name;
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
