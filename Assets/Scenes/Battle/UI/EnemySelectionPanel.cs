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
    /// 选择敌人的面板。
    /// 绘制攻击范围，并在目标的位置显示光标。
    /// </summary>
    public class EnemySelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject cursorPrefab;

        private Actor CurrentActor { get; set; }
        private Static.UsedScope Scope { get; set; }
        private Battler CurrentTarget { get; set; }
        private UnityAction CancelCallback { get; set; }
        private UnityAction FinishCallback { get; set; }
        private Enemy[] Enemies { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Press, SelectLeft),
                new(InputCommand.ButtonRight, ButtonType.Press, SelectRight),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(EnemySelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(EnemySelectionPanel));
        }

        public void Setup(
            Actor actor,
            Static.UsedScope scope,
            UnityAction cancelCallback,
            UnityAction finishCallback
        )
        {
            CurrentActor = actor;
            Scope = scope;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
            CurrentTarget = BattleManager.BestTarget(actor, scope);
            Enemies = BattleManager.GetActorToEnemyTargets();
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

        private void SelectLeft()
        {
            //设两条k值分别为1和-1的直线，与当前目标位置相交，
            //划分出四个区域，选择左侧区域的目标。

            //当前目标位置
            Vector3 p = CurrentTarget.DisplayObject.Position;

            //直线的b值
            //k=-1
            float b0 = p.y + p.x;
            //k=1
            float b1 = p.y - p.x;

            List<Battler> list = new();

            foreach (Battler battler in Enemies)
            {
                Vector3 bp = battler.DisplayObject.Position;
                if (bp.x < p.x && bp.x < b0 - bp.y && bp.x <= bp.y - b1)
                    list.Add(battler);
            }

            SelectNearest(p, list);
        }

        private void SelectRight()
        {
            Vector3 p = CurrentTarget.DisplayObject.Position;
            float b0 = p.y + p.x;
            float b1 = p.y - p.x;
            List<Battler> list = new();

            foreach (Battler battler in Enemies)
            {
                Vector3 bp = battler.DisplayObject.Position;
                if (bp.x > p.x && bp.x > b0 - bp.y && bp.x >= bp.y - b1)
                    list.Add(battler);
            }

            SelectNearest(p, list);
        }

        private void SelectUp()
        {
            Vector3 p = CurrentTarget.DisplayObject.Position;
            float b0 = p.y + p.x;
            float b1 = p.y - p.x;
            List<Battler> list = new();

            foreach (Battler battler in Enemies)
            {
                Vector3 bp = battler.DisplayObject.Position;
                if (bp.y >= p.y && bp.y > b0 - bp.x && bp.y >= b1 + bp.x)
                    list.Add(battler);
            }

            SelectNearest(p, list);
        }

        private void SelectDown()
        {
            Vector3 p = CurrentTarget.DisplayObject.Position;
            float b0 = p.y + p.x;
            float b1 = p.y - p.x;
            List<Battler> list = new();

            foreach (Battler battler in Enemies)
            {
                Vector3 bp = battler.DisplayObject.Position;
                if (bp.y <= p.y && bp.y < b0 - bp.x && bp.y <= b1 + bp.x)
                    list.Add(battler);
            }

            SelectNearest(p, list);
        }

        /// <summary>
        /// 选择最接近的
        /// </summary>
        private void SelectNearest(Vector3 position, List<Battler> list)
        {
            if (list.Count != 0)
            {
                float minValue = (list[0].DisplayObject.Position - position).magnitude;
                Battler nearestTarget = list[0];

                for (int i = 1; i < list.Count; i++)
                {
                    float m = (list[i].DisplayObject.Position - position).magnitude;

                    if (m < minValue)
                    {
                        minValue = m;
                        nearestTarget = list[i];
                    }
                }

                CurrentTarget.DisplayObject.HideCursor();
                CurrentTarget = nearestTarget;
                CurrentTarget.DisplayObject.ShowCursor(cursorPrefab);
            }
        }
    }
}
