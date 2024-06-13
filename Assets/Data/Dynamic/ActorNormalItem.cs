using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dynamic
{
    public class ActorNormalItem : ICommodity
    {
        public ActorNormalItem(int id)
        {
            DataObject = Root.ResourceManager.ActorNormalItem.GetItem(id);
        }

        private Static.ActorNormalItem DataObject { get; set; }

        public int ID => DataObject.id;

        public string Name => DataObject.Name;

        public int Price => DataObject.price;

        public int SellingPrice => (int)(Price * Party.SellingPriceRate);

        public void Buy(int quantity)
        {
            Party.ActorNormalItem.GainItem(ID, quantity);
            Party.LoseGold(Price * quantity);
        }

        public void Sell(int quantity)
        {
            Party.ActorNormalItem.LoseItem(ID, quantity);
            Party.GainGold(SellingPrice * quantity);
        }
    }
}
