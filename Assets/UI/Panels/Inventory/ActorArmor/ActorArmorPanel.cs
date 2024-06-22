using Dynamic;
using Root;
using UnityEngine;

namespace UI
{
    public class ActorArmorPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorArmorStatistic itemStatistic;

        private int SlotIndex { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(int slotIndex)
        {
            SlotIndex = slotIndex;
            var list = Party.GetActorArmorList(SlotIndex);
            itemListBox.SetSource(list);
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorArmorPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorArmorPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStatistic.Refresh(new ActorArmor(SlotIndex, item.id));
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
                    var d = new ActorArmor(SlotIndex, item.id);
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
