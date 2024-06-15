using System;
using System.Collections;
using System.Collections.Generic;
using Root;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 管理玩家财产的类。
    /// 提供API可查找玩家拥有的各种物品，以及一些系数
    /// </summary>
    public static class Party
    {
        public const int MaxBattleMembers = 3;
        public const int MaxGold = 99999999;
        public const int MaxItemQuantity = 99;

        private static int gold;

        public static int Gold
        {
            get => gold;
            private set
            {
                gold = Math.Clamp(value, 0, MaxGold);
                goldChanged.Invoke(gold);
            }
        }

        /// <summary>
        /// 战斗金钱比例
        /// </summary>
        public static float BattleGoldRate => ITrait.GoldRate;

        /// <summary>
        /// 卖价比例，通常与原价相乘的结果作为卖价
        /// </summary>
        public static float SellingPriceRate => 0.5f;

        /// <summary>
        /// 用于游戏逻辑的bool变量
        /// </summary>
        public static VariableDictionary<int, bool> BoolVariables { get; private set; }

        /// <summary>
        /// 用于游戏逻辑的int变量
        /// </summary>
        public static VariableDictionary<int, int> IntVariables { get; private set; }

        public static QuantityList ActorWeapon { get; private set; }
        public static QuantityList ActorHeadArmor { get; private set; }
        public static QuantityList ActorBodyArmor { get; private set; }
        public static QuantityList ActorHandArmor { get; private set; }
        public static QuantityList ActorFootArmor { get; private set; }
        public static QuantityList ActorOrnamentArmor { get; private set; }
        public static QuantityList ActorRecoverItem { get; private set; }
        public static QuantityList ActorAttackItem { get; private set; }
        public static QuantityList ActorAuxiliaryItem { get; private set; }
        public static QuantityList ActorNormalItem { get; private set; }

        /// <summary>
        /// 所有角色，包括未加入队伍的
        /// </summary>
        public static List<Actor> AllActorList { get; private set; }

        /// <summary>
        /// 在队伍中的角色列表
        /// </summary>
        public static List<Actor> PartyActorList { get; set; }

        public static int PartyActorCount => PartyActorList.Count;

        public static void GainGold(int value)
        {
            if (value > 0)
                Gold += value;
        }

        public static void LoseGold(int value)
        {
            if (value > 0)
                Gold -= value;
        }

        public static QuantityList GetActorItemList(Static.ActorUsableItem.ItemType type)
        {
            return type switch
            {
                Static.ActorUsableItem.ItemType.RecoverItem => ActorRecoverItem,
                Static.ActorUsableItem.ItemType.AttackItem => ActorAttackItem,
                Static.ActorUsableItem.ItemType.AuxiliaryItem => ActorAuxiliaryItem,
                _ => null,
            };
        }

        public static QuantityList GetActorArmorList(int slotIndex) =>
            slotIndex switch
            {
                0 => ActorHeadArmor,
                1 => ActorBodyArmor,
                2 => ActorHandArmor,
                3 => ActorFootArmor,
                4 => ActorOrnamentArmor,
                _ => null,
            };

        public static void GainActorArmor(int slotIndex, int id, int value)
        {
            if (value > 0)
                GetActorArmorList(slotIndex).GainItem(id, value);
        }

        public static void LoseActorArmor(int slotIndex, int id, int value)
        {
            if (value > 0)
                GetActorArmorList(slotIndex).LoseItem(id, value);
        }

        /// <summary>
        /// 队伍中排列靠前的几名角色
        /// </summary>
        public static List<Actor> GetBattleActorList()
        {
            List<Actor> actors = new();

            for (int i = 0; i < PartyActorList.Count && i < MaxBattleMembers; i++)
                actors.Add(PartyActorList[i]);

            return actors;
        }

        public static int GetBattleActorCount() =>
            Mathf.Min(PartyActorList.Count, MaxBattleMembers);

        /// <summary>
        /// 活的参战角色
        /// </summary>
        public static List<Actor> GetAliveBattleActorList()
        {
            List<Actor> actors = new();

            for (int i = 0; i < PartyActorList.Count && i < MaxBattleMembers; i++)
                if (PartyActorList[i].IsAlive)
                    actors.Add(PartyActorList[i]);

            return actors;
        }

        /// <summary>
        /// 死的参战角色
        /// </summary>
        public static List<Actor> GetDeadBattleActorList()
        {
            List<Actor> actors = new();

            for (int i = 0; i < PartyActorList.Count && i < MaxBattleMembers; i++)
                if (!PartyActorList[i].IsAlive)
                    actors.Add(PartyActorList[i]);

            return actors;
        }

        /// <summary>
        /// 获取参战的角色
        /// </summary>
        public static Actor GetBattleActor(int index)
        {
            if (index < MaxBattleMembers && index < PartyActorList.Count && index >= 0)
                return PartyActorList[index];
            return null;
        }

        /// <summary>
        /// 获取队伍里的角色
        /// </summary>
        public static Actor GetPartyActorByIndex(int index)
        {
            if (index < PartyActorList.Count && index >= 0)
                return PartyActorList[index];
            return null;
        }

        /// <summary>
        /// 获取队伍里的角色
        /// </summary>
        public static Actor GetPartyActorByID(int id)
        {
            foreach (Actor actor in PartyActorList)
                if (actor.ID == id)
                    return actor;
            return null;
        }

        /// <summary>
        /// 从新游戏初始化
        /// </summary>
        public static void InitializeByNewGame()
        {
            //暂时将数据放在代码里

            Gold = 10000;
            BoolVariables = new();
            IntVariables = new();

            //创建角色实例
            AllActorList = new();
            foreach (var item in ResourceManager.Actor.ItemList)
            {
                AllActorList.Add(new(item));
            }

            //将角色加入队伍
            PartyActorList = new();
            foreach (var item in AllActorList)
                PartyActorList.Add(item);

            const int quantity = 1;
            //添加物品
            ActorWeapon = new();
            foreach (var item in ResourceManager.ActorWeapon.ItemList)
                ActorWeapon.GainItem(item.id, quantity);
            ActorWeapon.LoseItem(-1, 1000);

            ActorHeadArmor = new();
            foreach (var item in ResourceManager.ActorHeadArmor.ItemList)
                ActorHeadArmor.GainItem(item.id, quantity);

            ActorBodyArmor = new();
            foreach (var item in ResourceManager.ActorBodyArmor.ItemList)
                ActorBodyArmor.GainItem(item.id, quantity);

            ActorHandArmor = new();
            foreach (var item in ResourceManager.ActorHandArmor.ItemList)
                ActorHandArmor.GainItem(item.id, quantity);

            ActorFootArmor = new();
            foreach (var item in ResourceManager.ActorFootArmor.ItemList)
                ActorFootArmor.GainItem(item.id, quantity);

            ActorOrnamentArmor = new();
            foreach (var item in ResourceManager.ActorOrnamentArmor.ItemList)
                ActorOrnamentArmor.GainItem(item.id, quantity);

            ActorRecoverItem = new();
            foreach (var item in ResourceManager.ActorRecoverItem.ItemList)
                ActorRecoverItem.GainItem(item.id, quantity);

            ActorAttackItem = new();
            foreach (var item in ResourceManager.ActorAttackItem.ItemList)
                ActorAttackItem.GainItem(item.id, quantity);

            ActorAuxiliaryItem = new();
            foreach (var item in ResourceManager.ActorAuxiliaryItem.ItemList)
                ActorAuxiliaryItem.GainItem(item.id, quantity);

            ActorNormalItem = new();
            foreach (var item in ResourceManager.ActorNormalItem.ItemList)
                ActorNormalItem.GainItem(item.id, quantity);
        }

        /// <summary>
        /// 从存档初始化
        /// </summary>
        public static void InitializeBySaveFile() { }

        private static readonly UnityEvent<int> goldChanged = new();

        public static void RegisterGoldChangeCallback(UnityAction<int> callback)
        {
            goldChanged.AddListener(callback);
        }

        public static void UnregisterGoldChangeCallback(UnityAction<int> callback)
        {
            goldChanged.RemoveListener(callback);
        }

        private static readonly UnityEvent memberSorted = new();

        public static void RegisterMemberSortCallback(UnityAction callback)
        {
            memberSorted.AddListener(callback);
        }

        public static void UnregisterMemberSortCallback(UnityAction callback)
        {
            memberSorted.RemoveListener(callback);
        }
    }
}
