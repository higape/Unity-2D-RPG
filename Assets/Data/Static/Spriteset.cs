using UnityEngine;

namespace Static
{
    /// <summary>
    /// 包含游戏内系统用语
    /// </summary>
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(Spriteset))]
    public class Spriteset : ScriptableObject
    {
        public Sprite[] elementSprite;

        public Sprite[] equipmentSprite;

        public Sprite GetElementSprite(ElementType type) => elementSprite[(int)type];

        public Sprite GetActorWeaponSprite() => equipmentSprite[0];

        public Sprite GetActorArmorSprite(int slotIndex) => equipmentSprite[slotIndex + 1];
    }
}
