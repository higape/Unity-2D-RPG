using Root;
using UnityEngine;

namespace UI
{
    //游戏配置的操作界面
    public class SettingPanel : MonoBehaviour
    {
        private const int VolumeOffset = 10;

        [SerializeField]
        private ListBoxItem[] items;

        private int currentRow = 0;

        private int CurrentRow
        {
            get => currentRow;
            set
            {
                SelectedItem.Selected = false;
                currentRow = Mathf.Clamp(value, 0, items.Length - 1);
                SelectedItem.Selected = true;
            }
        }
        private ListBoxItem SelectedItem => items[currentRow];
        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, Up),
                new(InputCommand.ButtonDown, ButtonType.Press, Down),
                new(InputCommand.ButtonLeft, ButtonType.Press, Left),
                new(InputCommand.ButtonRight, ButtonType.Press, Right),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };
        }

        private void Start()
        {
            var bgmItem = items[0] as TextItem2;
            bgmItem.textComponent0.text = ResourceManager.Term.bgm;
            bgmItem.textComponent1.text = SettingManager.BgmVolumePercentage.ToString();
            var seItem = items[1] as TextItem2;
            seItem.textComponent0.text = ResourceManager.Term.se;
            seItem.textComponent1.text = SettingManager.SeVolumePercentage.ToString();
            CurrentRow = 0;
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SettingPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SettingPanel));
        }

        private void Up()
        {
            CurrentRow--;
        }

        private void Down()
        {
            CurrentRow++;
        }

        private void Left()
        {
            switch (CurrentRow)
            {
                case 0:
                    SettingManager.BgmVolumePercentage -= VolumeOffset;
                    (SelectedItem as TextItem2).textComponent1.text = SettingManager
                        .BgmVolumePercentage
                        .ToString();
                    break;
                case 1:
                    SettingManager.SeVolumePercentage -= VolumeOffset;
                    (SelectedItem as TextItem2).textComponent1.text = SettingManager
                        .SeVolumePercentage
                        .ToString();
                    break;
            }
        }

        private void Right()
        {
            switch (CurrentRow)
            {
                case 0:
                    SettingManager.BgmVolumePercentage += VolumeOffset;
                    (SelectedItem as TextItem2).textComponent1.text = SettingManager
                        .BgmVolumePercentage
                        .ToString();
                    break;
                case 1:
                    SettingManager.SeVolumePercentage += VolumeOffset;
                    (SelectedItem as TextItem2).textComponent1.text = SettingManager
                        .SeVolumePercentage
                        .ToString();
                    break;
            }
        }

        private void Interact() { }

        private void Cancel()
        {
            SettingManager.SaveSetting();
            Destroy(gameObject);
        }
    }
}
