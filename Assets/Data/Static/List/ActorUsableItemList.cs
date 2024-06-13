using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(ActorUsableItemList))]
    public class ActorUsableItemList : StaticItemList<ActorUsableItem> { }
}
