using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 通过脚本添加内容的列表。
    /// </summary>
    public class ListBox : MonoBehaviour
    {
        /// <summary>
        /// 当无法同时显示所有列表项时的处理模式。
        /// </summary>
        private enum WorkMode
        {
            Scroll = 0,
            Page = 1,
        }

        private enum CursorMode
        {
            /// <summary>
            /// 用于单列或单行指令列表。
            /// </summary>
            [InspectorName("囚笼")]
            [Tooltip("如果空间足够，光标不会跨越边界。")]
            Cage = 0,

            /// <summary>
            /// 可用于指令矩阵。
            /// </summary>
            [InspectorName("循环")]
            [Tooltip("光标可以跨越边界到达另一侧。")]
            Loop = 1,

            /// <summary>
            /// 可用于物品矩阵。
            /// </summary>
            [InspectorName("顺序")]
            [Tooltip("光标可以跨越边界换行。")]
            Order = 2,
        }

        [SerializeField]
        private WorkMode workMode = WorkMode.Scroll;

        [SerializeField]
        private CursorMode cursorMode = CursorMode.Cage;

        [SerializeField]
        [Tooltip("子对象预制件,必须挂有ListBoxItem")]
        private GameObject listItemPrefab;

        [SerializeField]
        [Tooltip("滚动条,可选")]
        private Scrollbar scrollbar;

        [SerializeField]
        [Tooltip("页码,可选")]
        private PageNumber pageNumber;

        private readonly UnityEvent<object, int> selectedItemChanged = new();

        private int componentIndex = -1;
        private int selectedIndex = -1;
        private int topItemIndex = 0;

        /// <summary>
        /// 始终不能为null
        /// </summary>
        private IList itemsSource = new List<object>();

        /// <summary>
        /// 包含视觉信息的对象列表
        /// </summary>
        public IList ItemsSource
        {
            get => itemsSource;
            private set
            {
                if (value != null)
                    itemsSource = value;
                else
                    itemsSource = new List<object>();

                if (SelectedIndex == -1)
                    SelectFirst();
                else
                    Reselect();

                Refresh();
            }
        }

        private UnityAction<ListBoxItem, object> RefreshAction { get; set; }

        public int SelectedIndex
        {
            get => selectedIndex;
            private set
            {
                //空列表
                if (ItemsSource.Count == 0)
                {
                    selectedIndex = -1;
                    TopItemIndex = 0;
                    ComponentIndex = -1;
                }
                else
                {
                    //限制在安全范围
                    selectedIndex = Mathf.Clamp(value, 0, ItemsSource.Count - 1);
                    //更新页面或滚动
                    UpdateTopItemIndex();
                    //更新选中
                    ComponentIndex = selectedIndex - TopItemIndex;
                }
                selectedItemChanged.Invoke(SelectedItem, SelectedIndex);
            }
        }

        public object SelectedItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < ItemsSource.Count)
                    return ItemsSource[SelectedIndex];
                else
                    return null;
            }
        }

        /// <summary>
        /// 实际列数量
        /// </summary>
        private int Cols { get; set; }

        /// <summary>
        /// 实际列数量
        /// </summary>
        private int Rows { get; set; }

        /// <summary>
        /// 子对象的视觉控制组件
        /// </summary>
        private List<ListBoxItem> Components { get; set; } = new();

        /// <summary>
        /// 当前视觉组件索引。
        /// 被其它属性依赖的重要属性。
        /// </summary>
        private int ComponentIndex
        {
            get => componentIndex;
            set
            {
                //消除原选项的选中状态
                if (SelectedComponent != null)
                    SelectedComponent.Selected = false;
                componentIndex = value;
                //赋予新选项选中状态
                if (SelectedComponent != null)
                    SelectedComponent.Selected = true;
            }
        }

        /// <summary>
        /// 选中的项
        /// </summary>
        private ListBoxItem SelectedComponent
        {
            get
            {
                if (ComponentIndex >= 0 && ComponentIndex < Components.Count)
                    return Components[ComponentIndex];
                else
                    return null;
            }
        }

        /// <summary>
        /// 画面顶部第一个源的索引
        /// </summary>
        private int TopItemIndex
        {
            get => topItemIndex;
            set
            {
                if (topItemIndex != value)
                {
                    topItemIndex = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        public void Initialize(int cols, int rows, UnityAction<ListBoxItem, object> action)
        {
            if (cols > 0 && rows > 0)
            {
                Cols = cols;
                Rows = rows;
            }
            else
            {
                Debug.LogError("给定的行和列无法构成矩阵");
            }

            CreateChildren();
            RefreshAction = action;
        }

        /// <summary>
        /// 初始化列表并设置源
        /// </summary>
        public void Initialize(
            int cols,
            int rows,
            UnityAction<ListBoxItem, object> action,
            IList source
        )
        {
            if (cols > 0 && rows > 0)
            {
                Cols = cols;
                Rows = rows;
            }
            /* else if (cols > 0 && rows <= 0)
            {
                Cols = cols;
                Rows = Mathf.CeilToInt((float)source.Count / cols);
            }
            else if (cols <= 0 && rows > 0)
            {
                Cols = Mathf.CeilToInt((float)source.Count / rows);
                Rows = rows;
            } */
            else
            {
                Debug.LogError("给定的行和列无法构成矩阵");
            }

            CreateChildren();
            RefreshAction = action;
            ItemsSource = source;
        }

        public void SetSource(IList source, UnityAction<ListBoxItem, object> action)
        {
            if (Cols != 0 && Rows != 0)
            {
                RefreshAction = action;
                ItemsSource = source;
            }
        }

        public void SetSource(IList source)
        {
            if (Cols != 0 && Rows != 0)
            {
                ItemsSource = source;
            }
        }

        public void SelectUp()
        {
            //在上边缘
            if (SelectedIndex < Cols)
            {
                switch (cursorMode)
                {
                    case CursorMode.Cage:
                        //不移动
                        break;
                    case CursorMode.Loop:
                        SelectedIndex = SelectedIndex - Cols + ItemsSource.Count;
                        break;
                    case CursorMode.Order:
                        SelectedIndex = ItemsSource.Count - 1;
                        break;
                }
            }
            else
                SelectedIndex -= Cols;
        }

        public void SelectDown()
        {
            //在下边缘
            if (SelectedIndex >= ItemsSource.Count - Cols)
            {
                switch (cursorMode)
                {
                    case CursorMode.Cage:
                        //不移动
                        break;
                    case CursorMode.Loop:
                        SelectedIndex = SelectedIndex + Cols - ItemsSource.Count;
                        break;
                    case CursorMode.Order:
                        SelectedIndex = 0;
                        break;
                }
            }
            else
                SelectedIndex += Cols;
        }

        public void SelectLeft()
        {
            //在左边缘
            if (SelectedIndex % Cols == 0)
            {
                switch (cursorMode)
                {
                    case CursorMode.Cage:
                        //不移动
                        break;
                    case CursorMode.Loop:
                        SelectedIndex = SelectedIndex + Cols - 1;
                        break;
                    case CursorMode.Order:
                        if (SelectedIndex == 0)
                            SelectedIndex = ItemsSource.Count - 1;
                        else
                            SelectedIndex -= 1;
                        break;
                }
            }
            else
                SelectedIndex -= 1;
        }

        public void SelectRight()
        {
            //在右边缘
            if (SelectedIndex % Cols == Cols - 1)
            {
                switch (cursorMode)
                {
                    case CursorMode.Cage:
                        //不移动
                        break;
                    case CursorMode.Loop:
                        SelectedIndex = SelectedIndex - Cols + 1;
                        break;
                    case CursorMode.Order:
                        if (SelectedIndex == ItemsSource.Count - 1)
                            SelectedIndex = 0;
                        else
                            SelectedIndex += 1;
                        break;
                }
            }
            else
                SelectedIndex += 1;
        }

        public void PageUp()
        {
            SelectedIndex -= Components.Count;
        }

        public void PageDown()
        {
            SelectedIndex += Components.Count;
        }

        public void SelectFirst()
        {
            Select(0);
        }

        public void Select(int index)
        {
            SelectedIndex = index;
        }

        /// <summary>
        /// 重置选择，通常用于 ItemsSource 内部发生变化时
        /// </summary>
        public void Reselect()
        {
            SelectedIndex = SelectedIndex;
        }

        /// <summary>
        /// 不选中任何项
        /// </summary>
        public void Unselect()
        {
            ComponentIndex = -1;
            //ItemIndex = -1;
        }

        private void CreateChildren()
        {
            Components = new();

            foreach (var c in GetComponentsInChildren<ListBoxItem>())
            {
                Destroy(c.gameObject);
            }

            for (int i = 0; i < Cols * Rows; i++)
            {
                CreateChild();
            }
        }

        private void CreateChild()
        {
            var obj = Instantiate(listItemPrefab, this.transform);
            var c = obj.GetComponent<ListBoxItem>();
            Components.Add(c);
        }

        private void UpdateTopItemIndex()
        {
            switch (workMode)
            {
                case WorkMode.Scroll:
                    if (SelectedIndex < TopItemIndex)
                    {
                        //TopItemIndex = ItemIndex - ItemIndex % Rows;
                        TopItemIndex = SelectedIndex / Cols * Cols;
                    }
                    else if (SelectedIndex >= TopItemIndex + Components.Count)
                    {
                        //TopItemIndex = ItemIndex - ItemIndex % Rows - Components.Count + Rows;
                        TopItemIndex = SelectedIndex / Cols * Cols - Components.Count + Cols;
                    }
                    break;
                case WorkMode.Page:
                    TopItemIndex = SelectedIndex / Components.Count * Components.Count;
                    break;
            }
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        public void Refresh()
        {
            RefreshComponents();
            switch (workMode)
            {
                case WorkMode.Scroll:
                    RefreshScrollBar();
                    break;
                case WorkMode.Page:
                    RefreshPageNumber();
                    break;
            }
        }

        /// <summary>
        /// 刷新视觉组件
        /// </summary>
        private void RefreshComponents()
        {
            for (
                int compIndex = 0, sourceIndex = TopItemIndex;
                compIndex < Components.Count;
                compIndex++, sourceIndex++
            )
            {
                if (sourceIndex >= 0 && sourceIndex < ItemsSource.Count)
                    RefreshAction?.Invoke(Components[compIndex], ItemsSource[sourceIndex]);
                else
                    RefreshAction?.Invoke(Components[compIndex], null);
            }
        }

        private void RefreshScrollBar()
        {
            if (scrollbar != null)
            {
                float total = ItemsSource.Count - Components.Count;
                if (total > 0)
                {
                    scrollbar.value = TopItemIndex / total;
                }
                else
                {
                    scrollbar.value = 0;
                }
                scrollbar.size = (float)Components.Count / ItemsSource.Count;
            }
        }

        private void RefreshPageNumber()
        {
            if (pageNumber != null)
            {
                int current = SelectedIndex / Components.Count + 1;
                int max = Mathf.CeilToInt((float)ItemsSource.Count / Components.Count);
                pageNumber.Refresh(current, max);
            }
        }

        public void ShowCursor()
        {
            if (SelectedComponent != null)
                SelectedComponent.Selected = true;
        }

        public void HideCursor()
        {
            if (SelectedComponent != null)
                SelectedComponent.Selected = false;
        }

        public void RegisterSelectedItemChangeCallback(UnityAction<object, int> callback)
        {
            selectedItemChanged.AddListener(callback);
        }

        public void UnregisterSelectedItemChangeCallback(UnityAction<object, int> callback)
        {
            selectedItemChanged.RemoveListener(callback);
        }
    }
}
