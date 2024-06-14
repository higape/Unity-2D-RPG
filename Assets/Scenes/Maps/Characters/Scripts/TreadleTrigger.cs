using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map
{
    /// <summary>
    /// 踩踏触发器
    /// </summary>
    [AddComponentMenu("地图/踩踏触发器")]
    public class TreadleTrigger : TreadleTriggerBase
    {
        protected override void OnHandle(GameObject character)
        {
            if (TryGetComponent<CommandInterpreter>(out var inter) && !inter.enabled)
            {
                inter.StartCommand();
            }
        }
    }
}
