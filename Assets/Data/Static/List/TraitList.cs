using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(TraitList))]
    public class TraitList : StaticItemList<Trait> { }
}
