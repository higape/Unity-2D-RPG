using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(FixedEncounter))]
    public class FixedEncounter : ScriptableObject
    {
        [Serializable]
        public class EnemyInfo
        {
            [Tooltip("敌人ID")]
            public int id;

            [Tooltip("敌人等级")]
            public int level;

            public Vector2 position;
        }

        public EnemyInfo[] enemies;

        public List<(int, int, Vector2)> MakeBattleData()
        {
            List<(int, int, Vector2)> data = new();
            foreach (EnemyInfo enemy in enemies)
            {
                data.Add((enemy.id, enemy.level, enemy.position));
            }
            return data;
        }
    }
}
