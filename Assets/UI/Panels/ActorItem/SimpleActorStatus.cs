using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SimpleActorStatus : ListBoxItem
    {
        [SerializeField]
        private Image displayObject;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI hpLabel;

        [SerializeField]
        private TextMeshProUGUI hpContent;

        private Actor CurrentActor { get; set; }

        private void Awake()
        {
            hpLabel.text = ResourceManager.Term.hp;
        }

        public void SetActor(Actor human)
        {
            CurrentActor = human;
            Refresh();
        }

        private void Refresh()
        {
            if (CurrentActor != null)
            {
                displayObject.sprite = CurrentActor.BattleSkin.idle;
                nameContent.text = CurrentActor.Name;
                hpContent.text = CurrentActor.Hp.ToString() + '/' + CurrentActor.Mhp.ToString();
            }
            else
            {
                displayObject.sprite = null;
                nameContent.text = " ";
                hpContent.text = " ";
            }
        }
    }
}
