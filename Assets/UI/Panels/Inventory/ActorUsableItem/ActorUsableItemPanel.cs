using Dynamic;
using Root;
using TMPro;
using UnityEngine;
using UIT = Static.ActorUsableItem.ItemType;

namespace UI
{
    public class ActorUsableItemPanel : MonoBehaviour
    {
        private delegate ActorUsableItem MakeCommodity(int id);

        [SerializeField]
        private TextMeshProUGUI header;

        [SerializeField]
        private ListBox itemListBox;

        [SerializeField]
        private ActorUsableItemStatistic itemStatistic;

        [SerializeField]
        private GameObject actorListPrefab;

        private MakeCommodity CurrentAction { get; set; }

        private SimpleActorStatusList ActorListInstance { get; set; }

        private QuantityList.ListItem CurrentQuantityItem { get; set; }

        private ActorUsableItem CurrentItem { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(UIT itemType)
        {
            header.text = ResourceManager.Term.GetText(itemType);
            CurrentAction = (id) => new ActorUsableItem(itemType, id);
            itemListBox.SetSource(Party.GetActorItemList(itemType));
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonLeft, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonRight, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonPrevious, ButtonType.Down, itemListBox.PageUp),
                new(InputCommand.ButtonNext, ButtonType.Down, itemListBox.PageDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorUsableItemPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorUsableItemPanel));
        }

        private void OnSelectedItemChange(object data, int index)
        {
            if (data is QuantityList.ListItem item)
            {
                itemStatistic.Refresh(CurrentAction(item.id));
            }
            else
            {
                itemStatistic.Refresh(null);
            }
        }

        private void Interact()
        {
            if (itemListBox.SelectedItem != null)
            {
                CurrentQuantityItem = (QuantityList.ListItem)itemListBox.SelectedItem;
                CurrentItem = CurrentAction(((QuantityList.ListItem)itemListBox.SelectedItem).id);
                if (CurrentItem.UsedInMenu)
                {
                    //打开角色面板并传递回调
                    ActorListInstance = UIManager
                        .Instantiate(actorListPrefab)
                        .GetComponent<SimpleActorStatusList>();
                    ActorListInstance.SetCallback(OnActorInteract);
                }
                else
                {
                    //提示道具无法使用
                    UIManager.StartMessage(ResourceManager.Term.promptCannotUseItemInMenu, null);
                }
            }
        }

        private void OnActorInteract(Battler actor)
        {
            if (CurrentQuantityItem.quantity > 0)
            {
                var item = CurrentItem;
                var result = Mathc.ProcessItemEffect(
                    item.Usage,
                    actor,
                    actor,
                    actor.Atk + item.Attack,
                    actor.Hit,
                    1f
                );
                //在生效的情况下消耗道具
                if (result.Item1 > 0 || result.Item2 > 0)
                {
                    item.Cost();
                    itemListBox.Refresh();
                    if (CurrentQuantityItem.quantity <= 0)
                        itemListBox.Reselect();
                }
                ActorListInstance.Refresh();
            }
        }

        private void Cancel()
        {
            Destroy(gameObject);
        }

        private void RefreshItem(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem2 c)
            {
                if (data is QuantityList.ListItem item)
                {
                    var d = CurrentAction(item.id);
                    c.textComponent0.text = d.Name;
                    c.textComponent1.text = item.quantity.ToString();
                }
                else
                {
                    c.textComponent0.text = " ";
                    c.textComponent1.text = " ";
                }
            }
        }
    }
}
