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
    public class MainCommandPanel : MonoBehaviour
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

            (string, string)[] texts = new (string, string)[5]
            {
                (ResourceManager.Term.attack, "attack"),
                (ResourceManager.Term.skill, "skill"),
                (ResourceManager.Term.item, "item"),
                (ResourceManager.Term.status, "status"),
                (ResourceManager.Term.escape, "escape")
            };

            listBox.Initialize(1, texts.Length, RefreshItem, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(MainCommandPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(MainCommandPanel));
        }

        public void Setup(Actor human, bool enableSkill)
        {
            CurrentActor = human;
            EnableSkill = enableSkill;
        }

        private void Interact()
        {
            BattleManager.CurrentCommand = new();
            switch ((((string, string))listBox.SelectedItem).Item2)
            {
                case "attack":
                    canvasGroup.alpha = 0;
                    var we = BattleManager
                        .CreateUI(weaponPanelPrefab)
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
                            var se = BattleManager
                                .CreateUI(skillPanelPrefab)
                                .GetComponent<SkillSelectionPanel>();
                            se.Setup(
                                CurrentActor,
                                skills,
                                () => canvasGroup.alpha = 1,
                                OnInputFinish
                            );
                        }
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
                    c.textComponent.text = (((string, string))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
