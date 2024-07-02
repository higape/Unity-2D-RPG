using Dynamic;
using Root;
using UnityEngine;
using UIT = Static.ActorUsableItem.ItemType;

namespace UI
{
    public class ActorUsableItemPanel : MonoBehaviour
    {
        private delegate ActorUsableItem MakeCommodity(int id);

        [SerializeField]
        private ListBox typeListBox;

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

        private (string, UIT)[] TextTypePairs { get; set; }

        private InputCommand[] InputCommands { get; set; }

        public void Setup(UIT itemType)
        {
            int index = -1;
            for (int i = 0; i < TextTypePairs.Length; i++)
                if (TextTypePairs[i].Item2 == itemType)
                    index = i;
            typeListBox.Select(index);
        }

        private void Awake()
        {
            InputCommands = new InputCommand[]
            {
                new(InputCommand.ButtonUp, ButtonType.Press, itemListBox.SelectUp),
                new(InputCommand.ButtonDown, ButtonType.Press, itemListBox.SelectDown),
                new(InputCommand.ButtonInteract, ButtonType.Down, Interact),
                new(InputCommand.ButtonCancel, ButtonType.Down, Cancel),
                new(InputCommand.ButtonPrevious, ButtonType.Down, typeListBox.SelectLeft),
                new(InputCommand.ButtonNext, ButtonType.Down, typeListBox.SelectRight),
            };

            TextTypePairs = new (string, UIT)[]
            {
                (ResourceManager.Term.recoverItem, UIT.RecoverItem),
                (ResourceManager.Term.attackItem, UIT.AttackItem),
                (ResourceManager.Term.auxiliaryItem, UIT.AuxiliaryItem)
            };

            itemListBox.Initialize(1, 8, RefreshItem);
            itemListBox.RegisterSelectedItemChangeCallback(OnSelectedItemChange);

            typeListBox.Initialize(TextTypePairs.Length, 1, RefreshType, TextTypePairs);
            typeListBox.RegisterSelectedItemChangeCallback(OnTypeChange);
        }

        private void OnEnable()
        {
            InputManagementSystem.AddCommands(nameof(ActorUsableItemPanel), InputCommands);
        }

        private void OnDisable()
        {
            InputManagementSystem.RemoveCommands(nameof(ActorUsableItemPanel));
        }

        private void OnTypeChange(object data, int index)
        {
            UIT itemType = (((string, UIT))typeListBox.SelectedItem).Item2;
            CurrentAction = (id) => new ActorUsableItem(itemType, id);
            itemListBox.SetSource(Party.GetActorItemList(itemType));
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

        private void RefreshType(ListBoxItem listItem, object data)
        {
            if (listItem is TextItem c)
            {
                if (data != null)
                    c.textComponent.text = (((string, UIT))data).Item1;
                else
                    c.textComponent.text = " ";
            }
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
