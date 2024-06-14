using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    /// <summary>
    /// 管理地形的类，地形会影响角色通行、移动速度等。
    /// 单例。
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        private const int MaxCacheCount = 5;
        private static Tilemap TerrainLayer { get; set; }
        private static List<(Vector3, string)> TileNameCache { get; set; } = new();

        /// <summary>
        /// 获取 Tile 资源文件的名称。以此名称作为判断地形的依据。
        /// </summary>
        public static string GetTerrainName(Vector3 position)
        {
            if (TerrainLayer != null)
            {
                foreach (var pair in TileNameCache)
                {
                    if (pair.Item1 == position)
                    {
                        return pair.Item2;
                    }
                }

                var tile2 = TerrainLayer.GetTile(Vector3Int.RoundToInt(position));
                string tileName = tile2 == null ? null : tile2.name;
                TileNameCache.Add((position, tileName));
                if (TileNameCache.Count > MaxCacheCount)
                    TileNameCache.RemoveAt(0);
                return tileName;
            }
            return null;
        }

        /// <summary>
        /// 判断能否通过
        /// </summary>
        public static bool CanPass(Vector3 position, RectangleCollider.ColliderType colliderType)
        {
            string terrainName = GetTerrainName(position);
            switch (terrainName)
            {
                case null: // 无地形
                case "Skate": // 滑行
                    return colliderType != RectangleCollider.ColliderType.PlayerShip;
                case "NoPass": // 不可通行
                case "Counter": // 柜台
                    return false;
                case "BoatShipNoPass": // 小舟和船不可通行
                case "Slow": // 沼泽，小舟和船不可通行
                    return colliderType != RectangleCollider.ColliderType.PlayerBoat
                        && colliderType != RectangleCollider.ColliderType.PlayerShip;
                case "ShipPass": // 只有船可以通行
                    return colliderType == RectangleCollider.ColliderType.None
                        || colliderType == RectangleCollider.ColliderType.PlayerShip;
                case "ShipNoPass": // 上船处，船不可通行
                    return colliderType != RectangleCollider.ColliderType.PlayerShip;
                default:
#if UNITY_EDITOR
                    Debug.LogWarning("未识别的地形:" + terrainName);
#endif
                    return true;
            }
        }

        public static bool GetMoveContinue(Vector3 position)
        {
            string terrainName = GetTerrainName(position);
            return terrainName switch
            {
                // 滑行
                "Skate" => true,
                _ => false,
            };
        }

        public static float GetMoveSpeedRate(Vector3 position)
        {
            string terrainName = GetTerrainName(position);
            return terrainName switch
            {
                // 滑行
                "Skate" => 2f,
                // 沼泽
                "Slow" => 0.5f,
                _ => 1f,
            };
        }

        public static float GetInertiaSpeed(Vector3 position)
        {
            string terrainName = GetTerrainName(position);
            return terrainName switch
            {
                // 滑行
                "Skate" => 2f,
                _ => 0,
            };
        }

        public static bool IsCounter(Vector3 position) => GetTerrainName(position) == "Counter";

        [SerializeField]
        private Tilemap terrainLayer;

        private void Awake()
        {
            TerrainLayer = terrainLayer;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(terrainLayer, TerrainLayer))
                TerrainLayer = null;
            TileNameCache.Clear();
        }
    }
}
