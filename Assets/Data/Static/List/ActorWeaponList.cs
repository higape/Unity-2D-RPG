using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(ActorWeaponList))]
    public class ActorWeaponList : StaticItemList<ActorWeapon> { }
}
