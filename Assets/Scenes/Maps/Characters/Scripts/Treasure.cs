using Dynamic;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// 宝箱
    /// </summary>
    public class Treasure : InteractTriggerBase
    {
        [SerializeField]
        [Tooltip("对应宝箱状态，为true时宝箱已开启")]
        private int boolID;

        [SerializeField]
        private Static.CommonItemType itemType;

        [SerializeField]
        private int itemID;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Sprite spriteOpen;

        [SerializeField]
        private Sprite spriteClose;

        protected new void Awake()
        {
            base.Awake();
            if (Party.BoolVariables.GetValue(boolID))
                spriteRenderer.sprite = spriteOpen;
            else
                spriteRenderer.sprite = spriteClose;
        }

        protected override void OnInteract()
        {
            //检查条件
            if (!Party.BoolVariables.GetValue(boolID))
            {
                //打开宝箱
                Party.GetItemList(itemType).GainItem(itemID, 1);
                Party.BoolVariables.SetValue(boolID, true);
                spriteRenderer.sprite = spriteOpen;

                //显示信息
                string itemName = Root.ResourceManager.GetItemName(itemType, itemID);
                PlayerController.WaitCount++;
                UI.UIManager.StartMessage(
                    string.Format(Root.ResourceManager.Term.gainItemFromTreasure, itemName),
                    MessageCallback
                );
            }
        }

        private void MessageCallback()
        {
            PlayerController.WaitCount--;
        }

#if UNITY_EDITOR
        [ContextMenu("刷新精灵渲染器")]
        private void RefreshSpriteRenderer()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteClose;
            }
            else
                Debug.LogWarning("无法刷新，因为没有精灵渲染器");
        }
#endif
    }
}
