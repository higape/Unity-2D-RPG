using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 地图主菜单
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private ActorStatusItem[] actorStatusItems;

        [SerializeField]
        private GameObject itemTypePrefab;

        [SerializeField]
        private GameObject actorSkillPrefab;

        [SerializeField]
        private GameObject actorEquipmentPrefab;

        [SerializeField]
        private GameObject actorStatusPrefab;

        [SerializeField]
        private GameObject settingPanelPrefab;

        [SerializeField]
        private TextMeshProUGUI goldLabel;

        [SerializeField]
        private TextMeshProUGUI goldContent;

        private UnityAction Callback { get; set; }
        private int SelectedIndex { get; set; }
        private bool IsSelectActor { get; set; }
        private InputCommand[] InputCommands { get; set; }
        private InputCommand[] StatusCommands { get; set; }

        public void Setup(UnityAction callback)
        {
            Callback = callback;
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, CommandInteract),
                new(InputCommand.ButtonCancel, ButtonType.Down, CommandCancel),
            };

            StatusCommands = new InputCommand[]
            {
                new(InputCommand.ButtonLeft, ButtonType.Press, ActorLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, ActorRight),
                new(InputCommand.ButtonInteract, ButtonType.Down, ActorInteract),
                new(InputCommand.ButtonCancel, ButtonType.Down, ActorCancel),
            };

            (string, string)[] texts = new (string, string)[]
            {
                (ResourceManager.Term.item, "item"),
                (ResourceManager.Term.skill, "skill"),
                (ResourceManager.Term.equip, "equip"),
                (ResourceManager.Term.status, "status"),
                (ResourceManager.Term.settings, "settings")
            };
            listBox.Initialize(1, texts.Length, Refresh, texts);

            RebindAllActor();
            Party.RegisterMemberSortCallback(RebindAllActor);

            goldLabel.text = ResourceManager.Term.currencyUnit + ':';
            RefreshGold(Party.Gold);
            Party.RegisterGoldChangeCallback(RefreshGold);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(MainMenuPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(MainMenuPanel));
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (actorStatusItems.Length != Party.MaxBattleMembers)
            {
                Debug.Log("ActorStatusItem 数量错误，应等于最大参战人员数量。");
            }
#endif
        }

        private void OnDestroy()
        {
            Party.UnregisterMemberSortCallback(RebindAllActor);
            Party.UnregisterGoldChangeCallback(RefreshGold);
            Callback?.Invoke();
        }

        /// <summary>
        /// 重新绑定所有角色
        /// </summary>
        private void RebindAllActor()
        {
            var list = Party.GetBattleActorList();
            for (int i = 0; i < actorStatusItems.Length; i++)
            {
                if (i < list.Count)
                    actorStatusItems[i].Rebind(list[i]);
                else
                    actorStatusItems[i].Rebind(null);
            }
        }

        private void RefreshGold(int gold)
        {
            goldContent.text = gold.ToString();
        }

        private void CommandInteract()
        {
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "item":
                    UIManager.Instantiate(itemTypePrefab);
                    break;
                case "skill":
                case "equip":
                case "status":
                    SelectedIndex = 0;
                    IsSelectActor = true;
                    actorStatusItems[0].Selected = true;
                    InputManagementSystem.AddCommands(
                        nameof(MainMenuPanel) + "Status",
                        StatusCommands
                    );
                    break;
                case "settings":
                    UIManager.Instantiate(settingPanelPrefab);
                    break;
            }
        }

        private void CommandCancel()
        {
            Destroy(gameObject);
        }

        private void ActorInteract()
        {
            var actor = Party.GetBattleActor(SelectedIndex);
            if (actor != null)
            {
                switch ((((string, string))listBox.SelectedItem).Item2)
                {
                    case "skill":
                        UIManager
                            .Instantiate(actorSkillPrefab)
                            .GetComponent<SkillPanel>()
                            .Setup(actor);
                        break;
                    case "equip":
                        UIManager
                            .Instantiate(actorEquipmentPrefab)
                            .GetComponent<ActorEquipmentPanel>()
                            .SetActor(actor);
                        break;
                    case "status":
                        UIManager
                            .Instantiate(actorStatusPrefab)
                            .GetComponent<ActorStatusPanel>()
                            .Setup(Party.GetBattleActorList(), SelectedIndex);
                        break;
                }
            }
        }

        private void ActorCancel()
        {
            if (IsSelectActor)
            {
                actorStatusItems[SelectedIndex].Selected = false;
            }
            InputManagementSystem.RemoveCommands(nameof(MainMenuPanel) + "Status");
        }

        private void ActorLeft()
        {
            if (SelectedIndex > 0)
            {
                actorStatusItems[SelectedIndex].Selected = false;
                SelectedIndex--;
                actorStatusItems[SelectedIndex].Selected = true;
            }
        }

        private void ActorRight()
        {
            if (SelectedIndex < Party.GetBattleActorCount() - 1)
            {
                actorStatusItems[SelectedIndex].Selected = false;
                SelectedIndex++;
                actorStatusItems[SelectedIndex].Selected = true;
            }
        }

        private void Refresh(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, string))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
