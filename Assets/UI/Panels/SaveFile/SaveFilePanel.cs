using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SaveFilePanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private ListBox listBox;

        private Static.SaveData[] SaveData { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            heading.text = ResourceManager.Term.saveFile;

            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, listBox.SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, listBox.SelectRight),
                new(InputCommand.ButtonPrevious, ButtonType.Down, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            List<object> list = new();
            SaveData = ResourceManager.LoadSaveInfo();
            if (SaveData != null)
            {
                if (SaveData.Length < 100)
                    list.Add(-1);
                list.AddRange(SaveData);
            }
            else
            {
                list.Add(-1);
            }

            listBox.Initialize(2, 5, RefreshItem, list);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SaveFilePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SaveFilePanel));
        }

        private void Interact()
        {
            if (listBox.SelectedItem is Static.SaveData oldSave)
            {
                List<string> texts =
                    new() { ResourceManager.Term.confirm, ResourceManager.Term.cancel };
                UIManager.StartChoice(
                    ResourceManager.Term.confirmOverwriteSaveFile,
                    texts,
                    (index) =>
                    {
                        if (index == 0)
                        {
                            var data = Party.CreateSaveData();
                            data.name = oldSave.name;
                            if (ResourceManager.SaveFile(data))
                                OnSaveSucceeded(data);
                            else
                                OnSaveFailed();
                        }
                    }
                );
            }
            else if (listBox.SelectedItem is int)
            {
                var data = Party.CreateSaveData();
                if (ResourceManager.SaveNewFile(data))
                    OnSaveSucceeded(data);
                else
                    OnSaveFailed();
            }
        }

        private void OnSaveSucceeded(Static.SaveData data)
        {
            //刷新窗口的存档信息
            listBox.ItemsSource[listBox.SelectedIndex] = data;
            listBox.Refresh();
            //弹出存档成功消息
            UIManager.StartMessage(ResourceManager.Term.saveFileSucceed, () => Destroy(gameObject));
        }

        private void OnSaveFailed()
        {
            UIManager.StartMessage(ResourceManager.Term.saveFileFail, null);
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem3 c)
            {
                if (data is Static.SaveData save)
                {
                    c.textComponent0.text = save.name;
                    c.textComponent1.text =
                        save.year.ToString() + '/' + save.month + '/' + save.day;
                    c.textComponent2.text =
                        save.hour.ToString().PadLeft(2, '0')
                        + ':'
                        + save.minute.ToString().PadLeft(2, '0')
                        + ':'
                        + save.second.ToString().PadLeft(2, '0');
                }
                else if (data is int)
                {
                    c.textComponent0.text = ResourceManager.Term.createFile;
                    c.textComponent1.text = string.Empty;
                    c.textComponent2.text = string.Empty;
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                    c.textComponent2.text = " ";
                }
            }
        }
    }
}
