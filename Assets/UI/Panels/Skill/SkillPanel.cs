using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI header;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private SkillStatistic itemStatistic;

        [SerializeField]
        private GameObject actorListPrefab;

        private Skill CurrentSkill { get; set; }

        private SimpleActorStatusList ActorListInstance { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(Actor actor)
        {
            itemListBox.SetSource(actor.AllSkills);
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            header.text = ResourceManager.Term.skill;
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
            itemListBox.Initialize(1, 8, RefreshItem);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SkillPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SkillPanel));
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is Skill item)
                {
                    c.textComponent0.text = item.Name;
                    if (item.IsEnable)
                    {
                        c.textComponent0.color = Color.white;
                        c.textComponent1.text = UIManager.CreateSkillCountText(item);
                    }
                    else
                    {
                        c.textComponent0.color = Color.gray;
                        c.textComponent1.text = " ";
                    }
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                }
            }
        }

        private void OnSelectedItemChange(object data, int index)
        {
            itemStatistic.Refresh(data as Skill);
        }

        private void Interact()
        {
            if (itemListBox.SelectedItem is Skill skill)
            {
                CurrentSkill = skill;
                if (skill.UsedInMenu && skill.CanUse)
                {
                    //打开角色面板并传递回调
                    ActorListInstance = UIManager
                        .Instantiate(actorListPrefab)
                        .GetComponent<SimpleActorStatusList>();
                    ActorListInstance.Setup(Party.PartyActorList, OnActorInteract);
                }
                else
                {
                    //提示无法使用
                    UIManager.StartMessage(ResourceManager.Term.promptCannotUseSkillInMenu, null);
                }
            }
        }

        private void OnActorInteract(Battler actor)
        {
            var item = CurrentSkill;
            var usage = item.Usage;
            if (item.CanUse && item.UsedInMenu && usage != null)
            {
                var result = Mathc.ProcessItemEffect(
                    usage.effects,
                    actor,
                    actor,
                    usage.element,
                    actor.Atk + item.Attack,
                    actor.Hit,
                    1f
                );
                //在生效的情况下消耗使用次数
                if (result.Item1 > 0 || result.Item2 > 0)
                {
                    item.Cost();
                    itemListBox.Refresh();
                }
                ActorListInstance.Refresh();
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }
    }
}
