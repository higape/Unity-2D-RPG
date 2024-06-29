using System;
using System.Collections.Generic;
using Root;
using UnityEngine;
using BAT = Static.BattleEffect.ActorType;
using BET = Static.BattleEffect.EffectType;

namespace Dynamic
{
    public sealed class Enemy : Battler, ITrait
    {
        public Enemy(int id, int level)
        {
            DataObject = ResourceManager.Enemy.GetItem(id);
            Name = DataObject.Name;
            Level = level;
            Hp = Mhp;
            Actions = new(DataObject.actions.Length);
            foreach (var item in DataObject.actions)
            {
                Actions.Add(new(item));
            }
        }

        private int hp;
        public Static.Enemy DataObject { get; private set; }
        public Sprite Skin => DataObject.skin;

        /// <summary>
        /// 身体在世界坐标的所占范围。
        /// 用来计算战斗者是否在攻击范围内。
        /// 采用 Unity Unit。
        /// </summary>
        public Rect ScopeRect
        {
            get
            {
                Rect skinRect = Skin.rect;
                Vector3 position = DisplayObject.Position;
                return new Rect(
                    skinRect.x / Skin.pixelsPerUnit + position.x,
                    skinRect.y / Skin.pixelsPerUnit + position.y,
                    skinRect.width / Skin.pixelsPerUnit,
                    skinRect.height / Skin.pixelsPerUnit
                );
            }
        }

        public override bool IsAlive => Hp > 0;
        public override Static.Battler.BattlerType BattlerType => DataObject.type;

        public override int Hp
        {
            get => hp;
            protected set => hp = Mathf.Clamp(value, 0, Mhp);
        }

        public override int Mhp
        {
            get
            {
                int sum =
                    DataObject.hp.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Life);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Life);
                return Mathf.Max(Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0), 1);
            }
        }

        public override int Atk
        {
            get
            {
                int sum =
                    DataObject.atk.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Attack);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Attack);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Def
        {
            get
            {
                int sum =
                    DataObject.def.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Defence);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Defence);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Agi
        {
            get
            {
                int sum =
                    DataObject.agi.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Agility);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Agility);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Hit
        {
            get
            {
                int sum =
                    DataObject.hit.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Hit);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Hit);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Eva
        {
            get
            {
                int sum =
                    DataObject.eva.GetValue(Level) + GetAbility(BET.AbilityConst, (int)BAT.Evasion);
                int rateDelta = GetAbility(BET.AbilityRate, (int)BAT.Evasion);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public int Exp => DataObject.exp.GetValue(Level);

        public int Gold => DataObject.gold.GetValue(Level);

        private int ActionCount { get; set; }

        public int MaxActionCount => DataObject.actionCount;

        public List<EnemyAction> Actions { get; private set; }

        public IEnumerable<Static.TraitData> Traits => DataObject.traits;

        private int GetAbility(BET type0, int type1) =>
            (this as ITrait).GetValue(type0, type1) + ITrait.GetValue(DurationStates, type0, type1);

        public override float GetElementRate(Static.ElementType elementType) =>
            DataObject.elementGroup.GetRate(elementType);

        public override int GainDp(int value)
        {
            if (value <= 0)
                return 0;

            var list = new List<IDurability>();
            foreach (var item in Actions)
                if (!(item as IDurability).IsFullDurability)
                    list.Add(item);

            int old = value;
            int randomIndex;
            while (value > 0 && list.Count != 0)
            {
                randomIndex = UnityEngine.Random.Range(0, list.Count);
                list[randomIndex].GainDurability();
                if (list[randomIndex].IsFullDurability)
                    list.RemoveAt(randomIndex);
                value--;
            }

            return old - value;
        }

        public override int LoseDp(int value)
        {
            if (value <= 0)
                return 0;

            var list = new List<IDurability>();
            foreach (var item in Actions)
                if (!(item as IDurability).IsZeroDurability)
                    list.Add(item);

            int old = value;
            int randomIndex;
            while (value > 0 && list.Count != 0)
            {
                randomIndex = UnityEngine.Random.Range(0, list.Count);
                list[randomIndex].GainDurability();
                if (list[randomIndex].IsZeroDurability)
                    list.RemoveAt(randomIndex);
                value--;
            }

            return old - value;
        }

        public override int Reborn(int value)
        {
            Debug.Log("死掉的怪无法复活");
            return 0;
        }

        /// <summary>
        /// 重置行动计数
        /// </summary>
        public void ResetActionCount()
        {
            ActionCount = 0;
        }

        /// <summary>
        /// 返回一个符合条件的随机行动
        /// </summary>
        public EnemyAction GetAction()
        {
            if (ActionCount++ < MaxActionCount)
            {
                var list = MakeUsableAction();
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
            return null;
        }

        /// <summary>
        /// 生成一个可以执行的行动列表
        /// </summary>
        private List<EnemyAction> MakeUsableAction()
        {
            var list = new List<EnemyAction>();
            foreach (var item in Actions)
                if (item.CanUse)
                    list.Add(item);
            return list;
        }
    }
}
