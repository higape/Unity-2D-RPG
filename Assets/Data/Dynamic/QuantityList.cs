using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dynamic
{
    /// <summary>
    /// 存储物品及其数量的列表类
    /// </summary>
    public class QuantityList : List<QuantityList.ListItem>
    {
        [Serializable]
        public class ListItem
        {
            public int id;
            public int quantity;

            public ListItem(int id, int quantity)
            {
                this.id = id;
                this.quantity = quantity;
            }
        }

        public QuantityList() { }

        public QuantityList(int[] data)
        {
            int i = 0;
            while (i < data.Length)
            {
                Add(new(data[i], data[i + 1]));
                i += 2;
            }
        }

        public int GetQuantity(int id)
        {
            foreach (var item in this)
            {
                if (item.id == id)
                {
                    return item.quantity;
                }
            }

            return 0;
        }

        public void GainItem(int id, int quantity)
        {
            foreach (var item in this)
            {
                if (item.id == id)
                {
                    item.quantity += quantity;
                    return;
                }
            }

            Add(new(id, quantity));
        }

        public void LoseItem(int id, int quantity)
        {
            foreach (var item in this)
            {
                if (item.id == id)
                {
                    item.quantity -= quantity;
                    if (item.quantity <= 0)
                    {
                        Remove(item);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 有足够数量时减少道具数量
        /// </summary>
        /// <returns>有无扣减</returns>
        public bool TryLoseItem(int id, int quantity)
        {
            foreach (var item in this)
            {
                if (item.id == id)
                {
                    if (item.quantity > quantity)
                    {
                        item.quantity -= quantity;
                        return true;
                    }
                    else if (item.quantity == quantity)
                    {
                        Remove(item);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        // public (int[], int[]) ToSaveData()
        // {
        //     int[] idArray = new int[this.Count];
        //     int[] quantityArray = new int[this.Count];
        //     for (int i = 0; i < this.Count; i++)
        //     {
        //         idArray[i] = this[i].id;
        //         quantityArray[i] = this[i].quantity;
        //     }
        //     return (idArray, quantityArray);
        // }

        public int[] ToSaveData()
        {
            int[] data = new int[this.Count * 2];
            int j = 0;
            foreach (var v in this)
            {
                data[j++] = v.id;
                data[j++] = v.quantity;
            }
            return data;
        }
    }
}
