using System;
using System.Collections.Generic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    /// <summary>
    /// 游戏内显示谈话信息的面板。
    /// 逐字显示。
    /// </summary>
    public class MessagePanel : MonoBehaviour
    {
        //字符间隔（文字速度，可调）
        private const float CharTime = 0f;

        //段落间隔
        private const float ParagraphInterval = 0.5f;

        [SerializeField]
        private TextMeshProUGUI textComponent;

        private List<string> Texts { get; set; } = new();

        private int CharIndex { get; set; }

        private float TimeCount { get; set; }

        /// <summary>
        /// 段落结束
        /// </summary>
        private bool IsParagraphEnd { get; set; }

        private UnityAction Callback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void StartMessage(string text, UnityAction callback)
        {
            Texts.Add(text);
            Callback = callback;
        }

        public void StartMessage(IEnumerable<string> texts, UnityAction callback)
        {
            Texts.AddRange(texts);
            Callback = callback;
        }

        private void EndMessage()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            textComponent.text = string.Empty;
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, NextParagraph),
                new(InputCommand.ButtonDown, ButtonType.Press, NextParagraph),
                new(InputCommand.ButtonLeft, ButtonType.Press, NextParagraph),
                new(InputCommand.ButtonRight, ButtonType.Press, NextParagraph),
                new(InputCommand.ButtonInteract, ButtonType.Down, NextParagraph),
                new(InputCommand.ButtonCancel, ButtonType.Down, NextParagraph),
                new(InputCommand.ButtonMainMenu, ButtonType.Down, NextParagraph),
                new(InputCommand.ButtonSubMenu, ButtonType.Down, NextParagraph),
            };
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(MessagePanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(MessagePanel));
        }

        private void Update()
        {
            if (TimeCount > 0)
                TimeCount -= Time.deltaTime;
            else
                NextChar();
        }

        private void OnDestroy()
        {
            Callback?.Invoke();
        }

        private void NextChar()
        {
            if (IsParagraphEnd)
                return;

            //逐个显示文字
            if (Texts.Count > 0 && CharIndex < Texts[0].Length)
            {
                textComponent.text += Texts[0][CharIndex];
                CharIndex++;
                TimeCount = CharTime;
            }
            //一段文字结束
            else
            {
                IsParagraphEnd = true;
                TimeCount = ParagraphInterval;
            }
        }

        private void NextParagraph()
        {
            //该段文字已完全显示
            if (IsParagraphEnd)
            {
                //准备下一段文字
                textComponent.text = "";
                CharIndex = 0;
                TimeCount = 0;
                IsParagraphEnd = false;
                if (Texts.Count > 1)
                {
                    Texts.RemoveAt(0);
                }
                else
                {
                    EndMessage();
                }
            }
            else
            {
                //文字全部显示
                textComponent.text = Texts[0];
                IsParagraphEnd = true;
            }
        }
    }
}
