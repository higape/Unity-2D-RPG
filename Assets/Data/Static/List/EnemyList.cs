using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(EnemyList))]
    public class EnemyList : StaticItemList<Enemy> { }
}
