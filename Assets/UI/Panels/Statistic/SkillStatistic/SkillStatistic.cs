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
        private GameObject traitPiece;

        [SerializeField]
        private TraitStatistic traitStat;

        [SerializeField]
        private GameObject usagePiece;

        [SerializeField]
        private UsageStatistic usageStat;

        [SerializeField]
        private GameObject selectionPiece;

        [SerializeField]
        private SkillSelectionStatistic selectionStat;

        private void Awake()
        {
            occasionLabel.text = ResourceManager.Term.usedOccasion;
        }

        public void EffectListPageUp() => usageStat.EffectListPageUp();

        public void EffectListPageDown() => usageStat.EffectListPageDown();

        public void Refresh(Dynamic.Skill item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            occasionContent.text = UIManager.CreateOccasionText(item.Occasion);

            switch (item.SkillType)
            {
                case Static.Skill.SkillType.Passivity:
                    traitStat.Refresh(item.Traits);
                    traitPiece.SetActive(true);
                    usagePiece.SetActive(false);
                    selectionPiece.SetActive(false);
                    break;
                case Static.Skill.SkillType.Usage:
                    usageStat.Refresh(item.Usage);
                    traitPiece.SetActive(false);
                    usagePiece.SetActive(true);
                    selectionPiece.SetActive(false);
                    break;
                case Static.Skill.SkillType.SelectActorWeapon:
                case Static.Skill.SkillType.SelectActorItem:
                    selectionStat.Refresh(item);
                    traitPiece.SetActive(false);
                    usagePiece.SetActive(false);
                    selectionPiece.SetActive(true);
                    break;
                default:
                    traitPiece.SetActive(false);
                    usagePiece.SetActive(false);
                    selectionPiece.SetActive(false);
                    break;
            }
        }
    }
}
