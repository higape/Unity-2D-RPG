using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 管理地图场景的UI。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager Instance { get; set; }

        public static GameObject Instantiate(GameObject original) =>
            Instantiate(original, Instance.transform);

        public static void StartMessage(string text, UnityAction callback)
        {
            Instantiate(Instance.messagePanelPrefab, Instance.transform)
                .GetComponent<MessagePanel>()
                .StartMessage(text, callback);
        }

        public static void StartMessage(IList<string> texts, UnityAction callback)
        {
            Instantiate(Instance.messagePanelPrefab, Instance.transform)
                .GetComponent<MessagePanel>()
                .StartMessage(texts, callback);
        }

        public static void StartChoice(string text, IList data, UnityAction<int> callback)
        {
            Instantiate(Instance.choiceListPanelPrefab, Instance.transform)
                .GetComponent<ChoiceListPanel>()
                .StartChoice(text, data, callback);
        }

        public static void OpenShop(int id, UnityAction callback)
        {
            Instantiate(Instance.shopPanelPrefab, Instance.transform)
                .GetComponent<ShopPanel>()
                .Setup(id, callback);
        }

        [SerializeField]
        private GameObject messagePanelPrefab;

        [SerializeField]
        private GameObject choiceListPanelPrefab;

        [SerializeField]
        private GameObject shopPanelPrefab;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
