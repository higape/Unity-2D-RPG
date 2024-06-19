using System.IO;
using Static;
using UnityEngine;

namespace Root
{
    //管理游戏配置的类，负责配置数据的加载、保存和应用
    public class SettingManager : MonoBehaviour
    {
        private static Setting GlobalSetting { get; set; }

        public static int BgmVolumePercentage
        {
            get => GlobalSetting.BgmVolumePercentage;
            set
            {
                GlobalSetting.BgmVolumePercentage = value;
                AudioManager.SetBgmVolume(BgmVolume);
            }
        }

        public static float BgmVolume => GlobalSetting.BgmVolume;

        public static int SeVolumePercentage
        {
            get => GlobalSetting.SeVolumePercentage;
            set
            {
                GlobalSetting.SeVolumePercentage = value;
                AudioManager.SetSeVolume(SeVolume);
            }
        }

        public static float SeVolume => GlobalSetting.SeVolume;

        public static void SaveSetting()
        {
            ResourceManager.SaveJson(
                Path.Combine(ResourceManager.SaveDirectory, "Setting.json"),
                GlobalSetting
            );
        }

        private void Awake()
        {
            GlobalSetting = ResourceManager.LoadJson<Setting>(
                Path.Combine(ResourceManager.SaveDirectory, "Setting.json")
            );
            if (GlobalSetting == null)
            {
                GlobalSetting = CreateDefaultSetting();
                SaveSetting();
            }
        }

        private void Start()
        {
            AudioManager.SetBgmVolume(BgmVolume);
            AudioManager.SetSeVolume(SeVolume);
        }

        private Setting CreateDefaultSetting()
        {
            Setting s = new();
            return s;
        }
    }
}
