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
        private SpriteMask maskRect0;

        [SerializeField]
        private SpriteMask maskRect1;

        [SerializeField]
        private SpriteMask maskCircle;

        private static ScopeDrawer Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            scopeRect.color = new Color(1f, 0f, 0f, 0.25f);
            scopeRect.transform.localScale = new Vector3(40f, 40f, 40f);
        }

        private void DrawSector(Vector2 point1, Vector2 point2, float sectorAngle)
        {
            scopeRect.enabled = true;
            scopeRect.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            maskRect0.transform.position = point1;
            maskRect1.transform.position = point1;

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

            maskRect0.transform.localScale = new Vector3(100f, 100f, 1f);
            maskRect1.transform.localScale = new Vector3(100f, 100f, 1f);

            //设置其它参数
            maskRect0.enabled = true;
            maskRect1.enabled = true;
            maskCircle.enabled = false;
        }

        private void DrawRay(Vector2 point1, Vector2 point2, float rayWidth)
        {
            scopeRect.enabled = true;
            scopeRect.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            maskRect0.transform.position = point1;

            // DrawSector方法里有解释
            float angleDeg =
                Mathf.Rad2Deg * Mathf.Atan((point2.y - point1.y) / (point2.x - point1.x));

            //设置矩形的角度
            maskRect0.transform.localEulerAngles = new Vector3(0f, 0f, angleDeg + 90f);

            //设置矩形的宽度
            maskRect0.transform.localScale = new Vector3(
                rayWidth / maskRect0.sprite.rect.height * (maskRect0.sprite.pixelsPerUnit / 32f),
                100f,
                1f
            );

            maskRect0.enabled = true;
            maskRect1.enabled = false;
            maskCircle.enabled = false;
        }

        private void DrawCircle(Vector2 point1, Vector2 point2, float circleRadius)
        {
            scopeRect.enabled = true;
            scopeRect.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            maskCircle.transform.position = point2;

            float scale =
                circleRadius
                * 2f
                / maskCircle.sprite.rect.width
                * (maskCircle.sprite.pixelsPerUnit / 32f);
            maskCircle.transform.localScale = new Vector3(scale, scale, 1f);

            //设置其它参数
            maskRect0.enabled = false;
            maskRect1.enabled = false;
            maskCircle.enabled = true;
        }

        public static void Clear()
        {
            if (Instance != null)
            {
                Instance.scopeRect.enabled = false;
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

        public static void DrawSmallCircle(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawCircle(point1, point2, BattleManager.SmallCircleRadius);
        }

        public static void DrawBigCircle(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawCircle(point1, point2, BattleManager.BigCircleRadius);
        }
    }
}
