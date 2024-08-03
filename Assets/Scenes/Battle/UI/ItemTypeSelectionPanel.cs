using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UIT = Static.ActorUsableItem.ItemType;

namespace Battle
{
    public class ItemTypeSelectionPanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private ListBox listBox;

        [SerializeField]
        private GameObject itemPanelPrefab;

        private Actor CurrentActor { get; set; }

        private UnityAction CancelCallback { get; set; }

        private UnityAction FinishCallback { get; set; }

        private InputCommand[] InputCommands { get; set; }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, listBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, listBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            (string, UIT)[] texts = new (string, UIT)[]
            {
                (ResourceManager.Term.recoverItem, UIT.RecoverItem),
                (ResourceManager.Term.attackItem, UIT.AttackItem),
                (ResourceManager.Term.auxiliaryItem, UIT.AuxiliaryItem),
            };

            listBox.Initialize(1, texts.Length, Refresh, texts);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ItemTypeSelectionPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ItemTypeSelectionPanel));
        }

        public void Setup(Actor actor, UnityAction cancelCallback, UnityAction finishCallback)
        {
            CurrentActor = actor;
            CancelCallback = cancelCallback;
            FinishCallback = finishCallback;
        }

        private void Refresh(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, UIT))data).Item1;
                else
                    c.textComponent.text = " ";
            }
        }

        private void Interact()
        {
            if (listBox.SelectedItem != null)
            {
                UIManager
                    .Instantiate(itemPanelPrefab)
                    .GetComponent<ItemSelectionPanel>()
                    .Setup(
                        CurrentActor,
                        () => canvasGroup.alpha = 1,
                        InvokeFinishCallback,
                        (((string, UIT))listBox.SelectedItem).Item2
                    );
                canvasGroup.alpha = 0;
            }
        }

        public void InvokeFinishCallback()
        {
            FinishCallback?.Invoke();
            Destroy(gameObject);
        }

        public void Cancel()
        {
            BattleManager.CurrentCommand.SelectedItems = null;
            CancelCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
