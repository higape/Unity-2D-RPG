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
        private GameObject actorUsableItemPrefab;

        [SerializeField]
        private GameObject actorArmorPrefab;

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
                (ResourceManager.Term.recoverItem, "recoverItem"),
                (ResourceManager.Term.attackItem, "attackItem"),
                (ResourceManager.Term.auxiliaryItem, "auxiliaryItem"),
                (ResourceManager.Term.normalItem, "normalItem"),
                (ResourceManager.Term.weapon, "weapon"),
                (ResourceManager.Term.headArmor, "headArmor"),
                (ResourceManager.Term.bodyArmor, "bodyArmor"),
                (ResourceManager.Term.handArmor, "handArmor"),
                (ResourceManager.Term.footArmor, "footArmor"),
                (ResourceManager.Term.ornamentArmor, "ornamentArmor"),
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
                case "recoverItem":
                    UIManager
                        .Instantiate(actorUsableItemPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(0);
                    break;
                case "attackItem":
                    UIManager
                        .Instantiate(actorUsableItemPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(1);
                    break;
                case "auxiliaryItem":
                    UIManager
                        .Instantiate(actorUsableItemPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(2);
                    break;
                case "normalItem":
                    break;
                case "weapon":
                    break;
                case "headArmor":
                    UIManager
                        .Instantiate(actorArmorPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(0);
                    break;
                case "bodyArmor":
                    UIManager
                        .Instantiate(actorArmorPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(1);
                    break;
                case "handArmor":
                    UIManager
                        .Instantiate(actorArmorPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(2);
                    break;
                case "footArmor":
                    UIManager
                        .Instantiate(actorArmorPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(3);
                    break;
                case "ornamentArmor":
                    UIManager
                        .Instantiate(actorArmorPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(4);
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
