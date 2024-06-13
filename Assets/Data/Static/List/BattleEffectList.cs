using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(BattleEffectList))]
    public class BattleEffectList : StaticItemList<BattleEffect> { }
}
