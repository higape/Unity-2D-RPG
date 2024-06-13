using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dynamic;
using Root;
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
        private ActorStatusItem[] humanStatusItems;

        [SerializeField]
        private GameObject itemTypePrefab;

        [SerializeField]
        private GameObject humanEquipmentPrefab;

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
                (ResourceManager.Term.options, "options")
            };

            listBox.Initialize(1, texts.Length, Refresh, texts);
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
            if (humanStatusItems.Length != Party.MaxBattleMembers)
            {
                Debug.Log("ActorStatusItem 数量错误，应等于最大参战人员数量。");
            }
#endif

            RebindAllActor();
            Party.RegisterMemberSortCallback(RebindAllActor);
        }

        private void OnDestroy()
        {
            Party.UnregisterMemberSortCallback(RebindAllActor);
            Callback?.Invoke();
        }

        /// <summary>
        /// 重新绑定所有角色
        /// </summary>
        private void RebindAllActor()
        {
            var list = Party.GetBattleActorList();
            for (int i = 0; i < humanStatusItems.Length; i++)
            {
                if (i < list.Count)
                    humanStatusItems[i].Rebind(list[i]);
                else
                    humanStatusItems[i].Rebind(null);
            }
        }

        private void CommandInteract()
        {
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "item":
                    UIManager.Instantiate(itemTypePrefab);
                    break;
                case "equip":
                    SelectedIndex = 0;
                    IsSelectActor = true;
                    humanStatusItems[0].Selected = true;
                    InputManagementSystem.AddCommands(
                        nameof(MainMenuPanel) + "Status",
                        StatusCommands
                    );
                    break;
            }
        }

        private void CommandCancel()
        {
            Destroy(gameObject);
        }

        private void ActorInteract()
        {
            var human = Party.GetBattleActor(SelectedIndex);
            if (human != null)
                UIManager
                    .Instantiate(humanEquipmentPrefab)
                    .GetComponent<ActorEquipmentPanel>()
                    .SetActor(human);
        }

        private void ActorCancel()
        {
            if (IsSelectActor)
            {
                humanStatusItems[SelectedIndex].Selected = false;
            }
            InputManagementSystem.RemoveCommands(nameof(MainMenuPanel) + "Status");
        }

        private void ActorLeft()
        {
            if (SelectedIndex > 0)
            {
                humanStatusItems[SelectedIndex].Selected = false;
                SelectedIndex--;
                humanStatusItems[SelectedIndex].Selected = true;
            }
        }

        private void ActorRight()
        {
            if (SelectedIndex < Party.GetBattleActorCount() - 1)
            {
                humanStatusItems[SelectedIndex].Selected = false;
                SelectedIndex++;
                humanStatusItems[SelectedIndex].Selected = true;
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
