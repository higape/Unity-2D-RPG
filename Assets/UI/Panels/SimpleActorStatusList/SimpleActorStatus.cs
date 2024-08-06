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

        public void SetActor(Actor actor)
        {
            CurrentActor = actor;
            Refresh();
        }

        private void Refresh()
        {
            if (CurrentActor != null)
            {
                displayObject.sprite = CurrentActor.BattleSkin.idle;
                nameContent.text = CurrentActor.Name;
                hpContent.text = UIManager.CreateHpText(CurrentActor.Hp, CurrentActor.Mhp);
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
