using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ActorEquipmentPanel : MonoBehaviour
    {
        #region Prefab & List

        [SerializeField]
        private GameObject labelPrefab;

        [SerializeField]
        private GameObject labelLayer;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private GameObject weaponPanelPrefab;

        [SerializeField]
        private GameObject armorPanelPrefab;

        #endregion

        #region Status UI

        [SerializeField]
        private Image displayObject;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI lvLabel;

        [SerializeField]
        private TextMeshProUGUI lvContent;

        [SerializeField]
        private TextMeshProUGUI hpLabel;

        [SerializeField]
        private TextMeshProUGUI hpContent;

        [SerializeField]
        private TextMeshProUGUI atkLabel;

        [SerializeField]
        private TextMeshProUGUI atkContent;

        [SerializeField]
        private TextMeshProUGUI defLabel;

        [SerializeField]
        private TextMeshProUGUI defContent;

        [SerializeField]
        private TextMeshProUGUI agiLabel;

        [SerializeField]
        private TextMeshProUGUI agiContent;

        [SerializeField]
        private TextMeshProUGUI hitLabel;

        [SerializeField]
        private TextMeshProUGUI hitContent;

        [SerializeField]
        private TextMeshProUGUI evaLabel;

        [SerializeField]
        private TextMeshProUGUI evaContent;

        [SerializeField]
        private TextMeshProUGUI expLabel;

        [SerializeField]
        private TextMeshProUGUI expContent;

        [SerializeField]
        private TextMeshProUGUI nextExpLabel;

        [SerializeField]
        private TextMeshProUGUI nextExpContent;

        #endregion

        [SerializeField]
        private ActorWeaponStatistic weaponStatistic;

        [SerializeField]
        private ActorArmorStatistic armorStatistic;

        private Actor CurrentActor { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
                new(InputCommand.ButtonMainMenu, ButtonType.Down, NotEquip),
            };

            lvLabel.text = ResourceManager.Term.lv;
            hpLabel.text = ResourceManager.Term.hp;
            atkLabel.text = ResourceManager.Term.atk;
            defLabel.text = ResourceManager.Term.def;
            agiLabel.text = ResourceManager.Term.agi;
            hitLabel.text = ResourceManager.Term.hit;
            evaLabel.text = ResourceManager.Term.eva;
            expLabel.text = ResourceManager.Term.exp;
            nextExpLabel.text = ResourceManager.Term.nextExp;

            string[] labelTexts = new string[Actor.WeaponCount + Actor.ArmorCount]
            {
                ResourceManager.Term.weaponPart,
                ResourceManager.Term.weaponPart,
                ResourceManager.Term.weaponPart,
                ResourceManager.Term.headPart,
                ResourceManager.Term.bodyPart,
                ResourceManager.Term.handPart,
                ResourceManager.Term.footPart,
                ResourceManager.Term.ornamentPart,
            };

            foreach (var labelText in labelTexts)
            {
                Instantiate(labelPrefab, labelLayer.transform)
                    .GetComponentInChildren<TextMeshProUGUI>()
                    .text = labelText;
            }

            itemListBox.Initialize(1, Actor.WeaponCount + Actor.ArmorCount, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorEquipmentPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorEquipmentPanel));
        }

        public void SetActor(Actor actor)
        {
            CurrentActor = actor;
            Refresh();
        }

        private void Refresh()
        {
            RefreshEquipment();
            RefreshStatus();
        }

        private void RefreshEquipment()
        {
            var items = CurrentActor.GetAllEquipments();
            itemListBox.SetSource(items);
        }

        private void RefreshStatus()
        {
            displayObject.sprite = CurrentActor.BattleSkin.idle;
            nameContent.text = CurrentActor.Name;
            lvContent.text = CurrentActor.Level.ToString();
            hpContent.text = CurrentActor.Hp.ToString() + '/' + CurrentActor.Mhp.ToString();

            if (
                itemListBox.SelectedIndex < Actor.WeaponCount
                && itemListBox.SelectedItem is ActorWeapon aw
            )
            {
                atkContent.color = Color.green;
                atkContent.text = (CurrentActor.Atk + aw.Attack).ToString();
            }
            else
            {
                atkContent.color = Color.white;
                atkContent.text = CurrentActor.Atk.ToString();
            }

            defContent.text = CurrentActor.Def.ToString();
            agiContent.text = CurrentActor.Agi.ToString();
            hitContent.text = CurrentActor.Hit.ToString();
            evaContent.text = CurrentActor.Eva.ToString();
            expContent.text = CurrentActor.Exp.ToString();
            nextExpContent.text = CurrentActor.NextExp.ToString();
        }

        private void OnSelectedItemChange(object item, int index)
        {
            if (itemListBox.SelectedItem is ActorWeapon aw)
            {
                atkContent.color = Color.green;
                atkContent.text = (CurrentActor.Atk + aw.Attack).ToString();
                weaponStatistic.Refresh(aw);
                armorStatistic.Refresh(null);
            }
            else if (itemListBox.SelectedItem is ActorArmor aa)
            {
                atkContent.color = Color.white;
                atkContent.text = CurrentActor.Atk.ToString();
                weaponStatistic.Refresh(null);
                armorStatistic.Refresh(aa);
            }
            else
            {
                atkContent.color = Color.white;
                atkContent.text = CurrentActor.Atk.ToString();
                weaponStatistic.Refresh(null);
                armorStatistic.Refresh(null);
            }
        }

        private void WeaponListCallback(object listObject)
        {
            if (listObject is QuantityList.ListItem item)
            {
                CurrentActor.EquipWeapon(itemListBox.SelectedIndex, item.id);
                Refresh();
            }
        }

        private void ArmorListCallback(object listObject)
        {
            if (listObject is QuantityList.ListItem item)
            {
                CurrentActor.EquipArmor(itemListBox.SelectedIndex - Actor.WeaponCount, item.id);
                Refresh();
            }
        }

        private void Interact()
        {
            if (itemListBox.SelectedIndex < Actor.WeaponCount)
                UIManager
                    .Instantiate(weaponPanelPrefab)
                    .GetComponent<ActorWeaponList>()
                    .Setup(CurrentActor, WeaponListCallback);
            else
                UIManager
                    .Instantiate(armorPanelPrefab)
                    .GetComponent<ActorArmorList>()
                    .Setup(
                        itemListBox.SelectedIndex - Actor.WeaponCount,
                        CurrentActor,
                        ArmorListCallback
                    );
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void NotEquip()
        {
            if (itemListBox.SelectedIndex < Actor.WeaponCount)
                CurrentActor.EquipWeapon(itemListBox.SelectedIndex, 0);
            else
                CurrentActor.EquipArmor(itemListBox.SelectedIndex - Actor.WeaponCount, 0);
            Refresh();
        }

        private void RefreshLabel(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data is string s)
                    c.textComponent.text = s;
                else
                    c.textComponent.text = " ";
            }
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data is ActorWeapon weapon)
                    c.textComponent.text = weapon.Name;
                else if (data is ActorArmor armor)
                    c.textComponent.text = armor.Name;
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
