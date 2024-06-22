using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ActorUsableItemStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI consumableContent;

        [SerializeField]
        private TextMeshProUGUI occasionLabel;

        [SerializeField]
        private TextMeshProUGUI occasionContent;

        [SerializeField]
        private TextMeshProUGUI sellingPriceLabel;

        [SerializeField]
        private TextMeshProUGUI sellingPriceContent;

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
        private ListBox effectListBox;

        private void Awake()
        {
            occasionLabel.text = ResourceManager.Term.usedOccasion;
            sellingPriceLabel.text = ResourceManager.Term.sellingPrice;
            elementLabel.text = ResourceManager.Term.element;
            scopeLabel.text = ResourceManager.Term.scope;
            waitTimeLabel.text = ResourceManager.Term.waitTime;
        }

        public void Refresh(Dynamic.ActorUsableItem item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            consumableContent.text = item.Consumable
                ? ResourceManager.Term.consumable
                : ResourceManager.Term.notConsume;
            if (item.UsedInMenu)
            {
                occasionContent.text = ResourceManager.Term.menu;
                if (item.UsedInBattle)
                {
                    occasionContent.text += '/' + ResourceManager.Term.battle;
                }
            }
            else if (item.UsedInBattle)
            {
                occasionContent.text = ResourceManager.Term.battle;
            }
            else
            {
                occasionContent.text = "";
            }
            sellingPriceContent.text = item.SellingPrice.ToString();
            var usage = item.Usage;
            elementContent.text = ResourceManager.Term.GetText(usage.element);
            if (usage.attackCount > 1)
            {
                scopeContent.text = string.Format(
                    ResourceManager.Term.scopeStatement,
                    ResourceManager.Term.GetText(usage.scope),
                    usage.attackCount
                );
            }
            else
            {
                scopeContent.text = ResourceManager.Term.GetText(usage.scope);
            }
            waitTimeContent.text = usage.waitTime.ToString() + ResourceManager.Term.round;
            effectListBox.Initialize(
                1,
                Mathf.Max(usage.effects.Length, 3),
                RefreshListItem,
                usage.effects
            );
            effectListBox.Unselect();
        }

        private void RefreshListItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data is Static.BattleEffectData item)
                {
                    if (item.duration > 0)
                    {
                        c.textComponent.text = item.GetDurationState().Name;
                    }
                    else
                    {
                        c.textComponent.text = item.GetBattleEffect().Name;
                    }
                }
                else
                {
                    c.textComponent.text = " ";
                }
            }
        }
    }
}
