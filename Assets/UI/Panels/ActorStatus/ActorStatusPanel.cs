using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActorStatusPanel : MonoBehaviour
    {
        [SerializeField]
        private Image displayImage;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI lvLabel;

        [SerializeField]
        private TextMeshProUGUI lvContent;

        [SerializeField]
        private TextMeshProUGUI expLabel;

        [SerializeField]
        private TextMeshProUGUI expContent;

        [SerializeField]
        private TextMeshProUGUI nextExpLabel;

        [SerializeField]
        private TextMeshProUGUI nextExpContent;

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

        [SerializeField]
        private TextMeshProUGUI equipmentLabel;

        [SerializeField]
        private GameObject equipmentGroup;

        [SerializeField]
        private GameObject equipmentPrefab;

        private List<Actor> Actors { get; set; }

        private int ActorIndex { get; set; }

        private Actor CurrentActor => Actors[ActorIndex];

        private List<TextItem2> AbilityItems { get; set; }

        private List<ImageTextItem> ElementItems { get; set; }

        private List<ImageTextItem> EquipmentItems { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Down, () => Select(-1)),
                new(InputCommand.ButtonDown, ButtonType.Down, () => Select(1)),
                new(InputCommand.ButtonLeft, ButtonType.Down, () => Select(-1)),
                new(InputCommand.ButtonRight, ButtonType.Down, () => Select(1)),
                new(InputCommand.ButtonPrevious, ButtonType.Down, () => Select(-1)),
                new(InputCommand.ButtonNext, ButtonType.Down, () => Select(1)),
                new(InputCommand.ButtonInteract, ButtonType.Down, () => Select(1)),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            lvLabel.text = ResourceManager.Term.lv;
            expLabel.text = ResourceManager.Term.exp;
            nextExpLabel.text = ResourceManager.Term.nextExp;
            abilityLabel.text = ResourceManager.Term.ability;
            elementLabel.text = ResourceManager.Term.elementResistance;
            equipmentLabel.text = ResourceManager.Term.equipment;

            AbilityItems = new();
            for (int i = 0; i < 6; i++)
            {
                AbilityItems.Add(
                    Instantiate(abilityPrefab, abilityGroup.transform).GetComponent<TextItem2>()
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
                    Instantiate(elementPrefab, elementGroup.transform).GetComponent<ImageTextItem>()
                );
                ElementItems[i].image.sprite = ResourceManager.GetElementSprite(
                    (Static.ElementType)i
                );
            }

            EquipmentItems = new();
            for (int i = 0; i < Actor.WeaponCount + Actor.ArmorCount; i++)
            {
                EquipmentItems.Add(
                    Instantiate(equipmentPrefab, equipmentGroup.transform)
                        .GetComponent<ImageTextItem>()
                );
            }
            //设置武器图标
            for (int i = 0; i < Actor.WeaponCount; i++)
            {
                EquipmentItems[i].image.sprite = ResourceManager.GetActorWeaponSprite();
            }
            //设置防具图标
            for (int i = Actor.WeaponCount; i < Actor.WeaponCount + Actor.ArmorCount; i++)
            {
                EquipmentItems[i].image.sprite = ResourceManager.GetActorArmorSprite(
                    i - Actor.WeaponCount
                );
            }
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorStatusPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorStatusPanel));
        }

        public void Setup(List<Actor> actors, int index)
        {
            Actors = actors;
            ActorIndex = index;
            Refresh();
        }

        private void Refresh()
        {
            displayImage.sprite = CurrentActor.BattleSkin.idle;
            nameContent.text = CurrentActor.Name;
            lvContent.text = CurrentActor.Level.ToString();
            expContent.text = CurrentActor.Exp.ToString();
            nextExpContent.text = CurrentActor.NextExp.ToString();

            AbilityItems[0].textComponent1.text =
                CurrentActor.Hp.ToString() + '/' + CurrentActor.Mhp.ToString();
            AbilityItems[1].textComponent1.text = CurrentActor.Atk.ToString();
            AbilityItems[2].textComponent1.text = CurrentActor.Def.ToString();
            AbilityItems[3].textComponent1.text = CurrentActor.Agi.ToString();
            AbilityItems[4].textComponent1.text = CurrentActor.Hit.ToString();
            AbilityItems[5].textComponent1.text = CurrentActor.Eva.ToString();

            var elementGroup = CurrentActor.GetElementGroup();
            ElementItems[0].text.text = elementGroup.Normal.ToString();
            ElementItems[1].text.text = elementGroup.Corrosion.ToString();
            ElementItems[2].text.text = elementGroup.Fire.ToString();
            ElementItems[3].text.text = elementGroup.Ice.ToString();
            ElementItems[4].text.text = elementGroup.Electricity.ToString();
            ElementItems[5].text.text = elementGroup.Wave.ToString();
            ElementItems[6].text.text = elementGroup.Ray.ToString();
            ElementItems[7].text.text = elementGroup.Gas.ToString();

            var equipments = CurrentActor.GetAllEquipments();
            for (int i = 0; i < equipments.Count; i++)
            {
                if (equipments[i] is ActorWeapon aw)
                    EquipmentItems[i].text.text = aw.Name;
                else if (equipments[i] is ActorArmor aa)
                    EquipmentItems[i].text.text = aa.Name;
                else
                    EquipmentItems[i].text.text = " ";
            }
        }

        private void Select(int delta)
        {
            ActorIndex += delta;

            if (ActorIndex < 0)
                ActorIndex = Actors.Count - 1;
            else if (ActorIndex >= Actors.Count)
                ActorIndex = 0;
            Refresh();
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }
    }
}
