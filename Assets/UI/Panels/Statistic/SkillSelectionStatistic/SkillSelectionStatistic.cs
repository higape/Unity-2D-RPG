using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillSelectionStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI selectionContent;

        [SerializeField]
        private TextMeshProUGUI quantityLabel;

        [SerializeField]
        private TextMeshProUGUI quantityContent;

        [SerializeField]
        private TextMeshProUGUI usedCountLabel;

        [SerializeField]
        private TextMeshProUGUI usedCountContent;

        [SerializeField]
        private TextMeshProUGUI effectRateLabel;

        [SerializeField]
        private TextMeshProUGUI effectRateContent;

        [SerializeField]
        private TextMeshProUGUI coolingTimeLabel;

        [SerializeField]
        private TextMeshProUGUI coolingTimeContent;

        [SerializeField]
        private TextMeshProUGUI addedEffectLabel;

        [SerializeField]
        private ListBox effectListBox;

        private void Awake()
        {
            quantityLabel.text = ResourceManager.Term.quantity;
            usedCountLabel.text = ResourceManager.Term.usedCount;
            effectRateLabel.text = ResourceManager.Term.effectRate;
            coolingTimeLabel.text = ResourceManager.Term.coolingTime;
            addedEffectLabel.text = ResourceManager.Term.addedEffect;
        }

        public void EffectListPageUp() => effectListBox.PageUp();

        public void EffectListPageDown() => effectListBox.PageDown();

        public void Refresh(Dynamic.Skill item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            selectionContent.text = item.SkillType switch
            {
                Static.Skill.SkillType.SelectActorWeapon
                    => string.Format(
                        ResourceManager.Term.skillUseItem,
                        ResourceManager.Term.weapon
                    ),
                Static.Skill.SkillType.SelectActorItem
                    => string.Format(
                        ResourceManager.Term.skillUseItem,
                        ResourceManager
                            .Term
                            .GetText((Static.ActorUsableItem.ItemType)item.SelectionLimit)
                    ),
                _ => " ",
            };
            quantityContent.text = item.ItemQuantity.ToString();
            usedCountContent.text = item.ItemUsedCount.ToString();
            effectRateContent.text = item.SkillEffectRatePercentage.ToString() + '%';
            coolingTimeContent.text = string.Format(
                ResourceManager.Term.coolingTimeStatement,
                item.CoolingTime
            );
            effectListBox.Initialize(1, 3, RefreshEffectItem, item.AddedEffects);
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
