using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InputTip : MonoBehaviour
    {
        public TextMeshProUGUI text0;
        public TextMeshProUGUI text1;
        public TextMeshProUGUI text2;
        public TextMeshProUGUI text3;
        public TextMeshProUGUI text4;
        public TextMeshProUGUI text5;

        private void Start()
        {
            text0.text = "移动";
            text1.text = "确定";
            text2.text = "取消";
            text3.text = "菜单";
            text4.text = "未定义";
            text5.text = "翻页";
        }
    }
}
