using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    public class EnemyAction : Weapon, IDurability
    {
        public EnemyAction(Static.Enemy.EnemyAction dataObject)
        {
            DataObject = dataObject;
        }

        private Static.Enemy.EnemyAction DataObject { get; set; }
        public override int Attack => 0;
        public Static.WeaponUsage Usage => DataObject.Usage;
        protected override Vector3 FirePosition =>
            DataObject.firePosition + Owner.DisplayObject.Position;
        public bool CanUse => !IsCooling && !(this as IDurability).IsZeroDurability;
        public int Damage { get; set; }
        public int MaxDurability => DataObject.durability;

        protected override void StartBullet()
        {
            ProcessBullet();
        }

        public override bool CostAndCool()
        {
            if (IsCooling)
                return false;
            CurrentCoolingTime += CurrentUsage.coolingTime;
            return true;
        }
    }
}
