using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    [CreateAssetMenu(menuName = "CustomizedData/" + nameof(Encounter))]
    public class Encounter : ScriptableObject
    {
        [Serializable]
        public class EnemyInfo
        {
            [Tooltip("敌人ID")]
            public int id;

            [Tooltip("ID对应的变量为true才可能出现")]
            public int boolID;

            [Tooltip("权重越大出现概率越高")]
            public int weight;
        }

        [Serializable]
        public class EnemyGroup
        {
            [Tooltip("区域ID")]
            public int regionID;

            [Tooltip("最小遇敌步数")]
            public int minStep;

            [Tooltip("最大遇敌步数")]
            public int maxStep;

            [Tooltip("最少数量")]
            public int minQuantity;

            [Tooltip("最多数量")]
            public int maxQuantity;

            [Tooltip("最低级")]
            public int minLevel;

            [Tooltip("最高级")]
            public int maxLevel;

            public EnemyInfo[] enemies;
        }

        public EnemyGroup[] enemyGroups;
    }
}
