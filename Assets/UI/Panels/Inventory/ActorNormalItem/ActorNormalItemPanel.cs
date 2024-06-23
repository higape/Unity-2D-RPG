using Dynamic;
using Root;
using UnityEngine;

namespace UI
{
    public class ActorNormalItemPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorNormalItemStatistic itemStatistic;

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            itemListBox.Initialize(1, 8, RefreshItem, Party.ActorNormalItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorNormalItemPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorNormalItemPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStatistic.Refresh(new ActorNormalItem(item.id));
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
                    var d = new ActorNormalItem(item.id);
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
