using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedDataList/" + nameof(SkillList))]
    public class SkillList : StaticItemList<Skill> { }
}
