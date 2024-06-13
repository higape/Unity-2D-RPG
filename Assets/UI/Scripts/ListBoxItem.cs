using UnityEngine;

namespace UI
{
    /// <summary>
    /// 作为ListBox的子对象，展示列表内容的组件。
    /// </summary>
    public abstract class ListBoxItem : MonoBehaviour
    {
        [SerializeField]
        protected Animator animator;

        protected bool selected = false;

        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value && animator != null)
                {
                    animator.SetBool("Select", value);
                }
                selected = value;
            }
        }
    }
}
