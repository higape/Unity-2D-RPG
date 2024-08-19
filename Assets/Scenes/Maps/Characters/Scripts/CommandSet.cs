using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class CommandSet
    {
        public enum CommandType
        {
            #region Panel

            [InspectorName("显示文本[文本索引,...]")]
            ShowText = 100,

            [InspectorName("显示选项[接收结果的变量ID或名称,文本索引,选项文本索引,...]")]
            ShowChoices = 110,

            [InspectorName("输入金额[变量ID或名称,文本索引]")]
            InputGold = 200,

            // InputName,

            [InspectorName("战斗[文件名],接收结果的变量ID或名称]")]
            Battle = 300,

            [InspectorName("商店[ID]")]
            Shop = 310,

            [InspectorName("打开系统菜单")]
            OpenSystemMenu = 400,

            #endregion

            #region Map

            [InspectorName("传送玩家[地图名,x,y],方向]")]
            TransferPlayer = 1000,

            [InspectorName("设置玩家位置[x,y],方向]")]
            SetPlayerLocation = 1001,

            [InspectorName("设置角色位置[对象索引,x,y],方向]")]
            SetCharacterLocation = 1002,

            [InspectorName("设置交通工具位置[地图名,x,y],方向]")]
            SetVehicleLocation = 1003,

            [InspectorName("集合队友")]
            GatherFolowers = 1010,

            [InspectorName("更改玩家透明度[0.0~1.0]")]
            ChangePlayerTransparency = 1100,

            [InspectorName("更改队友透明度[0.0~1.0]")]
            ChangeFollowersTransparency = 1101,

            [InspectorName("启用对象[对象索引]")]
            EnableObject = 1200,

            [InspectorName("禁用对象[对象索引]")]
            DisableObject = 1201,

            [InspectorName("设置移动路径[对象索引,重复,失败时可跳过,等待完成]")]
            MovementRoute = 1302,

            [InspectorName("结束移动路径")]
            EndMovementRoute = 1303,

            [InspectorName("上下交通工具")]
            GetOnOffVehicle = 1400,

            [InspectorName("滚动地图[x,y,timePerUnit]")]
            ScrollMap = 1500,

            #endregion

            #region Process

            [InspectorName("如果[...]")]
            If = 2000,

            [InspectorName("否则")]
            Else = 2002,

            [InspectorName("结束如果")]
            EndIf = 2003,

            [InspectorName("循环")]
            Loop = 2010,

            [InspectorName("跳出循环")]
            BreakLoop = 2011,

            [InspectorName("下一轮循环")]
            ContinueLoop = 2012,

            [InspectorName("结束循环")]
            EndLoop = 2013,

            [InspectorName("退出进程")]
            ExitProcessing = 2100,

            #endregion

            #region Data

            [InspectorName("操作Bool[ID或名称,值]")]
            ControlBool = 3000,

            [InspectorName("操作Int[ID或名称,运算符,来源,来源参数0,来源参数1]")]
            ControlInt = 3001,

            [InspectorName("更改金钱[运算符,ID或名称]")]
            ChangeGold = 3100,

            [InspectorName("更改物品数量[物品类型,物品ID,运算符,来源,来源参数0,来源参数1]")]
            ChangeItem = 3200,

            [InspectorName("更改队伍成员[角色ID,运算符]")]
            ChangePartyMember = 3300,

            // ChangeName,
            // ChangeEXP,
            // ChangeLevel,
            // ChangeHP,
            // ChangeSP,
            // ChangeEqiupment,

            [InspectorName("完全回复[...]")]
            RecoverAll = 3900,

            #endregion

            #region Other

            [InspectorName("等待[秒]")]
            Wait = 4000,

            [InspectorName("随机等待[最小时间,最大时间]")]
            WaitAtRandom = 4001,

            [InspectorName("淡出画面")]
            FadeoutScreen = 4100,

            [InspectorName("淡入画面")]
            FadeinScreen = 4101,

            [InspectorName("播放BGM[文件路径]")]
            PlayBGM = 4200,

            [InspectorName("播放SE[文件路径]")]
            PlaySE = 4201,

            [InspectorName("注释")]
            Comment = 4900,

            #endregion

            #region Move Command

            [InspectorName("移动[方向][方向,步数]")]
            Move = 10000,

            [InspectorName("随机移动[][步数][最小步数,最大步数]")]
            MoveAtRandom = 10001,

            [InspectorName("靠近玩家")]
            MoveTowardPlayer = 10010,

            [InspectorName("远离玩家")]
            MoveAwayFromPlayer = 10011,

            [InspectorName("前进[][步数]")]
            StepForward = 10020,

            [InspectorName("后退(不转向)[][步数]")]
            StepBackward = 10021,

            [InspectorName("转向[方向]")]
            Turn = 10100,

            [InspectorName("相对转向[方向]")]
            TurnAtRelative = 10101,

            [InspectorName("随机转向")]
            TurnAtRandom = 10102,

            [InspectorName("面向玩家")]
            TurnTowardPlayer = 10110,

            [InspectorName("背对玩家")]
            TurnAwayFromPlayer = 10111,

            [InspectorName("改移动速度(单位/秒)[速度]")]
            ChangeMoveSpeed = 10200,

            [InspectorName("禁用移动范围[x,y,宽,高]")]
            DisableMoveScope = 10301,

            [InspectorName("设置移动范围[x,y,宽,高]")]
            SetMoveScope = 10302,

            // SwitchWalkingAnimation,
            // SwitchSteppingAnimation,
            // SwitchDirectionFix,
            // SwitchThrough,
            // SwitchTransparent,

            #endregion
        }

        [Serializable]
        public class Command
        {
            public CommandType code;
            public string parameter;
        }

        public bool playerWaiting;

        /// <summary>
        /// 给指令使用的地图对象
        /// </summary>
        public GameObject[] gameObjects;

        /// <summary>
        /// 给指令使用的文本，支持换行
        /// </summary>
        [Multiline]
        public string[] texts;

        public Command[] commands;
    }
}
