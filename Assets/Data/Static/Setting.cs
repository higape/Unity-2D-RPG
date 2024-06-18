using UnityEngine;

namespace Static
{
    /// <summary>
    /// 游戏配置的保存数据
    /// </summary>
    public class Setting
    {
        [SerializeField]
        private int bgmVolumePercentage = 80;

        [SerializeField]
        private int seVolumePercentage = 80;

        public int BgmVolumePercentage
        {
            get => bgmVolumePercentage;
            set => bgmVolumePercentage = Mathf.Clamp(value, 0, 100);
        }

        public int SeVolumePercentage
        {
            get => seVolumePercentage;
            set => seVolumePercentage = Mathf.Clamp(value, 0, 100);
        }

        public float BgmVolume => BgmVolumePercentage / 100f;

        public float SeVolume => SeVolumePercentage / 100f;
    }
}
