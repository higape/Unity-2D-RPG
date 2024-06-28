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

        private void Awake()
        {
            occasionLabel.text = ResourceManager.Term.usedOccasion;
        }

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
        }
    }
}
