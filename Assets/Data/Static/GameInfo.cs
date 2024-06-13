using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(GameInfo))]
    public class GameInfo : ScriptableObject
    {
        public string gameName;
        public string startMapName;
        public Vector2 startPosition;
    }
}
