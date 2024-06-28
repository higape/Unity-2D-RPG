using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;

namespace UI
{
    public class SkillPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private SkillStatistic itemStatistic;

        private InputCommand[] InputCommands { get; set; }

        public void Setup(Actor actor)
        {
            itemListBox.SetSource(actor.AllSkills);
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            itemListBox.Initialize(1, 8, RefreshItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SkillPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SkillPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            itemStatistic.Refresh(data as Skill);
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is Skill item)
                {
                    c.textComponent0.text = item.Name;
                    if (item.IsCountLimit)
                        c.textComponent1.text =
                            item.CurrentCount.ToString() + '/' + item.MaxUsageCount;
                    else
                        c.textComponent1.text = "∞";
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
