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

        [SerializeField]
        private GameObject loadFilePanelPrefab;

        [SerializeField]
        private GameObject settingPanelPrefab;

        private Static.SaveData[] SaveData { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private LoadFilePanel LoadFilePanelInstance { get; set; }

        private void Awake()
        {
            SaveData = ResourceManager.LoadSaveInfo();
            title.text = ResourceManager.GameInfo.gameName;

            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
            };

            (string, string, bool)[] texts = new (string, string, bool)[]
            {
                (ResourceManager.Term.newGame, "newGame", true),
                (ResourceManager.Term.continueGame, "continueGame", SaveData.Length > 0),
                (ResourceManager.Term.settings, "setting", true),
                (ResourceManager.Term.endGame, "endGame", true)
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
            switch ((((string, string, bool))listBox.SelectedItem).Item2)
            {
                case "newGame":
                    enabled = false;
                    Dynamic.Party.InitializeByNewGame();
                    Map.PlayerController.WaitCount++;
                    ScreenManager.FadeOut(NewGameFadeOutCallback);
                    break;
                case "continueGame":
                    if (SaveData.Length > 0)
                    {
                        LoadFilePanelInstance = UIManager
                            .Instantiate(loadFilePanelPrefab)
                            .GetComponent<LoadFilePanel>();
                        LoadFilePanelInstance.Setup(
                            SaveData,
                            (saveData) =>
                            {
                                Dynamic.Party.InitializeBySaveData(saveData);
                                Map.PlayerController.WaitCount++;
                                ScreenManager.FadeOut(() =>
                                {
                                    Destroy(LoadFilePanelInstance.gameObject);
                                    LoadFileFadeOutCallback(saveData);
                                });
                            }
                        );
                    }
                    break;
                case "setting":
                    UIManager.Instantiate(settingPanelPrefab);
                    break;
                case "endGame":
                    Application.Quit();
                    break;
            }
        }

        private static void NewGameFadeOutCallback()
        {
            SceneManager.UnloadSceneAsync("Title");
            Map.PlayerParty.StartPosition = ResourceManager.GameInfo.startPosition;
            SceneManager.LoadScene("MapRoot", LoadSceneMode.Additive);
            MapManager.GoToNewMap(
                ResourceManager.GameInfo.startMapName,
                () => ScreenManager.FadeIn(() => Map.PlayerController.WaitCount--)
            );
        }

        private static void LoadFileFadeOutCallback(Static.SaveData saveData)
        {
            SceneManager.UnloadSceneAsync("Title");
            Map.PlayerParty.StartPosition = new Vector3(
                saveData.startPosition.x,
                saveData.startPosition.y,
                0
            );
            SceneManager.LoadScene("MapRoot", LoadSceneMode.Additive);
            MapManager.GoToNewMap(
                saveData.startMapName,
                () => ScreenManager.FadeIn(() => Map.PlayerController.WaitCount--)
            );
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                {
                    var v = ((string, string, bool))data;
                    c.textComponent.text = v.Item1;
                    c.textComponent.color = v.Item3 ? Color.white : Color.gray;
                }
                else
                {
                    c.textComponent.text = " ";
                }
            }
        }
    }
}
