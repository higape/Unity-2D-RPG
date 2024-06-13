using UnityEngine.Events;

namespace Root
{
    public class InputCommand
    {
        public const string ButtonUp = "Up";
        public const string ButtonDown = "Down";
        public const string ButtonLeft = "Left";
        public const string ButtonRight = "Right";
        public const string ButtonPrevious = "Previous";
        public const string ButtonNext = "Next";
        public const string ButtonInteract = "Interact";
        public const string ButtonCancel = "Cancel";
        public const string ButtonMainMenu = "MainMenu";
        public const string ButtonSubMenu = "SubMenu";
        private const float DefaultDownTime = 0.2f;
        private const float DefaultPressTime = 0.15f;

        public InputCommand(string name, ButtonType type, UnityAction callback)
        {
            Name = name;
            Type = type;
            Callback = callback;
            Delay = type switch
            {
                ButtonType.Down => DefaultDownTime,
                ButtonType.Press => DefaultPressTime,
                _ => 0
            };
        }

        public InputCommand(string name, ButtonType type, UnityAction action, float delay)
        {
            Name = name;
            Type = type;
            Callback = action;
            Delay = delay;
        }

        public string Name { get; private set; }
        public ButtonType Type { get; private set; }
        public UnityAction Callback { get; private set; }
        public float Delay { get; private set; }
    }
}
