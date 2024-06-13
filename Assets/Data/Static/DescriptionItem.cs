using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 附带说明文本的类
    /// </summary>
    public abstract class DescriptionItem : NameItem
    {
        [SerializeField]
        private string description;
        public string Description => description;
    }
}
