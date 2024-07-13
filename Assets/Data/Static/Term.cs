using System;
using System.Collections;
using System.Collections.Generic;
using Root;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 包含游戏内系统用语
    /// </summary>
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(Term))]
    public class Term : ScriptableObject
    {
        #region Ability

        public string lv;
        public string hp;
        public string sp;
        public string exp;
        public string nextExp;
        public string atk;
        public string def;
        public string agi;
        public string hit;
        public string eva;
        public string elementResistance;
        public string currencyUnit;
        public string price;
        public string sellingPrice;
        public string unitPrice;
        public string totalPrice;
        public string holdingQuantity;
        public string equippingQuantity;
        public string consumable;
        public string notConsume;
        public string usedOccasion;
        public string element;
        public string scope;
        public string scopeStatement;
        public string waitTime;
        public string round;
        public string effect;
        public string menu;
        public string battle;
        public string equipable;
        public string quantity;
        public string usedCount;
        public string effectRate;
        public string addedEffect;
        public string ability;
        public string equipment;
        public string durationState;

        #endregion

        #region Item Type

        public string all;

        public string recoverItem;
        public string attackItem;
        public string auxiliaryItem;
        public string normalItem;

        public string weapon;
        public string headArmor;
        public string bodyArmor;
        public string handArmor;
        public string footArmor;
        public string ornamentArmor;

        #endregion

        #region ElementType

        public string elementNormal;
        public string elementCorrosion;
        public string elementFire;
        public string elementIce;
        public string elementElectricity;
        public string elementWave;
        public string elementRay;
        public string elementGas;

        #endregion

        #region ScopeType

        public string scopeSelf;
        public string scopeOneFriend;
        public string scopeOneFriendExcludeSelf;
        public string scopeAllFriend;
        public string scopeAllFriendExcludeSelf;
        public string scopeOneDeadFriend;
        public string scopeAllDeadFriend;
        public string scopeOneEnemy;
        public string scopeAllEnemy;
        public string scopeSmallSector;
        public string scopeBigSector;
        public string scopeSmallRay;
        public string scopeBigRay;
        public string scopeSmallCircle;
        public string scopeBigCircle;

        #endregion

        #region Effect Type

        public string controlPanic;
        public string controlCharm;
        public string controlConfusion;
        public string controlFetter;

        #endregion

        #region Log

        public string gainItemFromTreasure;

        public string battleVictory;
        public string battleDefeat;
        public string escapeVictory;
        public string escapeDefeat;
        public string gainGoldInBattle;
        public string gainExpInBattle;
        public string actorLevelUp;
        public string promptActorHasNoBattleSkill;
        public string promptPanicState;

        public string selectQuantity;
        public string confirmBuy;
        public string promptCannotUseItemInMenu;
        public string promptNoSelectableTarget;

        public string skillUseItem;

        #endregion

        #region Command

        public string newGame;
        public string continueGame;
        public string settings;
        public string endGame;

        public string saveFile;
        public string loadFile;
        public string createFile;

        public string cancel;
        public string back;

        public string item;
        public string skill;
        public string equip;
        public string status;
        public string formation;
        public string notEquip;

        public string attack;
        public string guard;
        public string escape;

        public string buy;
        public string sell;

        public string bgm;
        public string se;

        #endregion

        #region Function

        public string GetText(BattleEffect.ControlType type) =>
            type switch
            {
                BattleEffect.ControlType.Panic => controlPanic,
                BattleEffect.ControlType.Charm => controlCharm,
                BattleEffect.ControlType.Confusion => controlConfusion,
                BattleEffect.ControlType.Fetter => controlFetter,
                _ => " ",
            };

        public string GetText(ElementType type) =>
            type switch
            {
                ElementType.Normal => elementNormal,
                ElementType.Corrosion => elementCorrosion,
                ElementType.Fire => elementFire,
                ElementType.Ice => elementIce,
                ElementType.Electricity => elementElectricity,
                ElementType.Wave => elementWave,
                ElementType.Ray => elementRay,
                ElementType.Gas => elementGas,
                _ => " "
            };

        public string GetText(UsedScope type) =>
            type switch
            {
                UsedScope.Self => scopeSelf,
                UsedScope.OneFriend => scopeOneFriend,
                UsedScope.OneFriendExcludeSelf => scopeOneFriendExcludeSelf,
                UsedScope.AllFriend => scopeAllFriend,
                UsedScope.AllFriendExcludeSelf => scopeAllFriendExcludeSelf,
                UsedScope.OneDeadFriend => scopeOneDeadFriend,
                UsedScope.AllDeadFriend => scopeAllDeadFriend,
                UsedScope.OneEnemy => scopeOneEnemy,
                UsedScope.AllEnemy => scopeAllEnemy,
                UsedScope.SmallSector => scopeSmallSector,
                UsedScope.BigSector => scopeBigSector,
                UsedScope.SmallRay => scopeSmallRay,
                UsedScope.BigRay => scopeBigRay,
                UsedScope.SmallCircle => scopeSmallCircle,
                UsedScope.BigCircle => scopeBigCircle,
                _ => " "
            };

        public string GetText(ActorUsableItem.ItemType type) =>
            type switch
            {
                ActorUsableItem.ItemType.RecoverItem => recoverItem,
                ActorUsableItem.ItemType.AttackItem => attackItem,
                ActorUsableItem.ItemType.AuxiliaryItem => auxiliaryItem,
                _ => " "
            };

#if UNITY_EDITOR
        /// <summary>
        /// 检查数组的索引是否超出数组范围，true为超出
        /// </summary>
        public bool CheckOutOfRange(int index, int length)
        {
            if (index < 0 || index >= length)
            {
                Debug.Log("索引值超出范围。");
                return true;
            }
            return false;
        }
#endif

        #endregion
    }
}
