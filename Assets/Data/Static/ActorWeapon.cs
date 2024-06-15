using System;
using System.Collections.Generic;
using System.Linq;
using Root;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 人类武器
    /// </summary>
    [Serializable]
    public class ActorWeapon : NameItem
    {
        [SerializeField]
        private int skinID;

        [SerializeField]
        private int[] usageIDs;

        public int atk;

        public int price;

        [Tooltip("可装备角色的ID,留空表示所有人可装备")]
        public int[] equipable;

        [NonSerialized]
        private ActorWeaponSkin skin;

        public ActorWeaponSkin Skin =>
            skin ??= Root.ResourceManager.ActorWeaponSkin.GetItem(skinID);

        [NonSerialized]
        private List<WeaponUsage> usageList;

        public List<WeaponUsage> UsageList
        {
            get
            {
                if (usageList == null)
                {
                    usageList = new(usageIDs.Length);
                    foreach (var id in usageIDs)
                    {
                        usageList.Add(Root.ResourceManager.WeaponUsage.GetItem(id));
                    }
                }
                return usageList;
            }
        }

        public bool CanEquip(int actorID) => equipable.Length == 0 || equipable.Contains(actorID);

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
