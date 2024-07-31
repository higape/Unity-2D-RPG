using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActorEquipmentStatusPanel : MonoBehaviour
    {
        [SerializeField]
        private Image displayObject;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI abilityLabel;

        [SerializeField]
        private GameObject abilityGroup;

        [SerializeField]
        private GameObject abilityPrefab;

        [SerializeField]
        private TextMeshProUGUI elementLabel;

        [SerializeField]
        private GameObject elementGroup;

        [SerializeField]
        private GameObject elementPrefab;

        private List<TextItem2> AbilityItems { get; set; }

        private List<ImageTextItem> ElementItems { get; set; }

        private TextItem2 AtkItem => AbilityItems[4];

        private void Awake()
        {
            elementLabel.text = ResourceManager.Term.elementResistance;
            abilityLabel.text = ResourceManager.Term.ability;

            AbilityItems = new();
            for (int i = 0; i < 9; i++)
            {
                AbilityItems.Add(
                    Instantiate(abilityPrefab, abilityGroup.transform).GetComponent<TextItem2>()
                );
            }
            AbilityItems[0].textComponent0.text = ResourceManager.Term.lv;
            AbilityItems[1].textComponent0.text = ResourceManager.Term.exp;
            AbilityItems[2].textComponent0.text = ResourceManager.Term.nextExp;
            AbilityItems[3].textComponent0.text = ResourceManager.Term.hp;
            AbilityItems[4].textComponent0.text = ResourceManager.Term.atk;
            AbilityItems[5].textComponent0.text = ResourceManager.Term.def;
            AbilityItems[6].textComponent0.text = ResourceManager.Term.agi;
            AbilityItems[7].textComponent0.text = ResourceManager.Term.hit;
            AbilityItems[8].textComponent0.text = ResourceManager.Term.eva;

            ElementItems = new();
            for (int i = 0; i < 8; i++)
            {
                ElementItems.Add(
                    Instantiate(elementPrefab, elementGroup.transform).GetComponent<ImageTextItem>()
                );
                ElementItems[i].image.sprite = ResourceManager.GetElementSprite(
                    (Static.ElementType)i
                );
            }
        }

        public void Refresh(Actor actor, ActorWeapon aw)
        {
            if (aw != null)
            {
                AtkItem.textComponent1.color = Color.green;
                AtkItem.textComponent1.text = (actor.Atk + aw.Attack).ToString();
            }
            else
            {
                AtkItem.textComponent1.color = Color.white;
                AtkItem.textComponent1.text = actor.Atk.ToString();
            }

            displayObject.sprite = actor.BattleSkin.idle;
            nameContent.text = actor.Name;

            AbilityItems[0].textComponent1.text = actor.Level.ToString();
            AbilityItems[1].textComponent1.text = actor.Exp.ToString();
            AbilityItems[2].textComponent1.text = actor.NextExp.ToString();
            AbilityItems[3].textComponent1.text = actor.Hp.ToString();
            //ignore atk here
            AbilityItems[5].textComponent1.text = actor.Def.ToString();
            AbilityItems[6].textComponent1.text = actor.Agi.ToString();
            AbilityItems[7].textComponent1.text = actor.Hit.ToString();
            AbilityItems[8].textComponent1.text = actor.Eva.ToString();

            var eg = actor.GetElementGroup();
            ElementItems[0].text.text = eg.Normal.ToString();
            ElementItems[1].text.text = eg.Corrosion.ToString();
            ElementItems[2].text.text = eg.Fire.ToString();
            ElementItems[3].text.text = eg.Ice.ToString();
            ElementItems[4].text.text = eg.Electricity.ToString();
            ElementItems[5].text.text = eg.Wave.ToString();
            ElementItems[6].text.text = eg.Ray.ToString();
            ElementItems[7].text.text = eg.Gas.ToString();
        }
    }
}
