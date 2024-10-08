using System.Collections.Generic;
using Root;
using UnityEngine;
using UnityEngine.Events;
using BAT = Static.BattleEffect.ActorType;
using BET = Static.BattleEffect.EffectType;

namespace Dynamic
{
    public sealed class Actor : Battler
    {
        public const int WeaponCount = 3;
        public const int ArmorCount = 5;
        public const int DefaultWeaponID = -1;

        public Actor(Static.Actor dataObject)
        {
            DataObject = dataObject;
            Name = DataObject.Name;
            Level = DataObject.lv;
            Exp = DataObject.exp.GetValue(Level);

            CreateEquipment();
            CreateOriginalSkill();
            Recover();
        }

        public Actor(Static.Actor.SaveData saveData)
        {
            DataObject = ResourceManager.Actor.GetItem(saveData.id);
            Name = saveData.name;
            Level = saveData.lv;
            Exp = saveData.exp;

#if UNITY_EDITOR
            if (saveData.weaponIDs.Length != WeaponCount)
                Debug.LogError($"角色[{Name}]的武器配置错误。");

            if (saveData.armorIDs.Length != ArmorCount)
                Debug.LogError($"角色[{Name}]的防具配置错误。");
#endif

            Weapons = new(saveData.weaponIDs.Length);

            foreach (int id in saveData.weaponIDs)
            {
                if (id == 0)
                    Weapons.Add(null);
                else
                    Weapons.Add(new(id));
            }

            Armors = new(saveData.armorIDs.Length);

            for (int i = 0; i < saveData.armorIDs.Length; i++)
            {
                int id = saveData.armorIDs[i];
                if (id == 0)
                    Armors.Add(null);
                else
                    Armors.Add(new(i, id));
            }

#if UNITY_EDITOR
            if (saveData.skillConsumeCounts.Length != DataObject.skills.Length)
                Debug.LogError($"角色[{Name}]的技能配置错误。");
#endif

            CreateOriginalSkill();

            for (int i = 0; i < OriginalSkills.Count; i++)
            {
                Skill skill = OriginalSkills[i];
                skill.ConsumeCount = saveData.skillConsumeCounts[i];
            }

            Hp = saveData.hp;
        }

        public Static.Actor.SaveData ToSaveData()
        {
            Static.Actor.SaveData saveData =
                new()
                {
                    id = DataObject.id,
                    name = Name,
                    lv = Level,
                    exp = Exp,
                    hp = Hp,
                    skillConsumeCounts = new int[DataObject.skills.Length],
                    weaponIDs = new int[WeaponCount],
                    armorIDs = new int[ArmorCount]
                };

            for (int i = 0; i < saveData.skillConsumeCounts.Length; i++)
            {
                saveData.skillConsumeCounts[i] = OriginalSkills[i].ConsumeCount;
            }

            for (int i = 0; i < WeaponCount; i++)
            {
                saveData.weaponIDs[i] = Weapons[i] == null ? 0 : Weapons[i].ID;
            }

            for (int i = 0; i < ArmorCount; i++)
            {
                saveData.armorIDs[i] = Armors[i] == null ? 0 : Armors[i].ID;
            }

            return saveData;
        }

        private int hp;

        public Static.Actor DataObject { get; private set; }
        public int ID => DataObject.id;
        public Static.Actor.ActorSkin BattleSkin => DataObject.battleSkin;
        public Static.CharacterSkin CharacterSkin => DataObject.characterSkin;
        public int Exp { get; private set; }
        public int NextExp => DataObject.exp.GetValue(Level + 1);

        private List<ActorWeapon> Weapons { get; set; }
        private List<ActorArmor> Armors { get; set; }

        /// <summary>
        /// 角色原始技能
        /// </summary>
        public List<Skill> OriginalSkills { get; private set; }

        /// <summary>
        /// 已启用的原始技能
        /// </summary>
        public List<Skill> EnabledOriginalSkills
        {
            get
            {
                var list = new List<Skill>();
                foreach (var item in OriginalSkills)
                    if (item.IsEnable)
                        list.Add(item);
                return list;
            }
        }

        /// <summary>
        /// 角色拥有的技能，不包括未习得的技能
        /// </summary>
        public List<Skill> AllSkills
        {
            get
            {
                var list = new List<Skill>(EnabledOriginalSkills);
                foreach (var item in Armors)
                    if (item != null)
                        list.AddRange(item.Skills);
                foreach (var item in DurationStates)
                    if (item.Skill != null)
                        list.Add(item.Skill);
                return list;
            }
        }

