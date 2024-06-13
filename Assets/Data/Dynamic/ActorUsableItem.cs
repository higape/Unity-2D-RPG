using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 战斗中使用的道具
    /// </summary>
    public class ActorUsableItem : Weapon, ICommodity
    {
        public ActorUsableItem(Static.ActorUsableItem.ItemType type, int id)
        {
            ItemType = type;
            switch (ItemType)
            {
                case Static.ActorUsableItem.ItemType.RecoverItem:
                    DataObject = Root.ResourceManager.ActorRecoverItem.GetItem(id);
                    break;
                case Static.ActorUsableItem.ItemType.AttackItem:
                    DataObject = Root.ResourceManager.ActorAttackItem.GetItem(id);
                    break;
                case Static.ActorUsableItem.ItemType.AuxiliaryItem:
                    DataObject = Root.ResourceManager.ActorAuxiliaryItem.GetItem(id);
                    break;
                default:
                    Debug.LogError($"不能实例化类型为{type}的ActorUsableItem.");
                    break;
            }
        }

        Static.ActorUsableItem.ItemType ItemType { get; set; }
        private Static.ActorUsableItem DataObject { get; set; }
        public int ID => DataObject.id;
        public string Name => DataObject.Name;
        public Static.ActorWeaponSkin Skin => DataObject.Skin;
        public bool Consumable => DataObject.consumable;
        public bool UsedInMenu => DataObject.UsedInMenu;
        public bool UsedInBattle => DataObject.UsedInBattle;
        public override int Attack => 0;
        public Static.WeaponUsage Usage => DataObject.Usage;
        protected override Vector3 FirePosition => Vector3.zero;
        public int Price => DataObject.price;
        public int SellingPrice => (int)(Price * Party.SellingPriceRate);

        public void Buy(int quantity)
        {
            Party.GetActorItemList(ItemType).GainItem(ID, quantity);
            Party.LoseGold(Price * quantity);
        }

        public void Sell(int quantity)
        {
            Party.GetActorItemList(ItemType).LoseItem(ID, quantity);
            Party.GainGold(SellingPrice * quantity);
        }

        protected override void StartBullet()
        {
            if (Owner is Actor human)
            {
                human.ShowMotion(Skin, ProcessBullet);
            }
            else
            {
                ProcessBullet();
            }
        }

        public override void CostAndCool()
        {
            //物品无冷却
            if (Consumable)
                Party.GetActorItemList(ItemType).LoseItem(ID, 1);
        }
    }
}
