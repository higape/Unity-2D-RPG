using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 战斗成员的父类，声明必要属性。
    /// </summary>
    public abstract class Battler
    {
        public Battle.DisplayBattler DisplayObject { get; set; }
        public string Name { get; protected set; }
        public abstract bool IsAlive { get; }
        public abstract Static.Battler.BattlerType BattlerType { get; }
        public int Level { get; protected set; } = 1;
        public abstract int Hp { get; protected set; }
        public abstract int Mhp { get; }
        public abstract int Atk { get; }
        public abstract int Def { get; }
        public abstract int Agi { get; }
        public abstract int Hit { get; }
        public abstract int Eva { get; }

        /// <summary>
        /// 持续效果
        /// </summary>
        protected List<DurationState> DurationStates { get; set; } = new();

        public abstract float GetElementRate(Static.ElementType elementType);

        /// <summary>
        /// 获取效果值最大的状态以及状态值
        /// </summary>
        public (Static.BattleEffect.ControlType, int) ControlInfo
        {
            get
            {
                //将状态值保存在数组，数组下标对应状态类型枚举值-1
                float[] controlValues = new float[4] { 0, 0, 0, 0 };

                //收集状态值
                foreach (var state in DurationStates)
                {
                    if (state.EffectType0 == Static.BattleEffect.EffectType.Control)
                    {
                        controlValues[state.EffectType1 - 1] +=
                            state.EffectValue * GetElementRate(state.ElementType);
                    }
                }
                //查找最大状态值以及对应的状态类型
                float sum = controlValues[0];
                int maxIndex = 0;
                for (int i = 1; i < controlValues.Length; i++)
                {
                    sum += controlValues[i];
                    if (controlValues[i] >= controlValues[maxIndex])
                    {
                        maxIndex = i;
                    }
                }

                int controlValue = Mathf.RoundToInt(sum);
                if (controlValue > 0)
                    return ((Static.BattleEffect.ControlType)(maxIndex + 1), controlValue);
                else
                    return (Static.BattleEffect.ControlType.None, 0);
            }
        }

        public Static.BattleEffect.ControlType ControlState
        {
            get
            {
                int controlType = (int)Static.BattleEffect.ControlType.None;
                float controlValue = 0;

                foreach (var state in DurationStates)
                {
                    if (state.EffectType0 == Static.BattleEffect.EffectType.Control)
                    {
                        controlValue += state.EffectValue * GetElementRate(state.ElementType);
                        controlType = Mathf.Max(controlType, state.EffectType1);
                    }
                }

                if (controlValue > 98f)
                    return (Static.BattleEffect.ControlType)controlType;
                else
                    return Static.BattleEffect.ControlType.None;
            }
        }

        public int ControlValue
        {
            get
            {
                float controlValue = 0;

                foreach (var state in DurationStates)
                    if (state.EffectType0 == Static.BattleEffect.EffectType.Control)
                        controlValue += state.EffectValue * GetElementRate(state.ElementType);

                if (controlValue > 98f)
                    return 100;
                else if (controlValue > 1f)
                    return (int)controlValue;
                else
                    return 0;
            }
        }

        /// <returns>实际获得量</returns>
        public int GainHp(int value)
        {
            if (value > 0 && IsAlive)
            {
                int old = Hp;

                if (Hp + value > Mhp)
                    Hp = Mhp;
                else
                    Hp += value;

                anyChanged.Invoke(this);

                return Hp - old;
            }

            return 0;
        }

        /// <returns>实际减少量</returns>
        public virtual int LoseHp(int value)
        {
            int damage = 0;
            if (value > 0 && IsAlive)
            {
                int old = Hp;

                if (Hp - value <= 0)
                {
                    Hp = 0;
                    GoToDie();
                }
                else
                {
                    Hp -= value;
                }

                anyChanged.Invoke(this);

                damage = old - Hp;
            }
            if (Battle.BattleManager.IsBattling)
                Battle
                    .BattleManager
                    .CreateDigit(damage, Battle.PopDigit.DigitStyle.Damage, DisplayObject.Position);

            return damage;
        }

        public abstract int GainDp(int value);
        public abstract int LoseDp(int value);

        /// <summary>
        /// 复活，复活时的回血对活人无效
        /// </summary>
        /// <param name="value">回血量</param>
        /// <returns>实际回血量，对无效目标返回0</returns>
        public abstract int Reborn(int value);

        public void GoToDie()
        {
            Hp = 0;

            if (Battle.BattleManager.IsBattling)
            {
                DisplayObject.GoToDie();
            }
            //发送死讯
            Debug.Log($"{this.Name}死了。");
        }

        public void AddState(DurationState state)
        {
            bool holdState = true;
            foreach (var item in DurationStates)
            {
                if (state.IsWeakerThan(item) || item.IsStrongerThan(state))
                {
                    holdState = false;
                    break;
                }
            }

            //添加状态，清除克制的其它状态
            if (holdState)
            {
                for (int i = 0; i < DurationStates.Count; i++)
                {
                    DurationState item = DurationStates[i];
                    if (state.IsStrongerThan(item) || item.IsWeakerThan(state))
                    {
                        DurationStates.RemoveAt(i);
                        i--;
                    }
                }

                DurationStates.Add(state);
            }
            //不添加状态，清除一个克制的其它状态
            else
            {
                for (int i = 0; i < DurationStates.Count; i++)
                {
                    DurationState item = DurationStates[i];
                    if (state.IsStrongerThan(item) || item.IsWeakerThan(state))
                    {
                        DurationStates.RemoveAt(i);
                        break;
                    }
                }
            }

            anyChanged.Invoke(this);
            DisplayObject.RefreshDurationState(DurationStates);
        }

        public void RemoveState(Static.BattleEffect.StateType type0, int type1)
        {
            int i;

            switch (type0)
            {
                case Static.BattleEffect.StateType.ID:
                    for (i = 0; i < DurationStates.Count; )
                    {
                        if (DurationStates[i].ID == type1)
                            DurationStates.RemoveAt(i);
                        else
                            i++;
                    }
                    break;
                case Static.BattleEffect.StateType.EffectType:
                    for (i = 0; i < DurationStates.Count; )
                    {
                        if ((int)DurationStates[i].EffectType0 == type1)
                            DurationStates.RemoveAt(i);
                        else
                            i++;
                    }
                    break;
                case Static.BattleEffect.StateType.ElementType:
                    for (i = 0; i < DurationStates.Count; )
                    {
                        if ((int)DurationStates[i].ElementType == type1)
                            DurationStates.RemoveAt(i);
                        else
                            i++;
                    }
                    break;
                case Static.BattleEffect.StateType.Buff:
                    Debug.Log("暂时无法判断增益");
                    break;
                case Static.BattleEffect.StateType.Debuff:
                    Debug.Log("暂时无法判断减益");
                    break;
            }

            DisplayObject.RefreshDurationState(DurationStates);
            anyChanged.Invoke(this);
        }

        /// <summary>
        /// 进入战斗，初始化各种数据
        /// </summary>
        public virtual void EnterBattle()
        {
            if (Hp > Mhp)
                Hp = Mhp;
        }

        /// <summary>
        /// 退出战斗时清理各种数据
        /// </summary>
        public virtual void QuiteBattle() { }

        /// <summary>
        /// 更新状态的回合并发动效果
        /// </summary>
        public void UpdateDurationState()
        {
            for (int i = 0; i < DurationStates.Count; i++)
            {
                DurationState state = DurationStates[i];
                state.OnEffect();
                if (--state.LastTurn <= 0)
                    DurationStates.RemoveAt(i);
            }

            anyChanged.Invoke(this);
        }

        protected readonly UnityEvent<Battler> anyChanged = new();

        public void RegisterAnyChangeCallback(UnityAction<Battler> callback)
        {
            anyChanged.AddListener(callback);
        }

        public void UnregisterAnyChangeCallback(UnityAction<Battler> callback)
        {
            anyChanged.RemoveListener(callback);
        }
    }
}
