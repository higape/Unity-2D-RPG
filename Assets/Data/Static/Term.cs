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
        public string resistance;
        public string currencyUnit;
        public string price;
        public string unitPrice;
        public string totalPrice;
        public string holdingQuantity;
        public string equippingQuantity;

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

        public string weaponPart;
        public string headPart;
        public string bodyPart;
        public string handPart;
        public string footPart;
        public string ornamentPart;

        #endregion

        #region Effect Type

        public string controlPanic;
        public string controlCharm;
        public string controlConfusion;
        public string controlFetter;

        #endregion

        #region Log

        public string battleVictory;
        public string battleDefeat;
        public string escapeVictory;
        public string escapeDefeat;

        public string selectQuantity;
        public string confirmBuy;
        public string promptCannotUseItemInMenu;

        #endregion

        #region Command

        public string newGame;
        public string continueGame;
        public string options;
        public string endGame;

        public string saveFile;
        public string loadFile;

        public string cancel;
        public string back;

        public string item;
        public string skill;
        public string equip;
        public string status;
        public string formation;

        public string attack;
        public string guard;
        public string escape;

        public string buy;
        public string sell;

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
