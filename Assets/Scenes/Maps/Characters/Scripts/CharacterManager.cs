using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class CharacterManager : MonoBehaviour
    {
#if UNITY_EDITOR
        //矫正NPC位置
        [ContextMenu("Round Position At Children")]
        public void RoundPositionAtChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                t.position = new Vector3(
                    Mathf.Round(t.position.x),
                    Mathf.Round(t.position.y),
                    Mathf.Round(t.position.z)
                );
                UnityEditor.EditorUtility.SetDirty(t);
            }
        }
#endif
    }
}
