using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class SimpleActorStatusList : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        private List<Actor> Actors { get; set; }

        private UnityAction<Actor> Callback { get; set; }

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
            InputManagementSystem.AddCommands(nameof(SimpleActorStatusList), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SimpleActorStatusList));
        }

        public void Setup(List<Actor> actors, UnityAction<Actor> callback)
        {
            Actors = actors;
            Callback = callback;
            listBox.Initialize(1, Mathf.Min(6, actors.Count), RefreshItem, actors);
        }

        public void Refresh()
        {
            listBox.Refresh();
        }

        private void Interact()
        {
            Callback.Invoke(listBox.SelectedItem as Actor);
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
