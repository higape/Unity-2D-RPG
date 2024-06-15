using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dynamic
{
    public class ActorWeapon : Weapon, ICommodity
    {
        public ActorWeapon(int id)
        {
            DataObject = Root.ResourceManager.ActorWeapon.GetItem(id);
        }

        private Static.ActorWeapon DataObject { get; set; }

        public int ID => DataObject.id;

        public string Name => DataObject.Name;

        public Static.ActorWeaponSkin Skin => DataObject.Skin;

        public override int Attack => DataObject.atk;

        protected override Vector3 FirePosition => Skin.firePosition + Owner.DisplayObject.Position;

        public int Price => DataObject.price;

        public int SellingPrice => (int)(Price * Party.SellingPriceRate);

        public void Buy(int quantity)
        {
            Party.ActorWeapon.GainItem(ID, quantity);
            Party.LoseGold(Price * quantity);
        }

        public void Sell(int quantity)
        {
            Party.ActorWeapon.LoseItem(ID, quantity);
            Party.GainGold(SellingPrice * quantity);
        }

        protected override void StartBullet()
        {
            if (Owner is Actor actor && Skin.motion != Static.Actor.Motion.Idle)
                actor.ShowMotion(Skin, ProcessBullet);
            else
                ProcessBullet();
        }

        public override void CostAndCool()
        {
            CurrentWaitTime += CurrentUsage.waitTime;
        }

        public Static.WeaponUsage GetUsage(int usageIndex) => DataObject.UsageList[usageIndex];

        public List<Actor> GetEquipableActorList()
        {
            if (DataObject.equipable.Length == 0)
            {
                return Party.PartyActorList;
            }
            else
            {
                List<Actor> actors = new();
                foreach (int actorID in DataObject.equipable)
                {
                    actors.Add(Party.GetPartyActorByID(actorID));
                }
                return actors;
            }
        }
    }
}
