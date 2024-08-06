using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Battle
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
        private TextMeshProUGUI stateLabel;

        [SerializeField]
        private ListBox stateListBox;

        private Actor CurrentActor { get; set; }

        private UnityAction CancelCallback { get; set; }

        private List<TextItem2> AbilityItems { get; set; }

        private List<ImageTextItem> ElementItems { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel)
            };

            lvLabel.text = ResourceManager.Term.lv;
            abilityLabel.text = ResourceManager.Term.ability;
            elementLabel.text = ResourceManager.Term.elementResistance;
            stateLabel.text = ResourceManager.Term.durationState;

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
                ElementItems[i].image.sprite = ResourceManager
                    .Spriteset
                    .GetElementSprite((Static.ElementType)i);
            }

            stateListBox.Initialize(1, 8, RefreshStateItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorStatusPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorStatusPanel));
        }

        public void Setup(Actor actor, UnityAction cancelCallback)
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            Refresh();
        }

        private void Refresh()
        {
            displayImage.sprite = CurrentActor.BattleSkin.idle;
            nameContent.text = CurrentActor.Name;
            lvContent.text = CurrentActor.Level.ToString();

            AbilityItems[0].textComponent1.text = UIManager.CreateHpText(
                CurrentActor.Hp,
                CurrentActor.Mhp
            );
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

            stateListBox.SetSource(CurrentActor.GetDurationStates());
        }

        private void RefreshStateItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is DurationState item)
                {
                    c.textComponent0.text = item.Name;
                    c.textComponent1.text = string.Format(
                        ResourceManager.Term.coolingTimeStatement,
                        item.LastTurn
                    );
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                }
            }
        }

        private void Cancel()
        {
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
