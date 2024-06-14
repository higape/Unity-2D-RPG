using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [Serializable]
    public class MoveCommandSet
    {
        public CommandSet.Command[] commands;
        public bool repeat;

        /// <summary>
        /// 当一个指令无法执行时，跳过该指令，继续执行之后的指令
        /// </summary>
        public bool skippable;
    }
}
