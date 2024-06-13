using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(WeaponUsageList))]
    public class WeaponUsageList : StaticItemList<WeaponUsage> { }
}
