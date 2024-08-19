using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    /// <summary>
    /// 管理各种画面效果
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        private const float Speed = 8f;

        public static ScreenManager Instance { get; private set; }

        private static UnityAction Action { get; set; }

        private static UnityAction FadeCallback { get; set; }

        private static bool IsFadeIning { get; set; }

        private static bool IsFadeOuting { get; set; }

        public static void FadeIn(UnityAction callback)
        {
            if (IsFadeOuting)
            {
                Debug.LogWarning("在淡出时请求淡入");
                callback?.Invoke();
                return;
            }
            IsFadeIning = true;
            Action = Instance.ProcessFadeIn;
            FadeCallback = callback;
        }

        public static void FadeOut(UnityAction callback)
        {
            if (IsFadeIning)
            {
                Debug.LogWarning("在淡入时请求淡出");
                callback?.Invoke();
                return;
            }
            IsFadeOuting = true;
            Action = Instance.ProcessFadeOut;
            FadeCallback = callback;
        }

        //染色
        public static void Tint() { }

        //闪烁
        public static void Flash() { }

        //摇晃
        public static void Shake() { }

        [SerializeField]
        private CanvasGroup fadeBlack;

        private void Awake()
        {
            Instance = this;
            var rect = Screen.mainWindowDisplayInfo.workArea;
            if (rect.width >= 1280 && rect.height >= 720)
            {
                Screen.SetResolution(1280, 720, false);
            }
        }

        private void Update()
        {
            Action?.Invoke();
        }

        private void ProcessFadeIn()
        {
            if (fadeBlack.alpha > 0)
            {
                fadeBlack.alpha -= Time.deltaTime * Speed;
            }
            else
            {
                IsFadeIning = false;
                Action = null;
                FadeCallback?.Invoke();
            }
        }

        private void ProcessFadeOut()
        {
            if (fadeBlack.alpha < 1)
            {
                fadeBlack.alpha += Time.deltaTime * Speed;
            }
            else
            {
                IsFadeOuting = false;
                Action = null;
                FadeCallback?.Invoke();
            }
        }
    }
}
