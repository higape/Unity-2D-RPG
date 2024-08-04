using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    /// <summary>
    /// 选择技能的面板
    /// </summary>
    public class SkillSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private SkillStatistic skillStat;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private GameObject actorPanelPrefab;

        [SerializeField]
        private GameObject enemyPanelPrefab;

        private Actor CurrentActor { get; set; }
        private List<Skill> Skills { get; set; }
        private UnityAction CancelCallback { get; set; }
        private UnityAction FinishCallback { get; set; }
        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            listBox.RegisterSelectedItemChangeCallback(
                (data, index) => skillStat.Refresh(data as Skill)
            );
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(SkillSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(SkillSelectionPanel));
        }

        public void Setup(
            Actor actor,
            List<Skill> skills,
            UnityAction cancelCallback,
            UnityAction finishCallback
        )
        {
            CurrentActor = actor;
            Skills = skills;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            listBox.Initialize(1, Mathf.Min(8, Skills.Count), RefreshItem, Skills);
        }

        private void Interact()
        {
            if (listBox.SelectedItem is not Skill skill)
                return;

            if (skill.IsCooling)
            {
                UIManager.StartMessage(ResourceManager.Term.promptSkillIsCooling, null);
                return;
            }

            if (!skill.CanUse)
            {
                return;
            }

            var usage = skill.Usage;
            if (usage == null)
            {
                Debug.LogWarning("暂时不能使用选择类技能");
                return;
            }

            BattleManager.CurrentCommand.SelectedItems = new() { new(skill, usage) };

            switch (usage.scope)
            {
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriend:
                case Static.UsedScope.AllFriendExcludeSelf:
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    var actorTargets = BattleManager.GetActorToActorTargets(
                        CurrentActor,
                        usage.scope
                    );
                    if (actorTargets.Length > 0)
                    {
                        UIManager
                            .Instantiate(actorPanelPrefab)
                            .GetComponent<ActorSelectionPanel>()
                            .Setup(
                                CurrentActor,
                                actorTargets,
                                usage.scope,
                                () => canvasGroup.alpha = 1,
                                InvokeFinishCallback
                            );
                    }
                    else
                    {
                        //提示没有可选目标
                        UIManager.StartMessage(ResourceManager.Term.promptNoSelectableTarget, null);
                    }
                    break;
                case Static.UsedScope.OneEnemy:
                case Static.UsedScope.AllEnemy:
                case Static.UsedScope.SmallSector:
                case Static.UsedScope.BigSector:
                case Static.UsedScope.SmallRay:
                case Static.UsedScope.BigRay:
                case Static.UsedScope.SmallCircle:
                case Static.UsedScope.BigCircle:
                    UIManager
                        .Instantiate(enemyPanelPrefab)
                        .GetComponent<EnemySelectionPanel>()
                        .Setup(
                            CurrentActor,
                            usage.scope,
                            () => canvasGroup.alpha = 1,
                            InvokeFinishCallback
                        );
                    break;
                default:
                    BattleManager.CurrentCommand.SelectedTarget = BattleManager.BestTarget(
                        CurrentActor,
                        usage.scope
                    );
                    InvokeFinishCallback();
                    break;
            }

            canvasGroup.alpha = 0;
        }

        private void Cancel()
        {
            BattleManager.CurrentCommand.SelectedItems = null;
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        private void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                var skill = data as Skill;
                c.textComponent0.text = skill.Name;
                c.textComponent1.text =
                    skill.CurrentCount.ToString().PadLeft(2, ' ')
                    + '/'
                    + skill.MaxUsageCount.ToString().PadLeft(2, ' ');
            }
        }
    }
}
