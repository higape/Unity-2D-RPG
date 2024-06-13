using System.Collections.Generic;
using System.Linq;
using Root;

namespace Dynamic
{
    public class ActorArmor : ITrait, ICommodity
    {
        public ActorArmor(int slotIndex, int id)
        {
            var list = ResourceManager.GetActorArmorList(slotIndex);

            SlotIndex = slotIndex;
            DataObject = list.GetItem(id);
            UpdateSkillList();
        }

        private int SlotIndex { get; set; }
        private Static.ActorArmor DataObject { get; set; }
        public int ID => DataObject.id;
        public string Name => DataObject.Name;
        public int Hp => DataObject.hp;
        public int Atk => DataObject.atk;
        public int Def => DataObject.def;
        public int Agi => DataObject.agi;
        public int Hit => DataObject.hit;
        public int Eva => DataObject.eva;
        public Static.ElementGroup ElementGroup => DataObject.elementGroup;
        public IEnumerable<Static.TraitData> Traits => DataObject.TraitList;
        public List<Skill> Skills { get; private set; }
        public int Price => DataObject.price;
        public int SellingPrice => (int)(Price * Party.SellingPriceRate);

        private void UpdateSkillList()
        {
            Skills = new List<Skill>();
            foreach (var item in Traits)
            {
                if (
                    item.Effect.type0 == Static.BattleEffect.EffectType.AdditionType
                    && item.Effect.type1 == (int)Static.BattleEffect.ActorType.Skill
                )
                    Skills.Add(new Skill(item.effectValue, false, () => true));
            }
        }

        public void Buy(int quantity)
        {
            Party.GainActorArmor(SlotIndex, ID, quantity);
            Party.LoseGold(Price * quantity);
        }

        public void Sell(int quantity)
        {
            Party.LoseActorArmor(SlotIndex, ID, quantity);
            Party.GainGold(SellingPrice * quantity);
        }
    }
}
