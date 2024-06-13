using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dynamic
{
    /// <summary>
    /// 商品
    /// </summary>
    public interface ICommodity
    {
        string Name { get; }
        int Price { get; }
        int SellingPrice { get; }
        void Buy(int quantity);
        void Sell(int quantity);
    }
}
