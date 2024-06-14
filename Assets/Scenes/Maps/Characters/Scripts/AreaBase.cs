using UnityEngine;

namespace Map
{
    /// <summary>
    /// 范围组件的基类
    /// </summary>
    [DisallowMultipleComponent]
    public class AreaBase : MonoBehaviour
    {
        [SerializeField]
        private int width = 1;

        [SerializeField]
        private int height = 1;

        public int Width
        {
            get { return width; }
            set
            {
                if (value > 0)
                {
                    width = value;
                }
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (value > 0)
                {
                    height = value;
                }
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Rect CurrentRect => new(Position.x, Position.y, width, height);

        /// <summary>
        /// 检查点接触。
        /// </summary>
        public bool Overlap(Vector3 otherPosition)
        {
            return CurrentRect.Contains(otherPosition);
        }

        /// <summary>
        /// 检查矩形接触。
        /// </summary>
        public bool Overlap(Rect otherRect)
        {
            return CurrentRect.Overlaps(otherRect);
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                new Vector3(
                    position.x + width / 2f - 0.5f,
                    position.y + height / 2f - 0.5f,
                    position.z
                ),
                new Vector3(width, height, 1)
            );
        }
    }
}
