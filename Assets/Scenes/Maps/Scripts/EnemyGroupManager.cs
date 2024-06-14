using System;
using System.Collections;
using System.Collections.Generic;
using Root;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    //计算遇敌步数和生成敌人队列
    public class EnemyGroupManager : MonoBehaviour
    {
        private static Tilemap EncounterLayer { get; set; }

        private static string GetRegionName(Vector3 position)
        {
            if (EncounterLayer != null)
            {
                var tile = EncounterLayer.GetTile(Vector3Int.RoundToInt(position));
                if (tile != null)
                    return tile.name;
            }
            return null;
        }

        public static int GetRegionID(Vector3 position)
        {
            string regionName = GetRegionName(position);
            if (int.TryParse(regionName, out int regionID))
            {
                return regionID;
            }
            else
            {
                Debug.LogError(regionName + "无法转换为RegionID");
                return 0;
            }
        }

        public static void CheckEncounter() { }

        [SerializeField]
        private Tilemap encounterLayer;

        private void Awake()
        {
            EncounterLayer = encounterLayer;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(encounterLayer, EncounterLayer))
            {
                EncounterLayer = null;
            }
        }
    }
}
