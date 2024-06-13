using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(ShopList))]
    public class ShopList : StaticItemList<Shop> { }
}
