using System.Collections;
using System.Collections.Generic;
using Dynamic;
using Root;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Battle
{
    /// <summary>
    /// 管理战斗流程。
    /// 战斗采用回合制，角色在轮到其行动时选择指令。
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        public const float SmallSectorAngle = 16f;
        public const float BigSectorAngle = 24f;
        public const float SmallRayWidth = 20f; // pixel unit
        public const float BigRayWidth = 50f; // pixel unit
        public const float SmallCircleRadius = 100f; // pixel unit
        public const float BigCircleRadius = 200f; // pixel unit

        public class ItemInfo
        {
            public ItemInfo(Weapon weapon, Static.WeaponUsage usage)
            {
                Weapon = weapon;
                Usage = usage;
            }

            /// <summary>
            /// 武器，一些反馈效果的载体，如弹药消耗、冷却等
            /// </summary>
            public Weapon Weapon { get; set; }

            /// <summary>
            /// 要使用的用法，可以是不属于武器的用法，例如特殊炮弹
            /// </summary>
            public Static.WeaponUsage Usage { get; set; }
        }

        /// <summary>
        /// 玩家对角色的指令
        /// </summary>
        public class CommandInfo
        {
            /// <summary>
            /// 使用的技能，可为null
            /// </summary>
            public Skill SelectedSkill { get; set; }

            /// <summary>
            /// 选择的武器、道具，根据技能可能可以为null
            /// </summary>
            public List<ItemInfo> SelectedItems { get; set; } = new();

            /// <summary>
            /// SelectedItems 的索引，指示当前使用的武器、道具
            /// </summary>
            public int ItemIndex { get; set; } = 0;

            /// <summary>
            /// 选择的目标，范围攻击以此目标为基础获取更多目标
            /// </summary>
            public Battler SelectedTarget { get; set; }

            public ItemInfo ItemInfo => SelectedItems[ItemIndex];

            public float SkillEffectRate =>
                SelectedSkill == null ? 1f : SelectedSkill.SkillEffectRate;

            public List<Static.BattleEffectData> BattleEffectList
            {
                get
                {
                    List<Static.BattleEffectData> list = new();

                    if (SelectedSkill != null)
                        list.AddRange(SelectedSkill.AddedEffects);
                    list.AddRange(ItemInfo.Usage.effects);

                    return list;
                }
            }
        }

        public static BattleManager Instance { get; private set; }

        public static bool IsBattling { get; private set; }

        /// <summary>
        /// 回合计数，可能由敌人AI调用。
        /// </summary>
        public static int TurnCount { get; private set; } = 0;

        /// <summary>
        /// 玩家输入的指令
        /// </summary>
        public static CommandInfo CurrentCommand { get; set; }

        private static List<Enemy> Enemies { get; set; }

        private static List<Enemy> DeadEnemies { get; set; }

        private static List<ActorStatusUnit> ActorStatusList { get; set; }

        public static GameObject BattlerStatePrefab => Instance?.battlerStatePrefab;

        public static UnityEvent BattleEnded { get; set; } = new();

        public static void StartBattle(List<(int, int, Vector2)> enemyData, UnityAction callback)
        {
            if (!IsBattling)
            {
                IsBattling = true;
                BattleEnded.AddListener(callback);
                Enemies = new();
                DeadEnemies = new();
                foreach (var d in enemyData)
                {
                    var enemy = new Enemy(d.Item1, d.Item2);
                    enemy.LayoutPosition = d.Item3;
                    Enemies.Add(enemy);
                }

                Map.PlayerController.WaitCount++;
                ScreenManager.FadeOut(() =>
                {
                    Map.PlayerController.WaitCount--;
                    SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
                    ScreenManager.FadeIn(null);
                });
            }
            else
            {
                Debug.LogWarning("在已存在战斗的情况下请求开始新的战斗");
            }
        }

        private static void EndBattle()
        {
            IsBattling = false;

            ScreenManager.FadeOut(() =>
            {
                //复活死亡的角色
                foreach (var battler in Party.GetBattleActorList())
                {
                    battler.QuiteBattle();
                    battler.Reborn(1);
                }
                SceneManager.UnloadSceneAsync("Battle");
                ScreenManager.FadeIn(() =>
                {
                    BattleEnded.Invoke();
                    BattleEnded.RemoveAllListeners();
                });
            });
        }

        public static void EscapeBattle()
        {
            EndBattle();
        }

        //获取一个包含可能的目标的数组
        public static Enemy[] GetActorToEnemyTargets() => Enemies.ToArray();

        //获取一个包含可能的目标的数组
        public static Actor[] GetActorToActorTargets(Actor owner, Static.UsedScope scope)
        {
            List<Actor> list;
            switch (scope)
            {
                case Static.UsedScope.Self:
                    list = new() { owner };
                    break;
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.AllFriend:
                    list = Party.GetAliveBattleActorList();
                    break;
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriendExcludeSelf:
                    list = Party.GetAliveBattleActorList();
                    list.Remove(owner);
                    break;
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    list = Party.GetDeadBattleActorList();
                    break;
                default:
                    list = new();
                    break;
            }
            return list.ToArray();
        }

        public static Vector3 GetScopePivot(Battler battler, Static.UsedScope scope)
        {
            switch (scope)
            {
                case Static.UsedScope.None:
                case Static.UsedScope.Self:
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriend:
                case Static.UsedScope.AllFriendExcludeSelf:
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    if (battler is Enemy)
                        return Instance.enemyPivot.transform.position;
                    else
                        return Instance.actorPivot.transform.position;
                default:
                    if (battler is Enemy)
                        return Instance.actorPivot.transform.position;
                    else
                        return Instance.enemyPivot.transform.position;
            }
        }

        public static GameObject CreateBullet(Vector3 position)
        {
            return Instantiate(
                Instance.bulletPrefab,
                position,
                Quaternion.identity,
                Instance.animationLayer.transform
            );
        }

        public static GameObject CreateAnimation(Vector3 position)
        {
            return Instantiate(
                Instance.animationPrefab,
                position,
                Quaternion.identity,
                Instance.animationLayer.transform
            );
        }

        public static void CreateDigit(int digit, PopDigit.DigitStyle digitStyle, Vector3 position)
        {
            var c = Instantiate(Instance.digitPrefab, Instance.digitLayer.transform)
                .GetComponent<PopDigit>();
            //设置对象
            c.transform.position = position;
            c.Setup(digit, digitStyle);
        }

        /// <summary>
        /// 根据给定范围提供最佳目标。
        /// 暂时给个随机目标
        /// </summary>
        public static Battler BestTarget(Battler owner, Static.UsedScope scope)
        {
            List<Actor> hl;
            switch (scope)
            {
                case Static.UsedScope.None:
                case Static.UsedScope.Self:
                    return owner;
                case Static.UsedScope.OneFriend:
                case Static.UsedScope.AllFriend:
                    hl = Party.GetAliveBattleActorList();
                    if (hl.Count == 0)
                        return null;
                    else
                        return hl[0];
                case Static.UsedScope.OneFriendExcludeSelf:
                case Static.UsedScope.AllFriendExcludeSelf:
                    hl = Party.GetAliveBattleActorList();
                    foreach (var h in hl)
                        if (!ReferenceEquals(h, owner))
                            return h;
                    return null;
                case Static.UsedScope.OneDeadFriend:
                case Static.UsedScope.AllDeadFriend:
                    if (owner is Enemy)
                    {
                        //死掉的怪会被清除，无法复活
                        return null;
                    }
                    else
                    {
                        hl = Party.GetDeadBattleActorList();
                        if (hl.Count == 0)
                            return null;
                        else
                            return hl[0];
                    }
                case Static.UsedScope.OneEnemy:
                case Static.UsedScope.AllEnemy:
                    if (owner is Enemy)
                    {
                        hl = Party.GetAliveBattleActorList();
                        return hl[Random.Range(0, hl.Count)];
                    }
                    else
                    {
                        return Enemies[0];
                    }
                case Static.UsedScope.SmallSector:
                    return Enemies[0];
                case Static.UsedScope.BigSector:
                    return Enemies[0];
                case Static.UsedScope.SmallRay:
                    return Enemies[0];
                case Static.UsedScope.BigRay:
                    return Enemies[0];
                case Static.UsedScope.SmallCircle:
                    return Enemies[0];
                case Static.UsedScope.BigCircle:
                    return Enemies[0];
                default:
                    Debug.LogError($"无法获取目标, 成员: {owner.Name}, 范围: {scope}");
                    return null;
            }
        }

        /// <summary>
        /// 根据目标和范围生成目标列表
        /// </summary>
        public static List<Battler> GetTargets(
            Battler oldTarget,
            Battler owner,
            Static.UsedScope scope
        )
        {
            var targetList = new List<Battler>();

            switch (scope)
            {
                case Static.UsedScope.AllFriend:
                    if (owner is Enemy)
                        targetList.AddRange(Enemies);
                    else
                        targetList.AddRange(Party.GetAliveBattleActorList());
                    break;
                case Static.UsedScope.AllFriendExcludeSelf:
                    if (owner is Enemy)
                    {
                        targetList.AddRange(Enemies);
                        targetList.Remove(owner);
                    }
                    else
                    {
                        targetList.AddRange(Party.GetAliveBattleActorList());
                        targetList.Remove(owner);
                    }
                    break;
                case Static.UsedScope.AllDeadFriend:
                    if (owner is Actor)
                        targetList.AddRange(Party.GetDeadBattleActorList());
                    break;
                case Static.UsedScope.AllEnemy:
                    if (owner is Enemy)
                        targetList.AddRange(Party.GetAliveBattleActorList());
                    else
                        targetList.AddRange(Enemies);
                    break;
                case Static.UsedScope.SmallSector:
                    if (owner is Enemy)
                        targetList.Add(oldTarget);
                    else
                        targetList.AddRange(
                            Mathc.GetSectorTarget(
                                SmallSectorAngle,
                                owner.DisplayObject.Position,
                                oldTarget.DisplayObject.Position,
                                Enemies
                            )
                        );
                    break;
                case Static.UsedScope.BigSector:
                    if (owner is Enemy)
                        targetList.Add(oldTarget);
                    else
                        targetList.AddRange(
                            Mathc.GetSectorTarget(
                                BigSectorAngle,
                                owner.DisplayObject.Position,
                                oldTarget.DisplayObject.Position,
                                Enemies
                            )
                        );
                    break;
                case Static.UsedScope.SmallRay:
                    if (owner is Enemy)
                        targetList.Add(oldTarget);
                    else
                        targetList.AddRange(
                            Mathc.GetRayTarget(
                                SmallRayWidth,
                                owner.DisplayObject.Position,
                                oldTarget.DisplayObject.Position,
                                Enemies
                            )
                        );
                    break;
                case Static.UsedScope.BigRay:
                    if (owner is Enemy)
                        targetList.Add(oldTarget);
                    else
                        targetList.AddRange(
                            Mathc.GetRayTarget(
                                BigRayWidth,
                                owner.DisplayObject.Position,
                                oldTarget.DisplayObject.Position,
                                Enemies
                            )
                        );
                    break;
                case Static.UsedScope.SmallCircle:
                    targetList.AddRange(
                        Mathc.GetCircleTarget(
                            SmallCircleRadius,
                            oldTarget.DisplayObject.Position,
                            Enemies
                        )
                    );
                    break;
                case Static.UsedScope.BigCircle:
                    targetList.AddRange(
                        Mathc.GetCircleTarget(
                            BigCircleRadius,
                            oldTarget.DisplayObject.Position,
                            Enemies
                        )
                    );
                    break;
                default:
                    targetList.Add(oldTarget);
                    break;
            }

            return targetList;
        }

        public static void CommandInputEnd() => Instance.ProcessActorAction();

        /// <summary>
        /// 清除所有静态成员
        /// </summary>
        private static void ClearStaticMember()
        {
            Instance = null;
            CurrentCommand = null;
            Enemies = null;
            DeadEnemies = null;
            ActorStatusList = null;
        }

        #region 实例成员

        /// <summary>
        /// 进行范围打击时，提供给子弹特效的位置
        /// </summary>
        [SerializeField]
        private GameObject actorPivot;

        /// <summary>
        /// 进行范围打击时，提供给子弹特效的位置
        /// </summary>
        [SerializeField]
        private GameObject enemyPivot;

        [SerializeField]
        private GameObject actorLayer;

        [SerializeField]
        private GameObject actorPrefab;

        [SerializeField]
        private GameObject enemyLayer;

        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField]
        private GameObject animationLayer;

        [SerializeField]
        private GameObject animationPrefab;

        [SerializeField]
        private GameObject battlerStatePrefab;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private GameObject digitLayer;

        [SerializeField]
        private GameObject digitPrefab;

        [SerializeField]
        private GameObject commandPanelPrefab;

        [SerializeField]
        private GameObject actorStatusLayer;

        [SerializeField]
        private GameObject actorStatusPrefab;

        /// <summary>
        /// 当前回合，经过排序的，未行动的战斗者
        /// </summary>
        private List<Battler> ActionableBattlers { get; set; } = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            ClearStaticMember();
        }

        private void Start()
        {
            CreateActor();
            CreateEnemy();
            CreateActorStatus();

            TurnCount = 0;
            NewTurn();
        }

        private void CreateActor()
        {
            var bl = Party.GetBattleActorList();

            for (int i = 0; i < bl.Count; i++)
            {
                Actor b = bl[i];
                DisplayActor d = Instantiate(actorPrefab, actorLayer.transform)
                    .GetComponent<DisplayActor>();
                d.Setup(b);
                d.transform.localPosition = new Vector3(0, 8f - 24f * i / bl.Count, 0);
                b.DisplayObject = d;
                b.EnterBattle();
            }
        }

        private void CreateEnemy()
        {
            //根据数据生成显示对象
            foreach (var b in Enemies)
            {
                DisplayEnemy d = Instantiate(enemyPrefab, enemyLayer.transform)
                    .GetComponent<DisplayEnemy>();
                d.Setup(b);
                b.DisplayObject = d;
                b.EnterBattle();
            }
        }

        private void CreateActorStatus()
        {
            ActorStatusList = new();
            var bl = Party.GetBattleActorList();

            for (int i = 0; i < Party.MaxBattleMembers; i++)
            {
                var unit = Instantiate(actorStatusPrefab, actorStatusLayer.transform)
                    .GetComponent<ActorStatusUnit>();

                if (i < bl.Count)
                    unit.Rebind(bl[i]);
                else
                    unit.Rebind(null);

                ActorStatusList.Add(unit);
            }
        }

        /// <summary>
        /// 开始新的回合
        /// </summary>
        private void NewTurn()
        {
            TurnCount++;
            ActionableBattlers = new();
            ActionableBattlers.AddRange(Party.GetBattleActorList());
            ActionableBattlers.AddRange(Enemies);

            //根据战斗者速度生成序列
            ActionableBattlers.Sort(CompareBattlerSpeed);

            NextBattler();
        }

        private int CompareBattlerSpeed(Battler a, Battler b)
        {
            //省略数值随机浮动
            return b.Agi - a.Agi;
        }

        private void NextBattler()
        {
            if (CheckBattleEnd())
            {
                return;
            }

            //去除无法行动的
            while (ActionableBattlers.Count > 0 && !ActionableBattlers[0].IsAlive)
            {
                ActionableBattlers.RemoveAt(0);
            }

            if (ActionableBattlers.Count == 0)
            {
                TurnEnd();
                return;
            }

            var controlState = ActionableBattlers[0].ControlState;

            if (ActionableBattlers[0] is Actor actor)
            {
                bool enableSkill = true;

                switch (controlState)
                {
                    case Static.BattleEffect.ControlType.Charm:
                        Debug.LogWarning($"未实现魅惑，跳过角色{actor.Name}的行动");
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Confusion:
                        Debug.LogWarning($"未实现混乱，跳过角色{actor.Name}的行动");
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Fetter:
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Panic:
                        enableSkill = false;
                        break;
                }

                (actor.DisplayObject as DisplayActor).MoveToLeft();
                //打开指令面板
                UI.UIManager
                    .Instantiate(commandPanelPrefab)
                    .GetComponent<BattleCommandPanel>()
                    .Setup(actor, enableSkill);
            }
            else
            {
                var enemy = ActionableBattlers[0] as Enemy;
                enemy.ResetActionCount();

                switch (controlState)
                {
                    case Static.BattleEffect.ControlType.Charm:
                        Debug.LogWarning($"未实现魅惑，跳过敌人{enemy.Name}的行动");
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Confusion:
                        Debug.LogWarning($"未实现混乱，跳过敌人{enemy.Name}的行动");
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Fetter:
                        ActionEnd();
                        return;
                    case Static.BattleEffect.ControlType.Panic:
                        // do nothing
                        break;
                }

                ProcessEnemyAction();
            }
        }

        private void TurnEnd()
        {
            int i = 0;

            while (i < Enemies.Count)
            {
                Enemy enemy = Enemies[i];
                enemy.UpdateDurationState();

                if (enemy.IsAlive)
                {
                    enemy.ResetActionCount();
                    i++;
                }
            }

            foreach (var actor in Party.GetBattleActorList())
                actor.UpdateDurationState();

            if (!CheckBattleEnd())
                NewTurn();
        }

        private void ActionEnd()
        {
            ActionableBattlers.RemoveAt(0);

            //从行动队列去除死的战斗者
            int i = 0;
            while (i < ActionableBattlers.Count)
            {
                if (ActionableBattlers[i].IsAlive)
                    i++;
                else
                    ActionableBattlers.RemoveAt(i);
            }

            StartCoroutine(DelayNextBattler());
        }

        private IEnumerator DelayNextBattler()
        {
            yield return new WaitForSeconds(.5f);
            NextBattler();
        }

        //检查战斗结束，如果符合条件，将会进入战斗结束流程
        private bool CheckBattleEnd()
        {
            if (AllEnemyDead())
            {
                //战斗胜利
                UI.UIManager.StartMessage(ResourceManager.Term.battleVictory, ShowBattleVictory);
                return true;
            }
            else if (AllActorDead())
            {
                //战斗失败
                UI.UIManager.StartMessage(ResourceManager.Term.battleDefeat, EndBattle);
                return true;
            }

            return false;
        }

        private bool AllActorDead()
        {
            foreach (var b in Party.GetBattleActorList())
                if (b.IsAlive)
                    return false;
            return true;
        }

        private bool AllEnemyDead()
        {
            int i = 0;
            while (i < Enemies.Count)
            {
                var e = Enemies[i];
                if (!e.IsAlive)
                {
                    DeadEnemies.Add(e);
                    Destroy(e.DisplayObject);
                    e.QuiteBattle();
                    Enemies.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return Enemies.Count == 0;
        }

        private void ShowBattleVictory()
        {
            int goldTotal = 0;
            int expTotal = 0;
            List<string> messages = new();

            //统计战利品
            foreach (var e in DeadEnemies)
            {
                goldTotal += e.Gold;
                expTotal += e.Exp;
            }

            //金钱乘上比例
            goldTotal = Mathf.FloorToInt(goldTotal * Party.BattleGoldRate);
            //获得金钱
            Party.GainGold(goldTotal);

            //添加各种消息
            messages.Add(
                string.Format(ResourceManager.Term.gainGoldInBattle, goldTotal)
                    + '\n'
                    + string.Format(ResourceManager.Term.gainExpInBattle, expTotal)
            );
            foreach (var a in Party.GetBattleActorList())
            {
                //角色获得经验并检查升级
                if (a.GainExpInBattle(expTotal))
                {
                    messages.Add(string.Format(ResourceManager.Term.actorLevelUp, a.Name));
                }
            }
            UI.UIManager.StartMessage(messages, () => EndBattle());
        }

        /// <summary>
        /// 根据敌人行动生成子弹，也是子弹爆炸时的回调方法
        /// </summary>
        private void ProcessEnemyAction()
        {
            if (!CheckBattleEnd())
            {
                var enemy = ActionableBattlers[0] as Enemy;
                var action = enemy.GetAction();

                if (action != null)
                {
                    var target = BestTarget(enemy, action.Usage.scope);
                    if (target != null)
                    {
                        action.Emit(
                            target,
                            enemy,
                            action.Usage,
                            1f,
                            new(action.Usage.effects),
                            ProcessEnemyAction
                        );
                        return;
                    }
                }

                ActionEnd();
            }
        }

        /// <summary>
        /// 玩家选择指令后调用，根据指令行动。
        /// </summary>
        private void ProcessActorAction()
        {
            //执行选择类技能的消费和冷却
            if (CurrentCommand.SelectedSkill != null)
                switch (CurrentCommand.SelectedSkill.SkillType)
                {
                    case Static.Skill.SkillType.SelectActorWeapon:
                    case Static.Skill.SkillType.SelectActorItem:
                        CurrentCommand.SelectedSkill.CostAndCool();
                        break;
                }

            ProcessActorItem();
        }

        /// <summary>
        /// 根据选择的item生成子弹，通过回调方法遍历所有item
        /// </summary>
        private void ProcessActorItem()
        {
            if (!CheckBattleEnd())
            {
                if (CurrentCommand.ItemIndex < CurrentCommand.SelectedItems.Count)
                {
                    var target = CurrentCommand.SelectedTarget;
                    var itemInfo = CurrentCommand.ItemInfo;
                    var skillEffectRate = CurrentCommand.SkillEffectRate;
                    var battleEffectList = CurrentCommand.BattleEffectList;
                    CurrentCommand.ItemIndex++;

                    itemInfo
                        .Weapon
                        .Emit(
                            target,
                            ActionableBattlers[0],
                            itemInfo.Usage,
                            skillEffectRate,
                            battleEffectList,
                            ProcessActorItem
                        );
                }
                else
                {
                    ActionEnd();
                }
            }
        }

        // private void OnDrawGizmos()
        // {
        //     TestScope(new Vector3(9f, 8f, 0), new Vector3(-6f, 5.56f, 0));
        // }

        // //测试绘制攻击范围
        // private void TestScope(Vector3 start, Vector3 end)
        // {
        //     var kbb = Mathc.GetRectangleParam(SmallRayWidth, start, end);

        //     float k = kbb.x;
        //     float b0,
        //         b1;
        //     if (kbb.y > kbb.z)
        //     {
        //         b0 = kbb.y;
        //         b1 = kbb.z;
        //     }
        //     else
        //     {
        //         b0 = kbb.z;
        //         b1 = kbb.y;
        //     }

        //     Vector4 border = new(-9f, -3f, 2.56f, 8.56f);

        //     float leftUpY = k * border.x + b0;
        //     float leftDownY = k * border.x + b1;
        //     float rightUpY = k * border.y + b0;
        //     float rightDownY = k * border.y + b1;

        //     Gizmos.DrawWireCube(
        //         new Vector3(-6f, 5.56f * k + (b0 + b1) / 2f),
        //         new Vector3(3f, 3f, 3f)
        //     );

        //     Gizmos.DrawWireCube(new Vector3(border.x, border.w), new Vector3(0.25f, 0.25f, 0.25f));
        //     Gizmos.DrawWireCube(new Vector3(border.y, border.w), new Vector3(0.25f, 0.25f, 0.25f));
        //     Gizmos.DrawWireCube(new Vector3(border.y, border.z), new Vector3(0.25f, 0.25f, 0.25f));
        //     Gizmos.DrawWireCube(new Vector3(border.x, border.z), new Vector3(0.25f, 0.25f, 0.25f));

        //     Gizmos.DrawWireCube(new Vector3(border.x, leftUpY), new Vector3(0.5f, 0.5f, 0.5f));
        //     Gizmos.DrawWireCube(new Vector3(border.y, rightUpY), new Vector3(0.5f, 0.5f, 0.5f));
        //     Gizmos.DrawWireCube(new Vector3(border.y, rightDownY), new Vector3(0.5f, 0.5f, 0.5f));
        //     Gizmos.DrawWireCube(new Vector3(border.x, leftDownY), new Vector3(0.5f, 0.5f, 0.5f));
        // }

        #endregion
    }
}
