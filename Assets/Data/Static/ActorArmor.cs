using System;
using System.Collections.Generic;
using System.Linq;
using Root;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 角色防具
    /// </summary>
    [Serializable]
    public class ActorArmor : NameItem
    {
        public int hp;
        public int atk;
        public int def;
        public int agi;
        public int hit;
        public int eva;
        public ElementGroup elementGroup;
        public int price;

        [Tooltip("可装备角色的ID")]
        public int[] equipable;

        public TraitData[] traits;

        public bool CanEquip(Dynamic.Actor actor) =>
            equipable.Length == 0 || equipable.Contains(actor.ID);

        public Actor[] GetEquipableActors()
        {
            if (equipable.Length == 0)
            {
                return ResourceManager.Actor.ItemList;
            }
            else
            {
                var actors = new Actor[equipable.Length];
                for (int i = 0; i < equipable.Length; i++)
                {
                    actors[i] = ResourceManager.Actor.GetItem(equipable[i]);
                }
                return actors;
            }
        }
    }
}
