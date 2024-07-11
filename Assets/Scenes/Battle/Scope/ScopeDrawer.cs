using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class ScopeDrawer : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer scopeRect;

        [SerializeField]
        private SpriteRenderer scopeCircle;

        [SerializeField]
        private SpriteMask maskRect0;

        [SerializeField]
        private SpriteMask maskRect1;

        [SerializeField]
        private SpriteMask maskCircle;

        private static ScopeDrawer Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void DrawSector(Vector2 point1, Vector2 point2, float sectorAngle)
        {
            // 计算直线斜率
            // float slope = (point2.y - point1.y) / (point2.x - point1.x);

            // 计算斜率的反正切值
            // float angleRad = Mathf.Atan(slope);

            // 转换为角度，此角度为两点之间的直线与x轴的夹角的角度
            // float angleDeg = angleRad * Mathf.Rad2Deg;

            // 合并以上计算
            float angleDeg =
                Mathf.Rad2Deg * Mathf.Atan((point2.y - point1.y) / (point2.x - point1.x));

            //设置两个矩形遮罩的角度
            maskRect0.transform.localEulerAngles = new Vector3(0f, 0f, angleDeg - sectorAngle / 2);
            maskRect1.transform.localEulerAngles = new Vector3(
                0f,
                0f,
                180f + angleDeg + sectorAngle / 2
            );

            scopeRect.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            scopeRect.enabled = true;

            this.transform.position = point1;

            //设置其它参数
            this.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            this.transform.localScale = new Vector3(100f, 100f, 1f);
            scopeCircle.enabled = false;
        }

        private void DrawRay(Vector2 point1, Vector2 point2, float rayWidth)
        {
            // DrawSector方法里有解释
            float angleDeg =
                Mathf.Rad2Deg * Mathf.Atan((point2.y - point1.y) / (point2.x - point1.x));

            //设置矩形的角度
            this.transform.localEulerAngles = new Vector3(0f, 0f, angleDeg);

            //设置矩形的宽度
            this.transform.localScale = new Vector3(
                100f,
                rayWidth / scopeRect.sprite.rect.height * (scopeRect.sprite.pixelsPerUnit / 32f),
                1f
            );

            scopeRect.maskInteraction = SpriteMaskInteraction.None;
            scopeRect.enabled = true;

            this.transform.position = point1;

            //设置其它参数
            scopeCircle.enabled = false;
        }

        public static void Clear()
        {
            if (Instance != null)
            {
                Instance.scopeRect.enabled = false;
                Instance.scopeCircle.enabled = false;
            }
        }

        public static void DrawSmallSector(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawSector(point1, point2, BattleManager.SmallSectorAngle);
        }

        public static void DrawBigSector(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawSector(point1, point2, BattleManager.BigSectorAngle);
        }

        public static void DrawSmallRay(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawRay(point1, point2, BattleManager.SmallRayWidth);
        }

        public static void DrawBigRay(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawRay(point1, point2, BattleManager.BigRayWidth);
        }
    }
}
