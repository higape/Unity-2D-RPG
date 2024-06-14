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
    /// 选择武器的面板
    /// </summary>
    public class WeaponSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private GameObject enemyPanelPrefab;

        private Actor CurrentActor { get; set; }
        private UnityAction CancelCallback { get; set; }
        private UnityAction FinishCallback { get; set; }
        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonPrevious, ButtonType.Press, listBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Press, listBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(WeaponSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(WeaponSelectionPanel));
        }

        public void Setup(Actor human, UnityAction cancelCallback, UnityAction finishCallback)
        {
            CurrentActor = human;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            var wl = human.GetBattleWeapons();
            listBox.Initialize(1, Mathf.Min(8, wl.Count), RefreshItem, wl);
        }

        private void Interact()
        {
            var weapon = listBox.SelectedItem as ActorWeapon;
            var usage = weapon.GetUsage(0);
            BattleManager.CurrentCommand.SelectedItems = new() { new(weapon, usage) };
            var selectedTarget = BattleManager.BestTarget(CurrentActor, usage.scope);

            switch (usage.scope)
            {
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriend:
                case Static.UsedScope.AllFriendExcludeSelf:
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    Debug.LogWarning("未能选择友军");
                    break;
                case Static.UsedScope.OneEnemy:
                case Static.UsedScope.AllEnemy:
                case Static.UsedScope.SmallSector:
                case Static.UsedScope.BigSector:
                case Static.UsedScope.SmallRay:
                case Static.UsedScope.BigRay:
                case Static.UsedScope.SmallCircle:
                case Static.UsedScope.BigCircle:
                    BattleManager
                        .CreateUI(enemyPanelPrefab)
                        .GetComponent<EnemySelectionPanel>()
                        .Setup(
                            CurrentActor,
                            selectedTarget,
                            usage.scope,
                            () => canvasGroup.alpha = 1,
                            InvokeFinishCallback
                        );
                    break;
                default:
                    BattleManager.CurrentCommand.SelectedTarget = selectedTarget;
                    InvokeFinishCallback();
                    break;
            }

            canvasGroup.alpha = 0;
        }

        public void Cancel()
        {
            BattleManager.CurrentCommand.SelectedItems = null;
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        public void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                c.textComponent.text = (data as ActorWeapon).Name;
            }
        }
    }
}
