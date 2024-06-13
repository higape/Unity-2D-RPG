using UnityEngine;

namespace Static
{
    /// <summary>
    /// 保存地图角色行走图的类
    /// </summary>
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(CharacterSkin))]
    public class CharacterSkin : ScriptableObject
    {
        public Sprite idleLeft;
        public Sprite idleRight;
        public Sprite idleUp;
        public Sprite idleDown;
        public Sprite walkingLeft;
        public Sprite walkingRight;
        public Sprite walkingUp;
        public Sprite walkingDown;

#if UNITY_EDITOR
        public Texture2D texture;

        //引用特定格式的 texture 的 Sprite
        [ContextMenu("SetupSprites")]
        public void SetupSprites()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(texture);
            object[] objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
            if (objs.Length == 13)
            {
                idleLeft = objs[5] as Sprite;
                idleRight = objs[8] as Sprite;
                idleUp = objs[10] as Sprite;
                idleDown = objs[1] as Sprite;
                walkingLeft = objs[6] as Sprite;
                walkingRight = objs[9] as Sprite;
                walkingUp = objs[12] as Sprite;
                walkingDown = objs[3] as Sprite;
            }
        }
#endif
    }
}
