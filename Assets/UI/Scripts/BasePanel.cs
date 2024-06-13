using System;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 游戏内窗口的基类。
    /// 通过脚本或事件打开或关闭。
    /// 需要在事件处调用过渡。
    /// </summary>
    public class BasePanel : MonoBehaviour
    {
        //提供给事件调用
        protected static void PlayInteractSe() => AudioManager.PlayInteractSe();

        protected static void PlayCancelSe() => AudioManager.PlayCancelSe();
    }
}
