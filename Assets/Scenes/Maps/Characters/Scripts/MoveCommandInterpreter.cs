using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    public class MoveCommandInterpreter
    {
        public MoveCommandInterpreter(
            Mover mover,
            IList<CommandSet.Command> commands,
            bool repeat,
            bool skippable,
            UnityAction callback
        )
        {
            OwnerMover = mover;
            Commands = commands;
            Repeat = repeat;
            Skippable = skippable;
            Callback = callback;
            Reset();
        }

        private Mover OwnerMover { get; set; }
        public IList<CommandSet.Command> Commands { get; set; }
        public bool Repeat { get; set; }
        public bool Skippable { get; set; }
        public UnityAction Callback { get; set; }
        public int CurrentIndex { get; set; }
        public CommandSet.Command CurrentCommand => Commands[CurrentIndex];
        public string[] CurrentTexts { get; set; }
        public int StepCount { get; set; }
        public bool IsStepCountCalculate { get; set; }

        public void Reset()
        {
            CurrentIndex = 0;
            StepCount = 0;
            IsStepCountCalculate = false;
            CurrentTexts = CurrentCommand.parameter.Split(',');
        }

        public void NextCommand()
        {
            CurrentIndex++;
            if (CurrentIndex < Commands.Count)
            {
                CurrentTexts = CurrentCommand.parameter.Split(',');
            }
            else
            {
                CurrentTexts = null;
            }
        }

        public void ProcessMoveCommand()
        {
            int loopCount = 0;
            while (CurrentIndex < Commands.Count)
            {
                if (loopCount++ > 100)
                {
                    Debug.LogError("循环次数超出限制");
                    return;
                }

                switch (CurrentCommand.code)
                {
                    case CommandSet.CommandType.Wait:
                        Wait();
                        return;
                    case CommandSet.CommandType.WaitAtRandom:
                        WaitAtRandom();
                        return;
                    case CommandSet.CommandType.Comment:
                        // do nothing
                        NextCommand();
                        break;
                    case CommandSet.CommandType.Move:
                        Move();
                        return;
                    case CommandSet.CommandType.MoveAtRandom:
                        MoveAtRandom();
                        return;
                    case CommandSet.CommandType.MoveTowardPlayer:
                        MoveTowardPlayer();
                        return;
                    case CommandSet.CommandType.MoveAwayFromPlayer:
                        MoveAwayFromPlayer();
                        return;
                    case CommandSet.CommandType.StepForward:
                        StepForward();
                        return;
                    case CommandSet.CommandType.StepBackward:
                        StepBackward();
                        return;
                    case CommandSet.CommandType.Turn:
                        Turn();
                        break;
                    case CommandSet.CommandType.TurnAtRelative:
                        TurnAtRelative();
                        break;
                    case CommandSet.CommandType.TurnAtRandom:
                        TurnAtRandom();
                        break;
                    case CommandSet.CommandType.TurnTowardPlayer:
                        TurnTowardPlayer();
                        break;
                    case CommandSet.CommandType.TurnAwayFromPlayer:
                        TurnAwayFromPlayer();
                        break;
                    case CommandSet.CommandType.ChangeMoveSpeed:
                        ChangeMoveSpeed();
                        break;
                    case CommandSet.CommandType.DisableMoveScope:
                        DisableMoveScope();
                        break;
                    case CommandSet.CommandType.SetMoveScope:
                        SetMoveScope();
                        break;
                    default:
                        Debug.LogWarning($"{CurrentCommand.code}不能作为移动指令使用");
                        break;
                }
            }

            if (OwnerMover.WaitCount <= 0)
            {
                EndRouteMove();
            }
        }

        private void Wait()
        {
            OwnerMover.WaitCount = float.Parse(CurrentTexts[0]);
            NextCommand();
        }

        private void WaitAtRandom()
        {
            var texts = CurrentTexts;
            OwnerMover.WaitCount = UnityEngine
                .Random
                .Range(float.Parse(texts[0]), float.Parse(texts[1]) + 1);
            NextCommand();
        }

        private void Move()
        {
            var texts = CurrentTexts;
            if (OwnerMover.MoveInNoTime((Mover.DirectionType)int.Parse(texts[0])))
            {
                if (!IsStepCountCalculate)
                {
                    if (texts.Length >= 2)
                    {
                        StepCount = int.Parse(texts[1]);
                        IsStepCountCalculate = true;
                    }
                    else
                    {
                        NextCommand();
                        return;
                    }
                }

                StepCount--;
                if (StepCount <= 0)
                {
                    IsStepCountCalculate = false;
                    StepCount = 0;
                    NextCommand();
                }
            }
            else
            {
                RouteMoveFailed();
            }
        }

        private void MoveAtRandom()
        {
            var texts = CurrentTexts;
            if (OwnerMover.MoveInNoTime(Mover.RandomDirection()))
            {
                if (!IsStepCountCalculate)
                {
                    if (texts.Length >= 2)
                    {
                        StepCount = UnityEngine
                            .Random
                            .Range(int.Parse(texts[0]), int.Parse(texts[1]) + 1);
                        IsStepCountCalculate = true;
                    }
                    else if (texts[0] != string.Empty)
                    {
                        StepCount = int.Parse(texts[0]);
                        IsStepCountCalculate = true;
                    }
                    else
                    {
                        NextCommand();
                        return;
                    }
                }

                StepCount--;
                if (StepCount <= 0)
                {
                    IsStepCountCalculate = false;
                    StepCount = 0;
                    NextCommand();
                }
            }
            else
            {
                RouteMoveFailed();
            }
        }

        private void MoveAtRelative(Vector3 relativePosition)
        {
            if (OwnerMover.CanMove)
            {
                if (Random.value < 0.5f)
                {
                    if (relativePosition.x > 0)
                    {
                        OwnerMover.MoveInNoTime(Mover.DirectionType.Right);
                    }
                    else if (relativePosition.x < 0)
                    {
                        OwnerMover.MoveInNoTime(Mover.DirectionType.Left);
                    }
                }

                if (!OwnerMover.IsMoving)
                {
                    if (relativePosition.y > 0)
                    {
                        OwnerMover.MoveInNoTime(Mover.DirectionType.Up);
                    }
                    else if (relativePosition.y < 0)
                    {
                        OwnerMover.MoveInNoTime(Mover.DirectionType.Down);
                    }
                }

                if (OwnerMover.IsMoving)
                {
                    NextCommand();
                }
            }
        }

        private void MoveTowardPlayer()
        {
            Vector3 relativePosition = PlayerParty.PlayerPosition - OwnerMover.Position;
            MoveAtRelative(relativePosition);
        }

        private void MoveAwayFromPlayer()
        {
            Vector3 relativePosition = OwnerMover.Position - PlayerParty.PlayerPosition;
            MoveAtRelative(relativePosition);
        }

        private void StepForward()
        {
            var texts = CurrentTexts;
            if (OwnerMover.MoveInNoTime(OwnerMover.Direction))
            {
                if (!IsStepCountCalculate)
                {
                    if (texts[0] != string.Empty)
                    {
                        StepCount = int.Parse(texts[0]);
                        IsStepCountCalculate = true;
                    }
                    else
                    {
                        NextCommand();
                        return;
                    }
                }

                StepCount--;
                if (StepCount <= 0)
                {
                    IsStepCountCalculate = false;
                    StepCount = 0;
                    NextCommand();
                }
            }
            else
            {
                RouteMoveFailed();
            }
        }

        private void StepBackward()
        {
            Debug.Log("未实现StepBackward()");
            NextCommand();
        }

        private void Turn()
        {
            OwnerMover.Direction = (Mover.DirectionType)int.Parse(CurrentTexts[0]);
            NextCommand();
        }

        private void TurnAtRelative()
        {
            int newDirection = ((int)OwnerMover.Direction + int.Parse(CurrentTexts[0])) % 8;
            if (newDirection < 0)
            {
                newDirection += 8;
            }

            OwnerMover.Direction = (Mover.DirectionType)newDirection;
            NextCommand();
        }

        private void TurnAtRandom()
        {
            OwnerMover.Direction = Mover.RandomDirection();
            NextCommand();
        }

        private void TurnTowardPlayer()
        {
            OwnerMover.LookAtPlayer();
            NextCommand();
        }

        private void TurnAwayFromPlayer()
        {
            OwnerMover.BackToPlayer();
            NextCommand();
        }

        private void ChangeMoveSpeed()
        {
            OwnerMover.Speed = float.Parse(CurrentTexts[0]);
            NextCommand();
        }

        private void DisableMoveScope()
        {
            OwnerMover.EnableMoveScope = false;
        }

        private void SetMoveScope()
        {
            var texts = CurrentTexts;
            OwnerMover.SetMoveScope(
                new(
                    int.Parse(texts[0]),
                    int.Parse(texts[1]),
                    int.Parse(texts[2]),
                    int.Parse(texts[3])
                )
            );
            NextCommand();
        }

        private void RouteMoveFailed()
        {
            if (Skippable)
            {
                NextCommand();
            }
        }

        public void EndRouteMove()
        {
            Callback?.Invoke();
            if (Repeat)
            {
                Reset();
            }
            else if (OwnerMover.OriginalMoveRoute.repeat)
            {
                OwnerMover.ResetMoveRoute();
            }
            else
            {
                OwnerMover.CurrentMoveRoute = null;
            }
        }
    }
}
