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
            float slope = (point2.y - point1.y) / (point2.x - point1.x);

            // 计算斜率的反正切值
            float angleRad = Mathf.Atan(slope);

            // 转换为角度
            float angleDeg = angleRad * Mathf.Rad2Deg;

            //设置两个矩形mask的角度
            maskRect0.transform.eulerAngles = new Vector3(0f, 0f, angleDeg - sectorAngle / 2);
            maskRect1.transform.eulerAngles = new Vector3(
                0f,
                0f,
                180f + angleDeg + sectorAngle / 2
            );

            this.transform.position = point1;
            scopeRect.enabled = true;
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
                Instance.DrawSector(point1, point2, 16f);
        }

        public static void DrawBigSector(Vector2 point1, Vector2 point2)
        {
            if (Instance != null)
                Instance.DrawSector(point1, point2, 24f);
        }
    }
}
