using Dynamic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ActorWeaponPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI header;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorWeaponStatistic itemStatistic;

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, itemListBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Press, itemListBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            header.text = ResourceManager.Term.weapon;
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            itemListBox.Initialize(1, 8, RefreshItem, Party.ActorWeapon);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorWeaponPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorWeaponPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStatistic.Refresh(new ActorWeapon(item.id));
            }
            else
            {
                itemStatistic.Refresh(null);
            }
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
                    var d = new ActorWeapon(item.id);
                    c.textComponent0.text = d.Name;
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
