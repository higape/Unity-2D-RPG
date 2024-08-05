using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using UnityEngine;
using UnityEngine.Events;
using BE = Static.BattleEffect;
using BET = Static.BattleEffect.EffectType;

namespace Dynamic
{
    /// <summary>
    /// 所有战斗行为的基类
    /// </summary>
    public abstract class Weapon
    {
        /// <summary>
        /// 使用者
        /// </summary>
        public Battler Owner { get; private set; }

        /// <summary>
        /// 打击目标
        /// </summary>
        public Battler Target { get; private set; }

        /// <summary>
        /// 装备的攻击力
        /// </summary>
        public abstract int Attack { get; }

        /// <summary>
        /// 装备效率，影响效果计算。
        /// </summary>
        public virtual float EquipmentEffectRate { get; set; } = 1f;

        /// <summary>
        /// 技能的效率，仅在使用选择类技能时影响效果计算。
        /// </summary>
        public virtual float SkillEffectRate { get; set; } = 1f;

        /// <summary>
        /// 两种效率值相乘
        /// </summary>
        public float EffectRate => EquipmentEffectRate * SkillEffectRate;

        /// <summary>
        /// 子弹出生位置
        /// </summary>
        protected abstract Vector3 FirePosition { get; }

        /// <summary>
        /// 缓存
        /// </summary>
        protected Static.WeaponUsage CurrentUsage { get; set; }

        /// <summary>
        /// 冷却时间
        /// </summary>
        public int CurrentCoolingTime { get; protected set; } = 0;

        /// <summary>
        /// 判断武器是否正在冷却
        /// </summary>
        public bool IsCooling => CurrentCoolingTime > 0;

        /// <summary>
        /// 缓存技能效果和武器效果
        /// </summary>
        private List<Static.BattleEffectData> Effects { get; set; }

        /// <summary>
        /// 完成发射后执行
        /// </summary>
        private UnityAction Callback { get; set; }

        /// <summary>
        /// 已发射次数，用于判断是否发射完毕
        /// </summary>
        private int EmitCount { get; set; }

        /// <summary>
        /// 用于判断子弹是否完成飞行
        /// </summary>
        private int HitCount { get; set; }

        private int MaxHitCount { get; set; }

        private int DamageCount { get; set; }

        /// <summary>
        /// 发射子弹，发射完毕后回调
        /// </summary>
        public void Emit(
            Battler target,
            Battler owner,
            Static.WeaponUsage usage,
            float skillEffectRate,
            List<Static.BattleEffectData> effects,
            UnityAction callback
        )
        {
            Target = target;
            Owner = owner;
            CurrentUsage = usage;
            SkillEffectRate = skillEffectRate;
            Effects = effects;
            Callback = callback;
            EmitCount = 0;
            DamageCount = 0;
            if (CostAndCool())
                StartBullet();
            else
                Callback?.Invoke();
        }

        public abstract bool CostAndCool();

        /// <summary>
        /// 允许子类决定子弹发射的时机，或发射前后做什么
        /// </summary>
        protected abstract void StartBullet();

        protected void ProcessBullet()
        {
            //开始新的一轮子弹发射
            if (EmitCount < CurrentUsage.attackCount)
            {
                //获取目标列表
                var targets = BattleManager.GetTargets(Target, Owner, CurrentUsage.scope);
                //根据子弹类型生成子弹
                var bullets = new List<BulletProcessor>();
                //获取子弹数据
                var bulletData = CurrentUsage.GetBullet(EmitCount);

                switch (bulletData.createType)
                {
                    case Static.Bullet.CreateType.OneAll:
                        bullets.Add(
                            new(
                                bulletData,
                                FirePosition,
                                BattleManager.GetScopePivot(Target, CurrentUsage.scope),
                                targets,
                                OnTargetHit
                            )
                        );
                        break;
                    case Static.Bullet.CreateType.Every:
                        foreach (var t in targets)
                        {
                            bullets.Add(
                                new(
                                    bulletData,
                                    FirePosition,
                                    t.DisplayObject.Position,
                                    new() { t },
                                    OnTargetHit
                                )
                            );
                        }
                        break;
                }

                //更新参数
                HitCount = 0;
                MaxHitCount = bullets.Count;
                EmitCount++;
                //开始子弹动画
                foreach (var b in bullets)
                {
                    //必须确定子弹数量再开始动画，以正确判断全部子弹命中
                    b.StartAnimation();
                }
            }
            else
            {
                ProcessCostEffect();
                Callback?.Invoke();
            }
        }

        /// <summary>
        /// 子弹命中时调用
        /// </summary>
        private void OnTargetHit(List<Battler> targets)
        {
            int atk = Owner.Atk + Attack;
            //向每个目标施加效果
            foreach (var target in targets)
            {
                DamageCount += Root.Mathc
                    .ProcessItemEffect(
                        Effects,
                        Owner,
                        target,
                        CurrentUsage.element,
                        atk,
                        Owner.Hit,
                        EffectRate
                    )
                    .Item1;
            }

            //计数，然后开始下一轮
            if (++HitCount >= MaxHitCount)
                ProcessBullet();
        }

        /// <summary>
        /// 此武器的子弹发射完毕后执行效果
        /// </summary>
        private void ProcessCostEffect()
        {
            BE be;
            foreach (var ed in Effects)
            {
                if (ed.duration > 0)
                    continue;

                be = ed.GetBattleEffect();

                switch (be.type0)
                {
                    case BET.Cost:
                        switch ((BE.CostType)be.type1)
                        {
                            case BE.CostType.Durability:
                                Owner.LoseDp(ed.effectValue);
                                break;
                            case BE.CostType.NormalItem:
                                Party.ActorNormalItem.LoseItem(ed.effectValue, 1);
                                break;
                        }
                        break;
                    case BET.Repay:
                        Owner.GainHp(DamageCount);
                        break;
                }
            }
        }

        public void OnBattleStart()
        {
            CurrentCoolingTime = 0;
        }
    }
}
