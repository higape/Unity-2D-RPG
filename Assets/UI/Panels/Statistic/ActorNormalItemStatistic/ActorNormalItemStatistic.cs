using System.Collections;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ActorNormalItemStatistic : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI sellingPriceLabel;

        [SerializeField]
        private TextMeshProUGUI sellingPriceContent;

        private void Awake()
        {
            sellingPriceLabel.text = ResourceManager.Term.sellingPrice;
        }

        public void Refresh(Dynamic.ActorNormalItem item)
        {
            if (item == null)
            {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;
            nameContent.text = item.Name;
            sellingPriceContent.text = item.SellingPrice.ToString();
        }
    }
}
