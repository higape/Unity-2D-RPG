using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 角色可使用的物品，包括攻击道具和药品，部分道具可以在菜单使用。
    /// </summary>
    [Serializable]
    public class ActorUsableItem : NameItem
    {
        public enum ItemType
        {
            None = 0,

            [InspectorName("回复道具")]
            RecoverItem = 1,

            [InspectorName("攻击道具")]
            AttackItem = 2,

            [InspectorName("辅助道具")]
            AuxiliaryItem = 3,
        }

        [SerializeField]
        private int skinID;

        [SerializeField]
        private int usageID;

        public bool consumable;
        public UsedOccasion occasion;
        public int price;

        [NonSerialized]
        private ActorWeaponSkin skin;
        public ActorWeaponSkin Skin =>
            skin ??= Root.ResourceManager.ActorWeaponSkin.GetItem(skinID);

        [NonSerialized]
        private WeaponUsage usage;
        public WeaponUsage Usage => usage ??= Root.ResourceManager.WeaponUsage.GetItem(usageID);

        public bool UsedInMenu => (occasion & UsedOccasion.Menu) != UsedOccasion.None;
        public bool UsedInBattle => (occasion & UsedOccasion.Battle) != UsedOccasion.None;
    }
}
