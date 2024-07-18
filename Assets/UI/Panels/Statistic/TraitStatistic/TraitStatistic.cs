using Root;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TraitStatistic : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI traitLabel;

        [SerializeField]
        private ListBox traitListBox;

        private void Awake()
        {
            traitLabel.text = ResourceManager.Term.trait;
            traitListBox.Initialize(1, 3, RefreshTraitItem);
        }

        public void ListPageUp() => traitListBox.PageUp();

        public void ListPageDown() => traitListBox.PageDown();

        public void Refresh(Static.TraitData[] items)
        {
            traitListBox.SetSource(items);
            traitListBox.Unselect();
        }

        private void RefreshTraitItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data is Static.TraitData item)
                    c.textComponent.text = item.GetStatement();
                else
                    c.textComponent.text = " ";
            }
        }
    }
}
