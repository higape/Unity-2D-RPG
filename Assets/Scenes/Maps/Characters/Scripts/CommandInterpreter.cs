using System;
using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UI;
using UnityEngine;
using CT = Map.CommandSet.CommandType;

namespace Map
{
    /// <summary>
    /// 命令解析器
    /// </summary>
    public class CommandInterpreter : MonoBehaviour
    {
        public static bool Pause { get; set; }

        private static bool IsOnlyDigits(string str)
        {
            if (str == string.Empty)
            {
                return false;
            }

            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        [SerializeField]
        private CommandSet commandSet;

        private bool PlayerWaiting => commandSet.playerWaiting;
        private int CurrentIndex { get; set; }
        private string[] CurrentTexts { get; set; }
        private Dictionary<string, object> TempVariable { get; set; } = new();
        private List<CT> Branch { get; set; } = new();
        private float TimeCount { get; set; }
        private bool IsWaitingCallback { get; set; }

        public void StartCommand()
        {
            CurrentIndex = 0;
            enabled = true;
        }

        public void EndCommand()
        {
            if (IsWaitingCallback)
            {
                Debug.LogWarning($"{gameObject.name}在等待回调时结束命令");
            }
            else
            {
                Debug.Log($"{gameObject.name}结束命令");
            }
            CurrentIndex = 0;
            enabled = false;
        }

        private void OnEnable()
        {
            if (PlayerWaiting)
            {
                PlayerController.WaitCount++;
            }
        }

        private void OnDisable()
        {
            if (PlayerWaiting)
            {
                PlayerController.WaitCount--;
            }
        }

        private void Update()
        {
            if (Pause || IsWaitingCallback)
                return;

            if (TimeCount > 0)
            {
                TimeCount -= Time.deltaTime;
                if (TimeCount > 0)
                {
                    return;
                }
                else if (CurrentIndex >= commandSet.commands.Length)
                {
                    EndCommand();
                    return;
                }
            }

            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            int loopCount = 0;
            while (CurrentIndex < commandSet.commands.Length)
            {
                if (++loopCount > 500)
                {
                    Debug.LogError("Command calls exceeded the limit");
                    break;
                }

                if (!enabled)
                    return;

                Debug.Log($"{gameObject.name} 执行命令Index: {CurrentIndex}");
                var command = commandSet.commands[CurrentIndex];
                CurrentTexts = command.parameter.Split(',');

                switch (command.code)
                {
                    case CT.ShowText:
                        ShowText();
                        return;
                    case CT.ShowChoices:
                        ShowChoices();
                        return;
                    case CT.InputGold:
                        InputGold();
                        return;
                    case CT.Battle:
                        StartBattle();
                        return;
                    case CT.Shop:
                        Shop();
                        return;
                    case CT.OpenSystemMenu:
                        OpenSystemMenu();
                        return;
                    case CT.TransferPlayer:
                        TransferPlayer();
                        return;
                    case CT.SetPlayerLocation:
                        SetPlayerLocation();
                        break;
                    case CT.SetCharacterLocation:
                        SetCharacterLocation();
                        break;
                    case CT.SetVehicleLocation:
                        SetVehicleLocation();
                        break;
                    case CT.GatherFolowers:
                        GatherFolowers();
                        return;
                    case CT.ChangePlayerTransparency:
                        ChangePlayerTransparency();
                        break;
                    case CT.ChangeFollowersTransparency:
                        ChangeFollowersTransparency();
                        break;
                    case CT.EnableObject:
                        EnableObject();
                        break;
                    case CT.DisableObject:
                        DisableObject();
                        break;
                    case CT.MovementRoute:
                        if (MovementRoute())
                            return;
                        else
                            break;
                    case CT.EndMovementRoute:
                        EndMovementRoute();
                        break;
                    case CT.GetOnOffVehicle:
                        GetOnOffVehicle();
                        break;
                    case CT.ScrollMap:
                        ScrollMap();
                        break;
                    case CT.If:
                        If();
                        break;
                    case CT.Else:
                        Else();
                        break;
                    case CT.EndIf:
                        EndIf();
                        break;
                    case CT.Loop:
                        Loop();
                        break;
                    case CT.BreakLoop:
                        BreakLoop();
                        break;
                    case CT.ContinueLoop:
                        ContinueLoop();
                        break;
                    case CT.EndLoop:
                        EndLoop();
                        break;
                    case CT.ExitProcessing:
                        ExitProcessing();
                        return;
                    case CT.ControlBool:
                        ControlBool();
                        break;
                    case CT.ControlInt:
                        ControlInt();
                        break;
                    case CT.ChangeGold:
                        ChangeGold();
                        break;
                    case CT.ChangeItem:
                        ChangeItem();
                        break;
                    case CT.ChangePartyMember:
                        ChangePartyMember();
                        break;
                    case CT.RecoverAll:
                        RecoverAll();
                        break;
                    case CT.Wait:
                        Wait();
                        return;
                    case CT.WaitAtRandom:
                        WaitAtRandom();
                        return;
                    case CT.FadeoutScreen:
                        FadeOutScreen();
                        break;
                    case CT.FadeinScreen:
                        FadeInScreen();
                        break;
                    case CT.PlayBGM:
                        PlayBGM();
                        break;
                    case CT.PlaySE:
                        PlaySE();
                        break;
                    case CT.Comment:
                        // do nothing
                        break;
                    default:
                        Debug.LogWarning($"{command.code}不能作为指令使用");
                        break;
                }
            }

            if (TimeCount <= 0)
            {
                EndCommand();
            }
        }

        private void ExecuteCommandAfterCallback()
        {
            Pause = false;
            IsWaitingCallback = false;
            CurrentIndex++;
            ExecuteCommand();
        }

        private void ShowText()
        {
            Pause = true;
            IsWaitingCallback = true;
            List<string> list = new();
            for (int i = 0; i < CurrentTexts.Length; i++)
            {
                list.Add(commandSet.texts[int.Parse(CurrentTexts[i])]);
            }
            UIManager.StartMessage(list, MessageCallback);
        }

        private void MessageCallback()
        {
            ExecuteCommandAfterCallback();
        }

        private void ShowChoices()
        {
            Pause = true;
            IsWaitingCallback = true;
            List<string> list2 = new();
            for (int i = 2; i < CurrentTexts.Length; i++)
            {
                list2.Add(commandSet.texts[int.Parse(CurrentTexts[i])]);
            }
            UIManager.StartChoice(
                commandSet.texts[int.Parse(CurrentTexts[1])],
                list2,
                ChoicesCallback
            );
        }

        private void ChoicesCallback(int choiceIndex)
        {
            SetIntVariable(CurrentTexts[0], choiceIndex);
            ExecuteCommandAfterCallback();
        }

        private void InputGold()
        {
            Pause = true;
            IsWaitingCallback = true;
            //打开输入金额的界面
            Debug.LogError("输入金额的功能未实现");
        }

        private void InputGoldCallback(int gold)
        {
            SetIntVariable(CurrentTexts[0], gold);
            ExecuteCommandAfterCallback();
        }

        private void StartBattle()
        {
            Pause = true;
            var data = ResourceManager.LoadFixedEncounter(CurrentTexts[0]);
            Battle.BattleManager.StartBattle(data.MakeBattleData(), BattleCallback);
            CurrentIndex++;
        }

        private void BattleCallback(Battle.BattleManager.EndMode endMode)
        {
            if (CurrentTexts.Length > 1)
            {
                SetIntVariable(CurrentTexts[1], (int)endMode);
            }
            Pause = false;
        }

        private void Shop()
        {
            Pause = true;
            UIManager.OpenShop(
                int.Parse(CurrentTexts[0]),
                () =>
                {
                    Pause = false;
                    CurrentIndex++;
                }
            );
        }

        private void OpenSystemMenu()
        {
            Pause = true;
            UIManager.OpenSystemMenu(() =>
            {
                Pause = false;
                CurrentIndex++;
            });
        }

        private void TransferPlayer()
        {
            Pause = true;
            IsWaitingCallback = true;
            ScreenManager.FadeOut(TransferPlayerFadeOutFinished);
        }

        private void TransferPlayerFadeOutFinished()
        {
            //调整玩家位置和方向
            var newPosition = new Vector3(
                int.Parse(CurrentTexts[1]),
                int.Parse(CurrentTexts[2]),
                0
            );

            if (CurrentTexts.Length > 3)
            {
                int d = int.Parse(CurrentTexts[3]);
                if (d >= 0)
                {
                    PlayerParty.SetPartyPosition(newPosition, (Mover.DirectionType)d);
                }
            }
            else
            {
                PlayerParty.SetPartyPosition(newPosition);
            }

            MapManager.GoToNewMap(CurrentTexts[0], TransferPlayerMapLoaded);
        }

        private static void TransferPlayerMapLoaded()
        {
            Pause = false;
            PlayerController.WaitCount++;
            ScreenManager.FadeIn(() => PlayerController.WaitCount--);
        }

        private void SetPlayerLocation()
        {
            var newPosition = new Vector3(
                int.Parse(CurrentTexts[0]),
                int.Parse(CurrentTexts[1]),
                0
            );

            if (CurrentTexts.Length > 2)
            {
                PlayerParty.SetPartyPosition(
                    newPosition,
                    (Mover.DirectionType)int.Parse(CurrentTexts[2])
                );
            }
            else
            {
                PlayerParty.SetPartyPosition(newPosition);
            }

            CurrentIndex++;
        }

        private void SetCharacterLocation()
        {
            var newPosition = new Vector3(
                int.Parse(CurrentTexts[1]),
                int.Parse(CurrentTexts[2]),
                0
            );

            var obj = commandSet.gameObjects[int.Parse(CurrentTexts[0])];

            if (obj.TryGetComponent<AreaBase>(out var area))
            {
                area.Position = newPosition;
            }
            else
            {
                obj.transform.position = newPosition;
            }

            if (CurrentTexts.Length > 3 && obj.TryGetComponent<Mover>(out var mover))
            {
                mover.Direction = (Mover.DirectionType)int.Parse(CurrentTexts[3]);
            }

            CurrentIndex++;
        }

        private void SetVehicleLocation()
        {
            throw new NotImplementedException();
        }

        private void GatherFolowers()
        {
            IsWaitingCallback = true;
            StartCoroutine(ProcessGatherFolowers());
        }

        private IEnumerator ProcessGatherFolowers()
        {
            var pm = PlayerParty.Player.GetComponent<Mover>();
            for (int i = 0; i < PlayerParty.Followers.Count; i++)
            {
                var mover = PlayerParty.Followers[i].GetComponent<Mover>();
                mover.Follow(pm.Position, pm.IsMoveContinue, pm.CalculateMoveTime());
                while (!mover.CanMove)
                {
                    yield return null;
                }
            }

            IsWaitingCallback = false;
            CurrentIndex++;
        }

        private void ChangePlayerTransparency()
        {
            throw new NotImplementedException();
        }

        private void ChangeFollowersTransparency()
        {
            throw new NotImplementedException();
        }

        private void EnableObject()
        {
            int objIndex = int.Parse(CurrentTexts[0]);
            commandSet.gameObjects[objIndex].SetActive(true);
            CurrentIndex++;
        }

        private void DisableObject()
        {
            int objIndex = int.Parse(CurrentTexts[0]);
            commandSet.gameObjects[objIndex].SetActive(false);
            CurrentIndex++;
        }

        private bool MovementRoute()
        {
            //找到目标对象
            int objIndex = int.Parse(CurrentTexts[0]);
            GameObject obj;
            if (objIndex < 0)
            {
                obj = PlayerParty.Player;
            }
            else
            {
                obj = commandSet.gameObjects[objIndex];
            }

            //生成指令列表
            List<CommandSet.Command> commands = new();
            for (int i = CurrentIndex + 1; i < commandSet.commands.Length; i++)
            {
                if (commandSet.commands[i].code != CT.EndMovementRoute)
                {
                    commands.Add(commandSet.commands[i]);
                }
                else
                {
                    if (CurrentTexts[3] == "1")
                    {
                        IsWaitingCallback = true;
                        obj.GetComponent<Mover>()
                            .SetMoveRoute(
                                commands,
                                CurrentTexts[1] == "1",
                                CurrentTexts[2] == "1",
                                () =>
                                {
                                    IsWaitingCallback = false;
                                    CurrentIndex = i + 1;
                                    ExecuteCommand();
                                }
                            );
                        return true;
                    }
                    else
                    {
                        obj.GetComponent<Mover>()
                            .SetMoveRoute(
                                commands,
                                CurrentTexts[1] == "1",
                                CurrentTexts[2] == "1",
                                null
                            );
                        CurrentIndex = i + 1;
                        return false;
                    }
                }
            }

            Debug.LogError($"找不到位于{CurrentIndex}的MovementRoute对应的EndMovementRoute");
            EndCommand();
            return true;
        }

        private void EndMovementRoute()
        {
            Debug.LogWarning("尝试执行指令EndMovementRoute");
            CurrentIndex++;
        }

        private void GetOnOffVehicle()
        {
            throw new NotImplementedException();
        }

        private void ScrollMap()
        {
            throw new NotImplementedException();
        }

        private void If()
        {
            bool meetCondition = false;
            switch (CurrentTexts[0])
            {
                case "0":
                    if (GetBoolVariable(CurrentTexts[1]))
                    {
                        if (CurrentTexts[2] == "1")
                        {
                            meetCondition = true;
                        }
                    }
                    else if (CurrentTexts[2] == "0")
                    {
                        meetCondition = true;
                    }
                    break;
                case "1":
                    meetCondition = CompareInt(
                        CurrentTexts[2],
                        GetIntVariable(CurrentTexts[1]),
                        CurrentTexts[3] switch
                        {
                            "0" => int.Parse(CurrentTexts[4]),
                            "1" => GetIntVariable(CurrentTexts[4]),
                            _ => 0
                        }
                    );
                    break;
            }

            if (meetCondition)
            {
                Branch.Add(CT.If);
                CurrentIndex++;
            }
            else
            {
                int depth = 0;
                for (int i = CurrentIndex + 1; i < commandSet.commands.Length; i++)
                {
                    switch (commandSet.commands[i].code)
                    {
                        case CT.If:
                            depth++;
                            break;
                        case CT.Else:
                            if (depth == 0)
                            {
                                Branch.Add(CT.If);
                                CurrentIndex = i + 1;
                                return;
                            }
                            break;
                        case CT.EndIf:
                            depth--;
                            if (depth < 0)
                            {
                                CurrentIndex = i + 1;
                                return;
                            }
                            break;
                    }
                }
                Debug.LogError($"找不到位于{CurrentIndex}的IF对应的Else或EndIf");
                EndCommand();
            }
        }

        private void Else()
        {
            SkipBranch(CT.If, CT.EndIf);
        }

        private void EndIf()
        {
            EndBranch(CT.If);
        }

        private void Loop()
        {
            Branch.Add(CT.Loop);
            CurrentIndex++;
        }

        private void BreakLoop()
        {
            SkipBranch(CT.Loop, CT.EndLoop);
        }

        private void ContinueLoop()
        {
            RepeatBranch(CT.Loop, CT.EndLoop);
        }

        private void EndLoop()
        {
            RepeatBranch(CT.Loop, CT.EndLoop);
        }

        private void ExitProcessing()
        {
            EndCommand();
        }

        private void ControlBool()
        {
            SetBoolVariable(CurrentTexts[0], CurrentTexts[1] == "1");
            CurrentIndex++;
        }

        private void ControlInt()
        {
            int v0 = GetIntVariable(CurrentTexts[0]);

            int v1 = CurrentTexts[2] switch
            {
                "0" => int.Parse(CurrentTexts[3]),
                "1" => GetIntVariable(CurrentTexts[3]),
                "2"
                    => UnityEngine
                        .Random
                        .Range(int.Parse(CurrentTexts[3]), int.Parse(CurrentTexts[4]) + 1),
                _ => 0,
            };

            int result = CurrentTexts[1] switch
            {
                "=" => v1,
                "+" => v0 + v1,
                "-" => v0 - v1,
                "*" => v0 * v1,
                "/" => v0 / v1,
                "%" => v0 % v1,
                _ => 0,
            };

            SetIntVariable(CurrentTexts[0], result);
            CurrentIndex++;
        }

        private void ChangeGold()
        {
            if (CurrentTexts[0] == "+")
            {
                Party.GainGold(GetIntVariable(CurrentTexts[0]));
            }
            else
            {
                Party.LoseGold(GetIntVariable(CurrentTexts[0]));
            }

            CurrentIndex++;
        }

        private void ChangeItem()
        {
            throw new NotImplementedException();
        }

        private void ChangePartyMember()
        {
            throw new NotImplementedException();
        }

        private void RecoverAll()
        {
            throw new NotImplementedException();
        }

        private void Wait()
        {
            TimeCount = float.Parse(CurrentTexts[0]);
            CurrentIndex++;
        }

        private void WaitAtRandom()
        {
            TimeCount = UnityEngine
                .Random
                .Range(float.Parse(CurrentTexts[0]), float.Parse(CurrentTexts[1]));
            CurrentIndex++;
        }

        private void FadeOutScreen()
        {
            Pause = true;
            IsWaitingCallback = true;
            ScreenManager.FadeOut(FadeOutScreenCallback);
        }

        private void FadeOutScreenCallback()
        {
            ExecuteCommandAfterCallback();
        }

        private void FadeInScreen()
        {
            Pause = true;
            IsWaitingCallback = true;
            ScreenManager.FadeIn(FadeInScreenCallback);
        }

        private void FadeInScreenCallback()
        {
            ExecuteCommandAfterCallback();
        }

        private void PlayBGM()
        {
            var file = Resources.Load(CurrentTexts[0]) as AudioClip;
            AudioManager.PlayBgm(file);
            CurrentIndex++;
        }

        private void PlaySE()
        {
            var file = Resources.Load(CurrentTexts[0]) as AudioClip;
            AudioManager.PlaySe(file);
            CurrentIndex++;
        }

        #region Function

        private T GetTempValue<T>(string key)
        {
            if (TempVariable.ContainsKey(key))
            {
                return (T)TempVariable[key];
            }
            else
            {
                return default;
            }
        }

        private void SetTempValue<T>(string key, T value)
        {
            if (TempVariable.ContainsKey(key))
            {
                TempVariable[key] = value;
            }
            else
            {
                TempVariable.Add(key, value);
            }
        }

        private bool GetBoolVariable(string key)
        {
            if (IsOnlyDigits(key))
            {
                return Party.BoolVariables.GetValue(int.Parse(key));
            }
            else
            {
                return GetTempValue<bool>(key);
            }
        }

        private void SetBoolVariable(string key, bool value)
        {
            if (IsOnlyDigits(key))
            {
                Party.BoolVariables.SetValue(int.Parse(key), value);
            }
            else
            {
                SetTempValue(key, value);
            }
        }

        private int GetIntVariable(string key)
        {
            if (IsOnlyDigits(key))
            {
                return Party.IntVariables.GetValue(int.Parse(key));
            }
            else
            {
                return GetTempValue<int>(key);
            }
        }

        private void SetIntVariable(string key, int value)
        {
            if (IsOnlyDigits(key))
            {
                Party.IntVariables.SetValue(int.Parse(key), value);
            }
            else
            {
                SetTempValue(key, value);
            }
        }

        private bool CompareInt(string o, int v0, int v1)
        {
            switch (o)
            {
                case "=":
                    return v0 == v1;
                case ">=":
                    return v0 >= v1;
                case "<=":
                    return v0 <= v1;
                case ">":
                    return v0 > v1;
                case "<":
                    return v0 < v1;
                case "!=":
                    return v0 != v1;
                default:
                    Debug.LogError($"\"{o}\"不能作为运算符");
                    return false;
            }
        }

        private void SkipBranch(CT start, CT end)
        {
            if (Branch.Count > 0 && Branch[^1] == start)
            {
                int depth = 0;
                for (int i = CurrentIndex + 1; i < commandSet.commands.Length; i++)
                {
                    if (commandSet.commands[i].code == start)
                    {
                        depth++;
                    }
                    else if (commandSet.commands[i].code == end)
                    {
                        depth--;
                        if (depth < 0)
                        {
                            Branch.RemoveAt(Branch.Count - 1);
                            CurrentIndex = i + 1;
                            return;
                        }
                    }
                }
                Debug.LogError($"找不到位于{CurrentIndex}的指令对应的{start}");
                EndCommand();
            }
            else
            {
                Debug.LogError($"找不到位于{CurrentIndex}的指令对应的{end}");
                EndCommand();
            }
        }

        private void RepeatBranch(CT start, CT end)
        {
            if (Branch.Count > 0 && Branch[^1] == start)
            {
                int depth = 0;
                for (int i = CurrentIndex - 1; i >= 0; i--)
                {
                    if (commandSet.commands[i].code == start)
                    {
                        depth--;
                        if (depth < 0)
                        {
                            CurrentIndex = i + 1;
                            return;
                        }
                    }
                    else if (commandSet.commands[i].code == end)
                    {
                        depth++;
                    }
                }
            }
            else
            {
                Debug.LogError($"找不到位于{CurrentIndex}的指令对应的{start}");
                EndCommand();
            }
        }

        private void EndBranch(CT start)
        {
            if (Branch.Count > 0 && Branch[^1] == start)
            {
                Branch.RemoveAt(Branch.Count - 1);
                CurrentIndex++;
            }
            else
            {
                Debug.LogError($"找不到位于{CurrentIndex}的指令对应的{start}");
                EndCommand();
            }
        }

        #endregion
    }
}
