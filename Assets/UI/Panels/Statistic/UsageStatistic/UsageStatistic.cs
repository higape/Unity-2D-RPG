using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UsageStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI elementLabel;

        [SerializeField]
        private TextMeshProUGUI elementContent;

        [SerializeField]
        private TextMeshProUGUI scopeLabel;

        [SerializeField]
        private TextMeshProUGUI scopeContent;

        [SerializeField]
        private TextMeshProUGUI waitTimeLabel;

        [SerializeField]
        private TextMeshProUGUI waitTimeContent;

        [SerializeField]
        private TextMeshProUGUI effectLabel;

        [SerializeField]
        private ListBox effectListBox;

        private void Awake()
        {
            elementLabel.text = ResourceManager.Term.element;
            scopeLabel.text = ResourceManager.Term.scope;
            waitTimeLabel.text = ResourceManager.Term.waitTime;
            effectLabel.text = ResourceManager.Term.effect;
        }

        public void EffectListPageUp() => effectListBox.PageUp();

        public void EffectListPageDown() => effectListBox.PageDown();

        public void Refresh(Static.WeaponUsage item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            elementContent.text = ResourceManager.Term.GetText(item.element);
            if (item.attackCount > 1)
            {
                scopeContent.text = string.Format(
                    ResourceManager.Term.scopeStatement,
                    ResourceManager.Term.GetText(item.scope),
                    item.attackCount
                );
            }
            else
            {
                scopeContent.text = ResourceManager.Term.GetText(item.scope);
            }
            waitTimeContent.text = item.waitTime.ToString() + ResourceManager.Term.round;
            effectListBox.Initialize(1, 3, RefreshEffectItem, item.effects);
            effectListBox.Unselect();
        }

        private void RefreshEffectItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data is Static.BattleEffectData item)
                    c.textComponent.text = item.GetStatement();
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
