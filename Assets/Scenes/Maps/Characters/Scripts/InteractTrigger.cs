using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 交互触发器
    /// </summary>
    [AddComponentMenu("地图/交互触发器")]
    public class InteractTrigger : InteractTriggerBase
    {
        protected override void OnInteract()
        {
            if (TryGetComponent<CommandInterpreter>(out var inter) && !inter.enabled)
            {
                if (TryGetComponent<Mover>(out var mover))
                {
                    //能移动的对象看向玩家
                    mover.LookAtPlayer();
                }
                inter.StartCommand();
            }
        }
    }
}
