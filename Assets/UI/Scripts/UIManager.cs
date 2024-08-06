using System.Collections;
using System.Collections.Generic;
using System.Text;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 管理UI的类。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager Instance { get; set; }

        public static string CreateHpText(int hp, int mhp)
        {
            StringBuilder sb = new();
            sb.AppendJoin('/', hp.ToString().PadLeft(4, ' '), mhp.ToString().PadLeft(4, ' '));
            return sb.ToString();
        }

        public static string CreateSkillCountText(int current, int max)
        {
            StringBuilder sb = new();
            sb.AppendJoin('/', current.ToString().PadLeft(2, ' '), max.ToString().PadLeft(2, ' '));
            return sb.ToString();
        }

        public static string CreateOccasionText(Static.UsedOccasion occasion)
        {
            StringBuilder sb = new();
            if ((occasion & Static.UsedOccasion.Menu) != 0)
            {
                if (sb.Length > 0)
                    sb.Append('/');
                sb.Append(ResourceManager.Term.menu);
            }
            if ((occasion & Static.UsedOccasion.Battle) != 0)
            {
                if (sb.Length > 0)
                    sb.Append('/');
                sb.Append(ResourceManager.Term.battle);
            }
            return sb.ToString();
        }

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

        public static void StartChoice(string message, IList choiceTexts, UnityAction<int> callback)
        {
            Instantiate(Instance.choiceListPanelPrefab, Instance.transform)
                .GetComponent<ChoiceListPanel>()
                .StartChoice(message, choiceTexts, callback);
        }

        public static void OpenShop(int id, UnityAction callback)
        {
            Instantiate(Instance.shopPanelPrefab, Instance.transform)
                .GetComponent<ShopPanel>()
                .Setup(id, callback);
        }

        public static void OpenSystemMenu(UnityAction callback)
        {
            Instantiate(Instance.systemMenuPrefab, Instance.transform)
                .GetComponent<SystemMenuPanel>()
                .Setup(callback);
        }

        [SerializeField]
        private GameObject messagePanelPrefab;

        [SerializeField]
        private GameObject choiceListPanelPrefab;

        [SerializeField]
        private GameObject shopPanelPrefab;

        [SerializeField]
        private GameObject systemMenuPrefab;

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
