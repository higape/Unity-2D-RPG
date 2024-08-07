using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using TMPro;
using UI;
using UnityEngine;

namespace Battle
{
    public class ActorStatusUnit : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI nameContent;

        [SerializeField]
        private TextMeshProUGUI hpLabel;

        [SerializeField]
        private TextMeshProUGUI hpContent;

        [SerializeField]
        private TextMeshProUGUI stateContent;

        private Actor boundActor;

        private void Awake()
        {
            hpLabel.text = ResourceManager.Term.hp;
        }

        private void OnDestroy()
        {
            boundActor?.UnregisterAnyChangeCallback(Refresh);
        }

        /// <summary>
        /// 绑定到角色并刷新
        /// </summary>
        public void Rebind(Actor battler)
        {
            boundActor?.UnregisterAnyChangeCallback(Refresh);

            boundActor = battler;
            boundActor?.RegisterAnyChangeCallback(Refresh);
            Refresh(boundActor);
        }

        private void Refresh(Battler battler)
        {
            if (battler == null)
            {
                canvasGroup.alpha = 0;
            }
            else
            {
                canvasGroup.alpha = 1f;
                nameContent.text = battler.Name;
                hpContent.text = UIManager.CreateHpText(battler.Hp, battler.Mhp);
                stateContent.text = ResourceManager.Term.GetText(battler.ControlState);
            }
        }
    }
}
