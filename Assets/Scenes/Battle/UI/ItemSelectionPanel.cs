using System.Collections;
using System.Collections.Generic;
using Dynamic;
using UI;
using UnityEngine;

namespace Battle
{
    public class ItemSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private ListBox listBox;

        private Actor CurrentActor { get; set; }

        public void Setup(Actor human)
        {
            CurrentActor = human;
        }

        private void Interact() { }

        private void Cancel() { }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                c.textComponent.text = data as string;
            }
        }
    }
}
