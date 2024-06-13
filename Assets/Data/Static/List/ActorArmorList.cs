using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(ActorArmorList))]
    public class ActorArmorList : StaticItemList<ActorArmor> { }
}
