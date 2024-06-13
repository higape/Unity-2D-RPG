using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(ActorList))]
    public class ActorList : StaticItemList<Actor> { }
}
