using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    /// <summary>
    /// 在战斗中选择角色的面板。
    /// </summary>
    public class ActorSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject cursorPrefab;

        private Actor CurrentActor { get; set; }

        private Static.UsedScope Scope { get; set; }

        private Actor[] Targets { get; set; }

        private int CurrentIndex { get; set; }

        private Battler CurrentTarget => Targets[CurrentIndex];

        private UnityAction CancelCallback { get; set; }

        private UnityAction FinishCallback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, () => Select(-1)),
                new(InputCommand.ButtonDown, ButtonType.Press, () => Select(1)),
                new(InputCommand.ButtonLeft, ButtonType.Press, () => Select(-1)),
                new(InputCommand.ButtonRight, ButtonType.Press, () => Select(1)),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorSelectionPanel));
        }

        public void Setup(
            Actor owner,
            Actor[] selectableTargets,
            Static.UsedScope scope,
            UnityAction cancelCallback,
            UnityAction finishCallback
        )
        {
            CurrentActor = owner;
            Targets = selectableTargets;
            Scope = scope;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            CurrentTarget.DisplayObject.ShowCursor(cursorPrefab);
        }

        private void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        private void Interact()
        {
            CurrentTarget.DisplayObject.HideCursor();
            BattleManager.CurrentCommand.SelectedTarget = CurrentTarget;
            BattleManager.CommandInputEnd();
            InvokeFinishCallback();
        }

        private void Cancel()
        {
            CurrentTarget.DisplayObject.HideCursor();
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }

        private void Select(int delta)
        {
            CurrentTarget.DisplayObject.HideCursor();
            CurrentIndex += delta;

            if (CurrentIndex < 0)
                CurrentIndex = Targets.Length - 1;
            else if (CurrentIndex >= Targets.Length)
                CurrentIndex = 0;

            CurrentTarget.DisplayObject.ShowCursor(cursorPrefab);
        }
    }
}
