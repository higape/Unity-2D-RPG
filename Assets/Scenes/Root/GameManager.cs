using System.Collections;
using System.Collections.Generic;
using System.IO;
using Static;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    /// <summary>
    /// 加载游戏数据并处理错误
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            ResourceManager.CheckAndCreatePath();
            SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        }
    }
}
