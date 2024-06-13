using UnityEngine;

namespace Static
{
    /// <summary>
    /// 储存静态数据。
    /// 提供查找数据的API。
    /// </summary>
    public abstract class StaticItemList<T> : ScriptableObject
        where T : IDItem
    {
        [SerializeField]
        private T[] itemList;

        public T[] ItemList => itemList;

        public T GetItem(int id)
        {
            if (id == 0)
            {
                Debug.LogWarning("ItemID为0");
                return null;
            }

            foreach (var item in itemList)
            {
                if (item.id == id)
                {
                    return item;
                }
            }

            Debug.LogWarning("不存在的ID:" + id);
            return null;
        }
    }
}
