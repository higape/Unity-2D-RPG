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
        private GameObject actorUsableItemPanelPrefab;

        [SerializeField]
        private GameObject actorNormalItemPanelPrefab;

        [SerializeField]
        private GameObject actorWeaponPanelPrefab;

        [SerializeField]
        private GameObject actorArmorPanelPrefab;

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
                        .Instantiate(actorUsableItemPanelPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(Static.ActorUsableItem.ItemType.RecoverItem);
                    break;
                case "attackItem":
                    UIManager
                        .Instantiate(actorUsableItemPanelPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(Static.ActorUsableItem.ItemType.AttackItem);
                    break;
                case "auxiliaryItem":
                    UIManager
                        .Instantiate(actorUsableItemPanelPrefab)
                        .GetComponent<ActorUsableItemPanel>()
                        .Setup(Static.ActorUsableItem.ItemType.AuxiliaryItem);
                    break;
                case "normalItem":
                    UIManager.Instantiate(actorNormalItemPanelPrefab);
                    break;
                case "weapon":
                    UIManager.Instantiate(actorWeaponPanelPrefab);
                    break;
                case "headArmor":
                    UIManager
                        .Instantiate(actorArmorPanelPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(0);
                    break;
                case "bodyArmor":
                    UIManager
                        .Instantiate(actorArmorPanelPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(1);
                    break;
                case "handArmor":
                    UIManager
                        .Instantiate(actorArmorPanelPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(2);
                    break;
                case "footArmor":
                    UIManager
                        .Instantiate(actorArmorPanelPrefab)
                        .GetComponent<ActorArmorPanel>()
                        .Setup(3);
                    break;
                case "ornamentArmor":
                    UIManager
                        .Instantiate(actorArmorPanelPrefab)
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
