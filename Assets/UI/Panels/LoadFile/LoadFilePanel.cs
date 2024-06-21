using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class LoadFilePanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        private UnityAction<Static.SaveData> Callback { get; set; }
        private InputCommand[] InputCommands { get; set; }

        public void Setup(Static.SaveData[] saveData, UnityAction<Static.SaveData> callback)
        {
            Callback = callback;
            listBox.SetSource(saveData);
        }

        private void Awake()
        {
            heading.text = ResourceManager.Term.loadFile;
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, listBox.SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, listBox.SelectRight),
                new(InputCommand.ButtonPrevious, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            listBox.Initialize(2, 5, RefreshItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(LoadFilePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(LoadFilePanel));
        }

        private void Interact()
        {
            if (listBox.SelectedItem is Static.SaveData saveData)
            {
                Callback?.Invoke(saveData);
                enabled = false;
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem7 c)
            {
                if (data is Static.SaveData save)
                {
                    c.textComponent0.text = save.name;
                }
                else if (data is int)
                {
                    c.textComponent0.text = ResourceManager.Term.createFile;
                }
                else
                {
                    c.textComponent0.text = "nothing";
                }
            }
        }
    }
}
