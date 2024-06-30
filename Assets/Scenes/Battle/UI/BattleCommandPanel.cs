using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UI;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// 选择战斗指令的面板
    /// </summary>
    public class BattleCommandPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private GameObject weaponPanelPrefab;

        [SerializeField]
        private GameObject skillPanelPrefab;

        private Actor CurrentActor { get; set; }

        private bool EnableSkill { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(BattleCommandPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(BattleCommandPanel));
        }

        public void Setup(Actor actor, bool enableSkill)
        {
            CurrentActor = actor;
            EnableSkill = enableSkill;
            (string, string)[] texts = new (string, string)[]
            {
                (ResourceManager.Term.attack, "attack"),
                (ResourceManager.Term.skill, "skill"),
                (ResourceManager.Term.item, "item"),
                (ResourceManager.Term.status, "status"),
                (ResourceManager.Term.escape, "escape")
            };

            listBox.Initialize(1, texts.Length, RefreshItem, texts);
        }

        private void Interact()
        {
            BattleManager.CurrentCommand = new();
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "attack":
                    canvasGroup.alpha = 0;
                    var we = UIManager
                        .Instantiate(weaponPanelPrefab)
                        .GetComponent<WeaponSelectionPanel>();
                    we.Setup(CurrentActor, () => canvasGroup.alpha = 1, OnInputFinish);
                    break;
                case "skill":
                    if (EnableSkill)
                    {
                        var skills = CurrentActor.BattleSkills;
                        if (skills.Count > 0)
                        {
                            canvasGroup.alpha = 0;
                            var se = UIManager
                                .Instantiate(skillPanelPrefab)
                                .GetComponent<SkillSelectionPanel>();
                            se.Setup(
                                CurrentActor,
                                skills,
                                () => canvasGroup.alpha = 1,
                                OnInputFinish
                            );
                        }
                    }
                    else
                    {
                        UIManager.StartMessage(ResourceManager.Term.promptPanicState, null);
                    }
                    break;
                case "escape":
                    if (Random.value > 0.5f)
                    {
                        UIManager.StartMessage(
                            ResourceManager.Term.escapeVictory,
                            () =>
                            {
                                OnInputFinish();
                                BattleManager.EscapeBattle();
                            }
                        );
                    }
                    else
                    {
                        UIManager.StartMessage(
                            ResourceManager.Term.escapeDefeat,
                            () =>
                            {
                                OnInputFinish();
                                BattleManager.CommandInputEnd();
                            }
                        );
                    }
                    break;
            }
        }

        private void OnInputFinish()
        {
            (CurrentActor.DisplayObject as DisplayActor).MoveToRight();
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                {
                    (string, string) pair = ((string, string))data;
                    if (pair.Item2 == "skill" && !EnableSkill)
                    {
                        c.textComponent.color = Color.gray;
                        c.textComponent.text = pair.Item1;
                    }
                    else
                    {
                        c.textComponent.color = Color.white;
                        c.textComponent.text = pair.Item1;
                    }
                }
                else
                {
                    c.textComponent.color = Color.white;
                    c.textComponent.text = " ";
                }
            }
        }
    }
}
