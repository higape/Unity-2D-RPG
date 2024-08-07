using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActorWeaponStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI atkLabel;

        [SerializeField]
        private TextMeshProUGUI atkContent;

        [SerializeField]
        private TextMeshProUGUI sellingPriceLabel;

        [SerializeField]
        private TextMeshProUGUI sellingPriceContent;

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
        private TextMeshProUGUI equipableLabel;

        [SerializeField]
        private ListBox equipableListBox;

        [SerializeField]
        private TextMeshProUGUI effectLabel;

        [SerializeField]
        private ListBox effectListBox;

        private void Awake()
        {
            atkLabel.text = ResourceManager.Term.atk;
            equipableLabel.text = ResourceManager.Term.equipable;
            sellingPriceLabel.text = ResourceManager.Term.sellingPrice;
            elementLabel.text = ResourceManager.Term.element;
            scopeLabel.text = ResourceManager.Term.scope;
            coolingTimeLabel.text = ResourceManager.Term.coolingTime;
            effectLabel.text = ResourceManager.Term.effect;
        }

        public void Refresh(Dynamic.ActorWeapon item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            atkContent.text = item.Attack.ToString();
            sellingPriceContent.text = item.SellingPrice.ToString();

            var usage = item.GetUsage(0);
            elementContent.text = ResourceManager.Term.GetText(usage.element);
            elementImage.sprite = ResourceManager.Spriteset.GetElementSprite(usage.element);
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
            coolingTimeContent.text = string.Format(
                ResourceManager.Term.coolingTimeStatement,
                usage.coolingTime
            );

            var equipableActors = item.GetEquipableActorList();
            equipableListBox.Initialize(
                1,
                equipableActors.Count,
                RefreshEquipableItem,
                equipableActors
            );
            equipableListBox.Unselect();

            effectListBox.Initialize(
                1,
                Mathf.Max(usage.effects.Length, 3),
                RefreshEffectItem,
                usage.effects
            );
            effectListBox.Unselect();
        }

        private void RefreshEquipableItem(ListBoxItem listItem, object data)
        {
            if (listItem is ImageItem c)
            {
                if (data is Dynamic.Actor actor)
                {
                    c.image.sprite = actor.BattleSkin.idle;
                }
                else
                {
                    c.image.sprite = null;
                }
            }
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