        /// <summary>
        /// 战斗技能
        /// </summary>
        public List<Skill> BattleSkills
        {
            get
            {
                var list = new List<Skill>();
                foreach (var skill in AllSkills)
                {
                    if (skill.UsedInBattle)
                        list.Add(skill);
                }
                return list;
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
                    DataObject.hp.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Life);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Hp;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Life);
                return Mathf.Max(Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0), 1);
            }
        }

        public override int Atk
        {
            get
            {
                int sum =
                    DataObject.atk.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Attack);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Atk;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Attack);
                return Mathf.Max(Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0), 0);
            }
        }

        public override int Def
        {
            get
            {
                int sum =
                    DataObject.def.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Defence);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Def;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Defence);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Agi
        {
            get
            {
                int sum =
                    DataObject.agi.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Agility);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Agi;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Agility);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Hit
        {
            get
            {
                int sum =
                    DataObject.hit.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Hit);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Hit;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Hit);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Eva
        {
            get
            {
                int sum =
                    DataObject.eva.GetValue(Level)
                    + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Evasion);
                foreach (var item in Armors)
                    if (item != null)
                        sum += item.Eva;
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Evasion);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public override int Resistance
        {
            get
            {
                int sum = BaseResistance + GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Resistance);
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Resistance);
                return Mathf.Max((int)((rateDelta / 100f + 1f) * sum), 0);
            }
        }

        public int ExtraExpInBattle
        {
            get
            {
                int sum = GetAbilityOfTrait(BET.AbilityConst, (int)BAT.Exp);
                return Mathf.Max(sum, 0);
            }
        }
        public float ExpRateInBattle
        {
            get
            {
                int rateDelta = GetAbilityOfTrait(BET.AbilityRate, (int)BAT.Exp);
                return Mathf.Max(rateDelta / 100f + 1f, 0f);
            }
        }

        private int GetAbilityOfTrait(BET type0, int type1)
        {
            int sum = Static.Trait.GetValue(DurationStates, type0, type1);
            foreach (object item in Armors)
                if (item is ActorArmor armor)
                    sum += Static.Trait.GetValue(armor.Traits, type0, type1);
            foreach (object item in AllSkills)
                if (item is Skill skill)
                    sum += Static.Trait.GetValue(skill.Traits, type0, type1);
            return sum;
        }

        public override float GetElementRate(Static.ElementType elementType)
        {
            float rate = 1f;
            foreach (var item in Armors)
            {
                if (item != null)
                {
                    rate *= item.ElementGroup.GetRate(elementType);
                }
            }
            return rate;
        }

        public override int GainDp(int value)
        {
            return 0;
        }

        public override int LoseDp(int value)
        {
            return 0;
        }

        public override int Reborn(int value)
        {
            if (IsAlive)
                return 0;

            Hp = Mathf.Max(value, 1);
            (DisplayObject as Battle.DisplayActor)?.GoToAlive();
            anyChanged.Invoke(this);
            return value;
        }

        /// <summary>
        /// 通过睡觉休息时完全恢复
        /// </summary>
        public void Recover()
        {
            DurationStates.Clear();
            Hp = Mhp;
            foreach (var item in OriginalSkills)
                item.ConsumeCount = 0;
            anyChanged.Invoke(this);
        }

        public override void EnterBattle()
        {
            base.EnterBattle();
            foreach (var w in Weapons)
            {
                w?.EnterBattle();
            }
            foreach (var s in BattleSkills)
            {
                s?.EnterBattle();
            }
        }

        public override void QuitBattle()
        {
            base.QuitBattle();
            foreach (var w in Weapons)
            {
                w?.QuitBattle();
            }
            foreach (var s in BattleSkills)
            {
                s?.QuitBattle();
            }
        }

        public override void TurnEnd()
        {
            base.TurnEnd();
            foreach (var w in Weapons)
            {
                w?.TurnEnd();
            }
            foreach (var s in BattleSkills)
            {
                s?.TurnEnd();
            }
        }

        /// <summary>
        /// 获得经验值，并检查升级
        /// </summary>
        /// <returns>是否升级</returns>
        public bool GainExpInBattle(int value)
        {
            if (value > 0)
            {
                Exp += ExtraExpInBattle + Mathf.FloorToInt(value * ExpRateInBattle);
                return UpdateLevel();
            }
            return false;
        }

        /// <summary>
        /// 根据经验值刷新等级
        /// </summary>
        /// <returns>是否升级</returns>
        private bool UpdateLevel()
        {
            int oldLevel = Level;

            while (Exp >= NextExp)
            {
                Level++;
            }

            if (oldLevel != Level)
            {
                UpdateOriginalSkill();
                anyChanged.Invoke(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateEquipment()
        {
#if UNITY_EDITOR
            if (DataObject.weaponIDs.Length != WeaponCount)
                Debug.LogError($"角色[{Name}]的武器配置错误。");

            if (DataObject.armorIDs.Length != ArmorCount)
                Debug.LogError($"角色[{Name}]的防具配置错误。");
#endif

            Weapons = new(DataObject.weaponIDs.Length);

            foreach (int id in DataObject.weaponIDs)
            {
                if (id == 0)
                    Weapons.Add(null);
                else
                    Weapons.Add(new(id));
            }

            Armors = new(DataObject.armorIDs.Length);

            for (int i = 0; i < DataObject.armorIDs.Length; i++)
            {
                int id = DataObject.armorIDs[i];
                if (id == 0)
                    Armors.Add(null);
                else
                    Armors.Add(new(i, id));
            }
        }

        private void CreateOriginalSkill()
        {
            OriginalSkills = new(DataObject.skills.Length);
            for (int i = 0; i < DataObject.skills.Length; i++)
            {
                var sd = DataObject.skills[i];
                var skill = new Skill(sd.id, sd.GetCount(Level));
                OriginalSkills.Add(skill);
            }
        }

        private void UpdateOriginalSkill()
        {
            for (int i = 0; i < OriginalSkills.Count; i++)
            {
                OriginalSkills[i].MaxUsageCount = DataObject.skills[i].GetCount(Level);
            }
        }

        public ActorWeapon GetWeapon(int slotIndex) => Weapons[slotIndex];

        public ActorArmor GetArmor(int slotIndex) => Armors[slotIndex];

        public List<object> GetAllEquipments()
        {
            List<object> list = new();
            list.AddRange(Weapons);
            list.AddRange(Armors);
            return list;
        }

        /// <summary>
        /// 没有武器就生成默认武器
        /// </summary>
        public List<ActorWeapon> GetBattleWeapons()
        {
            List<ActorWeapon> list = new();
            foreach (var item in Weapons)
                if (item != null)
                    list.Add(item);
                else
                    list.Add(new ActorWeapon(DefaultWeaponID));
            return list;
        }

        /// <summary>
        /// 类型适配由UI实现，此处不检查。
        /// </summary>
        public void EquipWeapon(int slotIndex, int id)
        {
            if (Weapons[slotIndex] != null)
            {
                Party.ActorWeapon.GainItem(Weapons[slotIndex].ID, 1);
            }

            if (id == 0)
            {
                Weapons[slotIndex] = null;
            }
            else
            {
                Party.ActorWeapon.LoseItem(id, 1);
                Weapons[slotIndex] = new ActorWeapon(id);
            }

            anyChanged.Invoke(this);
        }

        /// <summary>
        /// 类型适配由UI实现，此处不检查。
        /// </summary>
        public void EquipArmor(int slotIndex, int id)
        {
            QuantityList list = slotIndex switch
            {
                0 => Party.ActorHeadArmor,
                1 => Party.ActorBodyArmor,
                2 => Party.ActorHandArmor,
                3 => Party.ActorFootArmor,
                4 => Party.ActorOrnamentArmor,
                _ => null,
            };

            if (Armors[slotIndex] != null)
            {
                list.GainItem(Armors[slotIndex].ID, 1);
                if (Armors[slotIndex] is ActorArmor armor)
                    Static.Trait.RemoveGlobalEffect(armor.Traits);
            }

            if (id == 0)
            {
                Armors[slotIndex] = null;
            }
            else
            {
                list.LoseItem(id, 1);
                Armors[slotIndex] = new ActorArmor(slotIndex, id);
                if (Armors[slotIndex] is ActorArmor armor)
                    Static.Trait.AddGlobalEffect(armor.Traits);
            }

            anyChanged.Invoke(this);
        }

        /// <summary>
        /// 拿出武器并在合适时机触发回调
        /// </summary>
        public void ShowMotion(Static.ActorWeaponSkin skin, UnityAction callback)
        {
            (DisplayObject as Battle.DisplayActor).ShowMotion(skin, callback);
        }
    }
}
