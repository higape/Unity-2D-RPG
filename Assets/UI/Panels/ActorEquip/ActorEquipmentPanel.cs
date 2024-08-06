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
        [SerializeField]
        private TextMeshProUGUI heading;

        [SerializeField]
        private CanvasGroup equippedLayer;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorEquipmentStatusPanel statusPanel;

        [SerializeField]
        private GameObject weaponPanelPrefab;

        [SerializeField]
        private GameObject armorPanelPrefab;

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
            };

            heading.text = ResourceManager.Term.equipment;

            itemListBox.Initialize(1, Actor.WeaponCount + Actor.ArmorCount, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback((o, i) => RefreshBySelectedItem());
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
            //为装备栏添加图标
            var es = CurrentActor.GetAllEquipments();
            List<(Sprite, object)> list = new();
            for (int i = 0; i < es.Count; i++)
            {
                if (i < Actor.WeaponCount)
                    list.Add((ResourceManager.Spriteset.GetActorWeaponSprite(), es[i]));
                else
                    list.Add(
                        (
                            ResourceManager.Spriteset.GetActorArmorSprite(i - Actor.WeaponCount),
                            es[i]
                        )
                    );
            }
            itemListBox.SetSource(list);

            RefreshBySelectedItem();
        }

        private void RefreshBySelectedItem()
        {
            statusPanel.Refresh(CurrentActor, itemListBox.SelectedItem as ActorWeapon);
            RefreshStat(itemListBox.SelectedItem);
        }

        private void RefreshStat(object item)
        {
            if (item == null)
                return;

            var statItem = (((Sprite, object))item).Item2;
            if (statItem is ActorWeapon aw)
            {
                weaponStatistic.Refresh(aw);
                armorStatistic.Refresh(null);
            }
            else if (statItem is ActorArmor aa)
            {
                weaponStatistic.Refresh(null);
                armorStatistic.Refresh(aa);
            }
            else
            {
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
            else if (listObject is int)
            {
                CurrentActor.EquipWeapon(itemListBox.SelectedIndex, 0);
                Refresh();
            }
            else
            {
                Refresh();
            }

            equippedLayer.alpha = 1;
        }

        private void ArmorListCallback(object listObject)
        {
            if (listObject is QuantityList.ListItem item)
            {
                CurrentActor.EquipArmor(itemListBox.SelectedIndex - Actor.WeaponCount, item.id);
                Refresh();
            }
            else if (listObject is int)
            {
                CurrentActor.EquipArmor(itemListBox.SelectedIndex - Actor.WeaponCount, 0);
                Refresh();
            }
            else
            {
                Refresh();
            }

            equippedLayer.alpha = 1;
        }

        private void Interact()
        {
            equippedLayer.alpha = 0;
            if (itemListBox.SelectedIndex < Actor.WeaponCount)
                UIManager
                    .Instantiate(weaponPanelPrefab)
                    .GetComponent<ActorWeaponList>()
                    .Setup(CurrentActor, WeaponListCallback, weaponStatistic);
            else
                UIManager
                    .Instantiate(armorPanelPrefab)
                    .GetComponent<ActorArmorList>()
                    .Setup(
                        itemListBox.SelectedIndex - Actor.WeaponCount,
                        CurrentActor,
                        ArmorListCallback,
                        armorStatistic
                    );
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is ImageTextItem c)
            {
                var item = ((Sprite, object))data;
                c.image.sprite = item.Item1;
                if (item.Item2 is ActorWeapon weapon)
                    c.text.text = weapon.Name;
                else if (item.Item2 is ActorArmor armor)
                    c.text.text = armor.Name;
                else
                    c.text.text = " ";
            }
        }
    }
}
