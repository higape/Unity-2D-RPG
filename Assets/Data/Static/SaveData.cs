using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    public class SaveData
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;

        public int gold;
        public int[] boolKeys;
        public bool[] boolValues;
        public int[] intKeys;
        public int[] intValues;
        public int[] actorWeapons;
        public int[] actorHeadArmors;
        public int[] actorBodyArmors;
        public int[] actorHandArmors;
        public int[] actorFootArmors;
        public int[] actorOrnamentArmors;
        public int[] actorRecoverItems;
        public int[] actorAttackItems;
        public int[] actorAuxiliaryItems;
        public int[] actorNormalItems;
        public Actor.SaveData[] actors;
        public int[] partyActorIDs;
    }
}
