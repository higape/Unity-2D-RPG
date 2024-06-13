using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Root
{
    /// <summary>
    /// 管理地图加载的类。
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        private static MapManager Instance { get; set; }
        public static string CurrentMapSceneName { get; private set; }
        private static string NextMapName { get; set; }
        private static UnityAction MapLoaded { get; set; }
        private static bool IsSceneChanging { get; set; } = false;

        public static void GoToNewMap(string mapName, UnityAction callback)
        {
            if (IsSceneChanging)
            {
                Debug.LogError(
                    $"在地图未切换完成时请求新的地图,current:{CurrentMapSceneName},loading:{NextMapName},new:{mapName}"
                );
                return;
            }

            IsSceneChanging = true;
            NextMapName = mapName;
            MapLoaded = callback;
            Instance.StartUnload();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void StartUnload()
        {
            if (CurrentMapSceneName == string.Empty || CurrentMapSceneName == null)
            {
                StartLoad();
            }
            else
            {
                StartCoroutine(UnloadMapAsync(CurrentMapSceneName, StartLoad));
            }
        }

        private void StartLoad()
        {
            StartCoroutine(LoadMapAsync(NextMapName, OnMapLoaded));
        }

        private void OnMapLoaded()
        {
            CurrentMapSceneName = NextMapName;
            NextMapName = string.Empty;
            MapLoaded?.Invoke();
            MapLoaded = null;
        }

        /// <summary>
        /// 异步加载地图
        /// </summary>
        private IEnumerator LoadMapAsync(string mapName, UnityAction callback)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            IsSceneChanging = false;
            callback?.Invoke();
        }

        /// <summary>
        /// 异步卸载地图
        /// </summary>
        private IEnumerator UnloadMapAsync(string mapName, UnityAction callback)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(mapName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            callback?.Invoke();
        }
    }
}
