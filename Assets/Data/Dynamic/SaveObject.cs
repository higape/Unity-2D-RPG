using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dynamic
{
    /// <summary>
    /// 存档，保存游玩数据的类
    /// </summary>
    public class SaveObject
    {
        public class Actor
        {
            public int id;
            public string name;
            public int exp;
            public int hp;
            public int[] equipments;
        }

        public List<Actor> humans = new();

        public void Make()
        {
            humans.Clear();
        }
    }
}
