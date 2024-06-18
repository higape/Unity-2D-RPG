using UnityEngine;

namespace Root
{
    /// <summary>
    /// 音频管理器，通过此类播放声音
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioSource BgmSource { get; set; }
        private static AudioSource MeSource { get; set; }
        private static AudioSource[] SeSources { get; set; }
        private static int SeIndex { get; set; } = 0;
        private static AudioClip CursorMove { get; set; }
        private static AudioClip Interact { get; set; }
        private static AudioClip Cancel { get; set; }

        public static void SetBgmVolume(float volume) => BgmSource.volume = volume;

        public static void SetSeVolume(float volume)
        {
            foreach (var s in SeSources)
            {
                s.volume = volume;
            }
        }

        public static void PlayBgm(AudioClip clip)
        {
            BgmSource.clip = clip;
            BgmSource.Play();
        }

        public static void PlayMe(AudioClip clip)
        {
            MeSource.clip = clip;
            MeSource.Play();
        }

        public static void PlaySe(AudioClip clip)
        {
            if (SeIndex >= SeSources.Length)
                SeIndex = 0;

            SeSources[SeIndex].clip = clip;
            SeSources[SeIndex].Play();
            ++SeIndex;
        }

        public static void PlayMoveSe()
        {
            PlaySe(CursorMove);
        }

        public static void PlayPageSe()
        {
            PlaySe(CursorMove);
        }

        public static void PlayInteractSe()
        {
            PlaySe(Interact);
        }

        public static void PlayCancelSe()
        {
            PlaySe(Cancel);
        }

        public static void PlayMainMenuSe()
        {
            PlaySe(Interact);
        }

        public static void PlaySubMenuSe()
        {
            PlaySe(Interact);
        }

        [SerializeField]
        private AudioSource bgmSource;

        [SerializeField]
        private AudioSource meSource;

        [SerializeField]
        private AudioSource[] seSources;

        [SerializeField]
        private AudioClip cursorMove;

        [SerializeField]
        private AudioClip interact;

        [SerializeField]
        private AudioClip cancel;

        private void Awake()
        {
            BgmSource = bgmSource;
            MeSource = meSource;
            SeSources = seSources;
            CursorMove = cursorMove;
            Interact = interact;
            Cancel = cancel;
        }
    }
}
