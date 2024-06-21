using System.Collections;
using System.Collections.Generic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class SystemMenuPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject saveFilePanelPrefab;

        [SerializeField]
        private GameObject loadFilePanelPrefab;

        [SerializeField]
        private GameObject settingPanelPrefab;

        private UnityAction Callback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(UnityAction callback)
        {
            Callback = callback;
        }

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
                (ResourceManager.Term.saveFile, "saveFile"),
                (ResourceManager.Term.loadFile, "loadFile"),
                (ResourceManager.Term.settings, "settings"),
                (ResourceManager.Term.endGame, "endGame")
            };

            listBox.Initialize(1, texts.Length, Refresh, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SystemMenuPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SystemMenuPanel));
        }

        private void Interact()
        {
            if (listBox.SelectedItem != null)
            {
                var item = ((string, string))listBox.SelectedItem;
                switch (item.Item2)
                {
                    case "saveFile":
                        UIManager.Instantiate(saveFilePanelPrefab);
                        break;
                    case "loadFile":
                        UIManager.Instantiate(loadFilePanelPrefab);
                        break;
                    case "settings":
                        UIManager.Instantiate(settingPanelPrefab);
                        break;
                    case "endGame":
#if UNITY_EDITOR
#else
                        ScreenManager.FadeOut(() => QuitGame());
                        Destroy(gameObject);
#endif
                        break;
                }
            }
        }

        private static void QuitGame() => Application.Quit();

        private void Cancel()
        {
            Callback?.Invoke();
            Destroy(gameObject);
        }

        private void Refresh(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                {
                    c.textComponent.text = (((string, string))data).Item1;
                }
                else
                {
                    c.textComponent.text = " ";
                }
            }
        }
    }
}
