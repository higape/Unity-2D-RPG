using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI occasionLabel;

        [SerializeField]
        private TextMeshProUGUI occasionContent;

        [SerializeField]
        private UsageStatistic usageStatistic;

        [SerializeField]
        private SkillSelectionStatistic selectionStatistic;

        private void Awake()
        {
            occasionLabel.text = ResourceManager.Term.usedOccasion;
        }

        public void EffectListPageUp() => usageStatistic.EffectListPageUp();

        public void EffectListPageDown() => usageStatistic.EffectListPageDown();

        public void Refresh(Dynamic.Skill item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            if (item.UsedInMenu)
            {
                occasionContent.text = ResourceManager.Term.menu;
                if (item.UsedInBattle)
                {
                    occasionContent.text += '/' + ResourceManager.Term.battle;
                }
            }
            else if (item.UsedInBattle)
            {
                occasionContent.text = ResourceManager.Term.battle;
            }
            else
            {
                occasionContent.text = "";
            }

            switch (item.SkillType)
            {
                case Static.Skill.SkillType.Passivity:
                    usageStatistic.gameObject.SetActive(false);
                    selectionStatistic.gameObject.SetActive(false);
                    break;
                case Static.Skill.SkillType.Usage:
                    usageStatistic.Refresh(item.Usage);
                    usageStatistic.gameObject.SetActive(true);
                    selectionStatistic.gameObject.SetActive(false);
                    break;
                case Static.Skill.SkillType.SelectActorWeapon:
                case Static.Skill.SkillType.SelectActorItem:
                    selectionStatistic.Refresh(item);
                    usageStatistic.gameObject.SetActive(false);
                    selectionStatistic.gameObject.SetActive(true);
                    break;
                default:
                    usageStatistic.gameObject.SetActive(false);
                    selectionStatistic.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
