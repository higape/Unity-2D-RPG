using System.Collections;
using System.Collections.Generic;
using Root;
using UnityEngine;

namespace UI
{
    public class ItemTypePanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject humanItemPrefab;

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

            (string, string)[] texts = new (string, string)[]
            {
                (ResourceManager.Term.item, "item"),
                (ResourceManager.Term.equip, "equip")
            };

            listBox.Initialize(1, texts.Length, Refresh, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ItemTypePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ItemTypePanel));
        }

        private void Interact()
        {
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "item":
                    UIManager.Instantiate(humanItemPrefab);
                    break;
                case "equip":
                    break;
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void Refresh(ListBoxItem listItem, object data)
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
