using System;
using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActorArmorStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private GameObject abilityPiece;

        [SerializeField]
        private GameObject abilityPrefab;

        [SerializeField]
        private TextMeshProUGUI elementLabel;

        [SerializeField]
        private GameObject elementPiece;

        [SerializeField]
        private GameObject elementPrefab;

        [SerializeField]
        private TextMeshProUGUI equipableLabel;

        [SerializeField]
        private GameObject equipablePiece;

        [SerializeField]
        private GameObject equipablePrefab;

        [SerializeField]
        private TextMeshProUGUI traitLabel;

        [SerializeField]
        private ListBox traitListBox;

        [SerializeField]
        private TextMeshProUGUI sellingPriceLabel;

        [SerializeField]
        private TextMeshProUGUI sellingPriceContent;

        private List<TextItem2> AbilityItems { get; set; }
        private List<ImageTextItem> ElementItems { get; set; }
        private List<Image> ImageItems { get; set; }

        private void Awake()
        {
            AbilityItems = new();
            for (int i = 0; i < 6; i++)
            {
                AbilityItems.Add(
                    Instantiate(abilityPrefab, abilityPiece.transform).GetComponent<TextItem2>()
                );
            }
            AbilityItems[0].textComponent0.text = ResourceManager.Term.hp;
            AbilityItems[1].textComponent0.text = ResourceManager.Term.atk;
            AbilityItems[2].textComponent0.text = ResourceManager.Term.def;
            AbilityItems[3].textComponent0.text = ResourceManager.Term.agi;
            AbilityItems[4].textComponent0.text = ResourceManager.Term.hit;
            AbilityItems[5].textComponent0.text = ResourceManager.Term.eva;

            ElementItems = new();
            for (int i = 0; i < 8; i++)
            {
                ElementItems.Add(
                    Instantiate(elementPrefab, elementPiece.transform).GetComponent<ImageTextItem>()
                );
                ElementItems[i].image.sprite = ResourceManager.GetElementSprite(
                    (Static.ElementType)i
                );
            }

            ImageItems = new();
            for (int i = 0; i < 8; i++)
            {
                ImageItems.Add(
                    Instantiate(equipablePrefab, equipablePiece.transform).GetComponent<Image>()
                );
            }

            elementLabel.text = ResourceManager.Term.elementResistance;
            equipableLabel.text = ResourceManager.Term.equipable;
            traitLabel.text = ResourceManager.Term.trait;
            sellingPriceLabel.text = ResourceManager.Term.sellingPrice;

            traitListBox.Initialize(1, 3, RefreshTraitItem);
        }

        public void Refresh(Dynamic.ActorArmor item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            sellingPriceContent.text = item.SellingPrice.ToString();

            AbilityItems[0].textComponent1.text = item.Hp.ToString();
            AbilityItems[1].textComponent1.text = item.Atk.ToString();
            AbilityItems[2].textComponent1.text = item.Def.ToString();
            AbilityItems[3].textComponent1.text = item.Agi.ToString();
            AbilityItems[4].textComponent1.text = item.Hit.ToString();
            AbilityItems[5].textComponent1.text = item.Eva.ToString();

            ElementItems[0].text.text = item.ElementGroup.Normal.ToString();
            ElementItems[1].text.text = item.ElementGroup.Corrosion.ToString();
            ElementItems[2].text.text = item.ElementGroup.Fire.ToString();
            ElementItems[3].text.text = item.ElementGroup.Ice.ToString();
            ElementItems[4].text.text = item.ElementGroup.Electricity.ToString();
            ElementItems[5].text.text = item.ElementGroup.Wave.ToString();
            ElementItems[6].text.text = item.ElementGroup.Ray.ToString();
            ElementItems[7].text.text = item.ElementGroup.Gas.ToString();

            var equipableActors = item.GetEquipableActorList();
            for (int i = 0; i < ImageItems.Count; i++)
            {
                if (i < equipableActors.Count)
                    ImageItems[i].sprite = equipableActors[i].BattleSkin.idle;
                else
                    ImageItems[i].sprite = null;
            }

            traitListBox.SetSource(item.Traits);
        }

        private void RefreshTraitItem(ListBoxItem arg0, object arg1)
        {
            if (arg0 is TextItem c)
            {
                if (arg1 is Static.TraitData traitData)
                    c.textComponent.text = traitData.GetStatement();
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
