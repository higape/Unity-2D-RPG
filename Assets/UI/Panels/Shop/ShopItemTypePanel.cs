using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using SIT = Static.Shop.ItemType;

namespace UI
{
    public class ShopItemTypePanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject sellingPanelPrefab;

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

            heading.text = ResourceManager.Term.sell;

            (string, SIT)[] texts = new (string, SIT)[]
            {
                (ResourceManager.Term.normalItem, SIT.ActorNormalItem),
                (ResourceManager.Term.recoverItem, SIT.ActorRecoverItem),
                (ResourceManager.Term.attackItem, SIT.ActorAttackItem),
                (ResourceManager.Term.auxiliaryItem, SIT.ActorAuxiliaryItem),
                (ResourceManager.Term.weapon, SIT.ActorWeapon),
                (ResourceManager.Term.headArmor, SIT.ActorHeadArmor),
                (ResourceManager.Term.bodyArmor, SIT.ActorBodyArmor),
                (ResourceManager.Term.handArmor, SIT.ActorHandArmor),
                (ResourceManager.Term.footArmor, SIT.ActorFootArmor),
                (ResourceManager.Term.ornamentArmor, SIT.ActorOrnamentArmor),
            };

            listBox.Initialize(1, 5, Refresh, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ShopItemTypePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ShopItemTypePanel));
        }

        private void Interact()
        {
            UIManager
                .Instantiate(sellingPanelPrefab)
                .GetComponent<ShopSellingPanel>()
                .Setup((((string, SIT))listBox.SelectedItem).Item2);
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
                    c.textComponent.text = (((string, SIT))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
