using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Battle
{
    public class PopDigit : MonoBehaviour
    {
        /// <summary>
        /// 弹出数字的样式
        /// </summary>
        public enum DigitStyle
        {
            Damage = 0,
            Recover = 1
        }

        [SerializeField]
        private TextMeshPro digitComponent;

        public void Setup(int digit, DigitStyle digitStyle)
        {
            digitComponent.text = digit.ToString();
            switch (digitStyle)
            {
                case DigitStyle.Damage:
                    digitComponent.color = new Color(1, 1, 1, 1);
                    break;
                case DigitStyle.Recover:
                    digitComponent.color = new Color(0, 1, 0, 1);
                    break;
            }
            StartCoroutine(PopUp());
        }

        /// <summary>
        /// 弹出结束后销毁对象
        /// </summary>
        private IEnumerator PopUp()
        {
            for (int i = 0; i < 30; i++)
            {
                transform.Translate(new Vector3(0, 0.05f, 0));
                yield return new WaitForSeconds(.05f);
            }
            Destroy(gameObject);
        }
    }
}
