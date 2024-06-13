using System.Collections;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 分支选择面板，临时使用，用完即毁
    /// </summary>
    public class ChoiceListPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textComponent;

        [SerializeField]
        private ListBox listBox;

        private UnityAction<int> Callback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ChoiceListPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ChoiceListPanel));
        }

        public void StartChoice(string text, IList data, UnityAction<int> callback)
        {
            textComponent.text = text;
            Callback = callback;
            listBox.Initialize(1, data.Count, RefreshItem, data);
        }

        private void Interact()
        {
            if (listBox.SelectedIndex >= 0)
            {
                Callback?.Invoke(listBox.SelectedIndex);
                Destroy(gameObject);
            }
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                c.textComponent.text = data as string;
            }
        }
    }
}
