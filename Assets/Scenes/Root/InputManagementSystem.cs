using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Root
{
    //管理输入的类。
    public class InputManagementSystem : MonoBehaviour
    {
        private struct Pair
        {
            public object key;
            public IList<InputCommand> commands;
            public UnityAction action;
        }

        private static float TimeCount { get; set; }

        private static bool IsTimeOk => TimeCount <= 0;

        private static bool BaseCondition => Pairs.Count != 0;

        private static List<Pair> Pairs { get; set; } = new List<Pair>();

        private static IList<InputCommand> CurrentCommands { get; set; }

        private static UnityAction CurrentAction { get; set; }

        private static void UpdateAction()
        {
            if (Pairs.Count > 0)
            {
                CurrentCommands = Pairs[^1].commands;
                CurrentAction = Pairs[^1].action;
            }
            else
            {
                CurrentCommands = null;
                CurrentAction = null;
            }
            Input.ResetInputAxes();
        }

        public static void AddCommands(object key, IList<InputCommand> commands)
        {
            Pairs.Add(
                new Pair
                {
                    key = key,
                    commands = commands,
                    action = UpdateByCommand
                }
            );
            UpdateAction();
        }

        public static void RemoveCommands(object key)
        {
            for (int i = 0; i < Pairs.Count; i++)
            {
                if (Pairs[i].key == key)
                {
                    Pairs.RemoveAt(i);
                    break;
                }
            }
            UpdateAction();
        }

        public static void AddAction(object key, UnityAction action)
        {
            Pairs.Add(new Pair { key = key, action = action });
            UpdateAction();
        }

        public static void RemoveAction(object key)
        {
            for (int i = 0; i < Pairs.Count; i++)
            {
                if (Pairs[i].key == key)
                {
                    Pairs.RemoveAt(i);
                    break;
                }
            }
            UpdateAction();
        }

        private static void UpdateByCommand()
        {
            if (TimeCount > 0)
            {
                TimeCount -= Time.deltaTime;
            }

            foreach (var cmd in CurrentCommands)
            {
                switch (cmd.Type)
                {
                    case ButtonType.Press:
                        if (Input.GetButton(cmd.Name) && IsTimeOk)
                        {
                            TimeCount = cmd.Delay;
                            cmd.Callback.Invoke();
                        }
                        break;
                    case ButtonType.Down:
                        if (Input.GetButtonDown(cmd.Name))
                        {
                            TimeCount = cmd.Delay;
                            cmd.Callback.Invoke();
                        }
                        break;
                    case ButtonType.Up:
                        if (Input.GetButtonUp(cmd.Name))
                            cmd.Callback.Invoke();
                        break;
                }
            }
        }

        private void Update()
        {
            if (BaseCondition)
            {
                CurrentAction?.Invoke();
            }
        }
    }
}
