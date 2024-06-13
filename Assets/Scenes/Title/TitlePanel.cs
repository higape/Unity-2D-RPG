using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title
{
    /// <summary>
    /// 标题画面的指令面板
    /// </summary>
    public class TitlePanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private ListBox listBox;

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            title.text = ResourceManager.GameInfo.gameName;

            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
            };

            (string, string)[] texts = new (string, string)[]
            {
                (ResourceManager.Term.newGame, "newGame"),
                (ResourceManager.Term.continueGame, "continueGame"),
                (ResourceManager.Term.options, "options"),
                (ResourceManager.Term.endGame, "endGame")
            };

            listBox.Initialize(1, texts.Length, RefreshItem, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(TitlePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(TitlePanel));
        }

        private void Interact()
        {
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "newGame":
                    enabled = false;
                    Dynamic.Party.InitializeByNewGame();
                    Map.PlayerController.WaitCount++;
                    ScreenManager.FadeOut(NewGameFadeOutCallback);
                    break;
                case "endGame":
                    Application.Quit();
                    break;
            }
        }

        private static void NewGameFadeOutCallback()
        {
            SceneManager.UnloadSceneAsync("Title");
            SceneManager.LoadScene("MapRoot", LoadSceneMode.Additive);
            MapManager.GoToNewMap(
                ResourceManager.GameInfo.startMapName,
                () => ScreenManager.FadeIn(() => Map.PlayerController.WaitCount--)
            );
        }

        private void RefreshItem(ListBoxItem listItem, object data)
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
