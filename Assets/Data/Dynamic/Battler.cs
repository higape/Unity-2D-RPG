using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ET = Static.ElementType;

namespace Dynamic
{
    /// <summary>
    /// 战斗成员的父类，声明必要属性。
    /// </summary>
    public abstract class Battler
    {
        public const int BaseResistance = 100;

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
        public abstract int Resistance { get; }

        /// <summary>
        /// 持续效果
        /// </summary>
        protected List<DurationState> DurationStates { get; set; } = new();

        public DurationState[] GetDurationStates() => DurationStates.ToArray();

        public abstract float GetElementRate(Static.ElementType elementType);

        /// <summary>
        /// 获取所有元素抗性，抗性的值以整数表示
        /// </summary>
        public Static.ElementGroup GetElementGroup() =>
            new()
            {
                Normal = 100 - Mathf.RoundToInt(GetElementRate(ET.Normal) * 100),
                Corrosion = 100 - Mathf.RoundToInt(GetElementRate(ET.Corrosion) * 100),
                Fire = 100 - Mathf.RoundToInt(GetElementRate(ET.Fire) * 100),
                Ice = 100 - Mathf.RoundToInt(GetElementRate(ET.Ice) * 100),
                Electricity = 100 - Mathf.RoundToInt(GetElementRate(ET.Electricity) * 100),
                Wave = 100 - Mathf.RoundToInt(GetElementRate(ET.Wave) * 100),
                Ray = 100 - Mathf.RoundToInt(GetElementRate(ET.Ray) * 100),
                Gas = 100 - Mathf.RoundToInt(GetElementRate(ET.Gas) * 100),
            };

        /// <summary>
        /// 获取控制状态
        /// </summary>
        public Static.BattleEffect.ControlType ControlState
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
#if UNITY_EDITOR
                        if (state.EffectType1 >= 1 && state.EffectType1 <= 4)
#endif
                            controlValues[state.EffectType1 - 1] +=
                                state.EffectValue * GetElementRate(state.ElementType);
                    }
                }

                //根据优先级处理状态
                for (int i = controlValues.Length - 1; i >= 0; i--)
                {
                    if (controlValues[i] >= Resistance)
                    {
                        return (Static.BattleEffect.ControlType)(i + 1);
                    }
                }

                //没有控制状态
                return Static.BattleEffect.ControlType.None;
            }
        }

        /// <returns>实际获得量</returns>
        public int GainHp(int value)
        {
            int effective = 0;
            if (value > 0 && IsAlive)
            {
                int old = Hp;

                if (Hp + value > Mhp)
                    Hp = Mhp;
                else
                    Hp += value;

                anyChanged.Invoke(this);

                effective = Hp - old;
            }

            if (Battle.BattleManager.IsBattling && DisplayObject != null)
                Battle
                    .BattleManager
                    .CreateDigit(
                        effective,
                        Battle.PopDigit.DigitStyle.Recover,
                        DisplayObject.Position
                    );

            return effective;
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

            if (DisplayObject != null)
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

            if (DisplayObject != null)
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

            DisplayObject?.RefreshDurationState(DurationStates);
            anyChanged.Invoke(this);
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

            DisplayObject?.RefreshDurationState(DurationStates);
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
        public virtual void QuitBattle()
        {
            DurationStates.Clear();
            if (DisplayObject != null)
            {
                Object.Destroy(DisplayObject.gameObject);
                DisplayObject = null;
            }
        }

        public virtual void TurnEnd()
        {
            UpdateDurationState();
        }

        /// <summary>
        /// 更新状态的回合并发动效果
        /// </summary>
        public void UpdateDurationState()
        {
            bool hasChange = false;

            for (int i = 0; i < DurationStates.Count; i++)
            {
                DurationState state = DurationStates[i];
                state.OnEffect();
                if (--state.LastTurn <= 0)
                {
                    DurationStates.RemoveAt(i);
                    hasChange = true;
                }
            }

            if (hasChange)
                DisplayObject.RefreshDurationState(DurationStates);
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
