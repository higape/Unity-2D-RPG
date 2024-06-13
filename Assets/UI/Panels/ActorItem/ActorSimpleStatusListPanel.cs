using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ActorSimpleStatusListPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        private UnityAction<Battler> Callback { get; set; }

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

            var humans = Party.GetBattleActorList();
            listBox.Initialize(1, humans.Count, RefreshItem, humans);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorSimpleStatusListPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorSimpleStatusListPanel));
        }

        public void SetCallback(UnityAction<Battler> callback)
        {
            Callback = callback;
        }

        public void Refresh()
        {
            listBox.Refresh();
        }

        private void Interact()
        {
            Callback.Invoke(listBox.SelectedItem as Battler);
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is SimpleActorStatus c)
            {
                c.SetActor(data as Actor);
            }
        }
    }
}
