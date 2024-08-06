using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private Image elementImage;

        [SerializeField]
        private TextMeshProUGUI scopeLabel;

        [SerializeField]
        private TextMeshProUGUI scopeContent;

        [SerializeField]
        private TextMeshProUGUI coolingTimeLabel;

        [SerializeField]
        private TextMeshProUGUI coolingTimeContent;

        [SerializeField]
        private TextMeshProUGUI effectLabel;

        [SerializeField]
        private ListBox effectListBox;

        private void Awake()
        {
            elementLabel.text = ResourceManager.Term.element;
            scopeLabel.text = ResourceManager.Term.scope;
            coolingTimeLabel.text = ResourceManager.Term.coolingTime;
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
            elementImage.sprite = ResourceManager.Spriteset.GetElementSprite(item.element);
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
            coolingTimeContent.text = string.Format(
                ResourceManager.Term.coolingTimeStatement,
                item.coolingTime
            );
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
