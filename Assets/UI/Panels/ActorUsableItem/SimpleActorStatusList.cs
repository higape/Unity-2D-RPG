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

            var actors = Party.GetBattleActorList();
            listBox.Initialize(1, actors.Count, RefreshItem, actors);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SimpleActorStatusList), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SimpleActorStatusList));
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
