using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 添加到 ListBox 以显示页码
    /// </summary>
    public class PageNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textNumber;

        public void Refresh(int currentPage, int maxPage)
        {
            textNumber.text = currentPage.ToString() + '/' + maxPage.ToString();
        }
    }
}
