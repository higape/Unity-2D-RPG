using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Static
{
    [Serializable]
    public class Shop : DescriptionItem
    {
        //商品分类与资源的存储有关
        [Serializable]
        public class ShopItem
        {
            public CommonItemType type;
            public int id;
        }

        public ShopItem[] items;
    }
}
