using System;
using UnityEngine;

namespace Static
{
    /// <summary>
    /// 包含武器外观以及与武器外观相符的角色动作
    /// </summary>
    [Serializable]
    public class ActorWeaponSkin : NameItem
    {
        public Sprite sprite;
        public Vector3 firePosition;
        public Actor.Motion motion;
    }
}
