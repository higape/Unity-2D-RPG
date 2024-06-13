using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 有名称的项
    /// </summary>
    public abstract class NameItem : IDItem
    {
        [SerializeField]
        private string name;
        public string Name => name;
    }
}
