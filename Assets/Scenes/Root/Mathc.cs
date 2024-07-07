using System.Collections;
using System.Collections.Generic;
using Dynamic;
using UnityEngine;
using BE = Static.BattleEffect;
using BET = Static.BattleEffect.EffectType;

namespace Root
{
    public static class Mathc
    {
        public const float EnemyLayoutWidth = 896;
        public const float EnemyLayoutHeight = 504;

        public static void LayoutEnemy(List<Enemy> enemies) { }

        public static void LayoutNewEnemy(List<Enemy> olds, List<Enemy> news) { }

        /// <summary>
        /// 获取构成扇形的两条直线
        /// </summary>
        public static Vector4 GetSectorParam(
            float angle,
            Vector3 startPosition,
            Vector3 endPosition
        )
        {
            //将终点绕起点向两个方向旋转一定角度后得到两个点
            //这两个点与起点的连线构成扇形的两条边

            //终点相对起点的坐标
            float dx = endPosition.x - startPosition.x;
            float dy = endPosition.y - startPosition.y;

            //角度转换为弧度
            float r0 = angle / -2 * Mathf.PI / 180;
            //旋转后的点
            float px0 = dx * Mathf.Cos(r0) - dy * Mathf.Sin(r0) + startPosition.x;
            float py0 = dx * Mathf.Sin(r0) + dy * Mathf.Cos(r0) + startPosition.y;
            //直线的k值
            float k0 = (startPosition.y - py0) / (startPosition.x - px0);
            //直线的b值
            float b0 = startPosition.y - k0 * startPosition.x;

            //计算公式同上一条直线，只是弧度相反
            float r1 = -r0;
            float px1 = dx * Mathf.Cos(r1) - dy * Mathf.Sin(r1) + startPosition.x;
            float py1 = dx * Mathf.Sin(r1) + dy * Mathf.Cos(r1) + startPosition.y;
            float k1 = (startPosition.y - py1) / (startPosition.x - px1);
            float b1 = startPosition.y - k1 * startPosition.x;

            Vector4 rayPair = new(k0, b0, k1, b1);
            return rayPair;
        }

        public static List<Enemy> GetSectorTarget(Vector4 kbkb, List<Enemy> targetList)
        {
            //捋清两条线的位置关系
            float k0,
                b0,
                k1,
                b1;

            if (kbkb.x < kbkb.z)
            {
                k0 = kbkb.x;
                b0 = kbkb.y;
                k1 = kbkb.z;
                b1 = kbkb.w;
            }
            else
            {
                k0 = kbkb.z;
                b0 = kbkb.w;
                k1 = kbkb.x;
                b1 = kbkb.y;
            }

            //选取两线之间的目标
            List<Enemy> newList = new();
            foreach (Enemy battler in targetList)
            {
                //检查表示身体范围的矩形
                var rect = battler.ScopeRect;

                //各定点坐标值
                float leftX = rect.x,
                    rightX = rect.x + rect.width,
                    downY = rect.y,
                    upY = rect.y + rect.height;

                //直线在矩形左右两侧对应的y值
                float leftUpY = k0 * leftX + b0;
                float leftDownY = k1 * leftX + b1;
                float rightUpY = k0 * rightX + b0;
                float rightDownY = k1 * rightX + b1;

                //从矩形左上角顺时针依次检查四个点，任意一点在两线之间即在范围内
                //根据y坐标确定位置关系
                if (
                    (upY < leftUpY && upY > leftDownY)
                    || (upY < rightUpY && upY > rightDownY)
                    || (downY < rightUpY && downY > rightDownY)
                    || (downY < leftUpY && downY > leftDownY)
                )
                    newList.Add(battler);
            }

            return newList;
        }

        /// <summary>
        /// 获取构成矩形的直线的k值和b值
        /// </summary>
        public static Vector3 GetRectangleParam(
            float rectWidth,
            Vector3 startPosition,
            Vector3 endPosition
        )
        {
            //终点相对起点的坐标
            float dx = endPosition.x - startPosition.x;
            float dy = endPosition.y - startPosition.y;

            //直线的k值和b值
            float k = dy / dx;
            float b = dy - k * dx;

            //计算矩形两条边所在直线的b值
            float d = Mathf.Sqrt(dx * dx + dy * dy);
            float offsetY = rectWidth * d / Mathf.Abs(dx);
            return new Vector3(k, b + offsetY / 2, b + offsetY / -2);
        }

        public static List<Enemy> GetRectangleTarget(Vector3 kbb, List<Enemy> targetList)
        {
            List<Enemy> newList = new();

            return newList;
        }

        /// <summary>
        /// 返回一个在区间[0.8,1.2]的随机数
        /// </summary>
        /// <returns></returns>
        public static float RandomCft() => Random.Range(0.8f, 1.2f);

        /// <summary>
        /// 计算伤害
        /// </summary>
        private static int ComputeDamage(
            float baseDamage,
            float hitCft,
            float eleCft,
            float dmgCft,
            float rdmCft
        )
        {
            return (int)(baseDamage * hitCft * eleCft * dmgCft * rdmCft);
        }

        /// <summary>
        /// 处理道具效果
        /// </summary>
        /// <returns>伤害量,治疗量</returns>
        public static (int, int) ProcessItemEffect(
            Static.WeaponUsage usage,
            Battler owner,
            Battler target,
            int attack,
            int hit,
            float weaponEffectRate
        )
        {
            return ProcessItemEffect(
                usage.effects,
                owner,
                target,
                usage.element,
                attack,
                hit,
                weaponEffectRate
            );
        }

        /// <summary>
        /// 处理道具效果
        /// </summary>
        /// <returns>伤害量,治疗量</returns>
        public static (int, int) ProcessItemEffect(
            Static.DurationState state,
            Battler owner,
            Battler target,
            int effectValue,
            Static.ElementType elementType,
            int attack,
            int hit,
            float weaponEffectRate
        )
        {
            var edl = new List<Static.BattleEffectData>()
            {
                new()
                {
                    effectID = state.id,
                    effectValue = effectValue,
                    duration = 0
                }
            };
            return ProcessItemEffect(
                edl,
                owner,
                target,
                elementType,
                attack,
                hit,
                weaponEffectRate
            );
        }

        /// <summary>
        /// 处理道具效果
        /// </summary>
        /// <returns>伤害量,治疗量</returns>
        public static (int, int) ProcessItemEffect(
            IList<Static.BattleEffectData> effectDatas,
            Battler owner,
            Battler target,
            Static.ElementType elementType,
            int attack,
            int hit,
            float weaponEffectRate
        )
        {
            int damageCount = 0;
            int recoverCount = 0;

            foreach (var ed in effectDatas)
            {
                //过滤菲目标种族的效果
                if (ed.targetType != 0 && (ed.targetType | target.BattlerType) == 0)
                    continue;

                if (ed.duration > 0)
                {
                    if (Battle.BattleManager.IsBattling)
                    {
                        target.AddState(
                            new DurationState(
                                ed.GetDurationState(),
                                owner,
                                target,
                                ed.effectValue,
                                attack,
                                hit,
                                weaponEffectRate,
                                ed.duration
                            )
                        );
                    }
                    else
                    {
                        Debug.LogWarning("忽略了持续效果");
                    }
                }
                else
                {
                    var be = ed.GetBattleEffect();
                    switch (be.type0)
                    {
                        case BET.Damage:
                            switch ((BE.DamageType)be.type1)
                            {
                                case BE.DamageType.Rate:
                                    damageCount += target.LoseHp(
                                        ComputeDamage(
                                            attack - target.Def / 2,
                                            (hit - target.Eva) / 100f + 1f,
                                            target.GetElementRate(elementType),
                                            ed.effectValue / 100f * weaponEffectRate,
                                            RandomCft()
                                        )
                                    );
                                    break;
                                case BE.DamageType.Constant:
                                    damageCount += target.LoseHp(
                                        ComputeDamage(
                                            ed.effectValue,
                                            (hit - target.Eva) / 100f + 1f,
                                            target.GetElementRate(elementType),
                                            weaponEffectRate,
                                            RandomCft()
                                        )
                                    );
                                    break;
                                case BE.DamageType.AttackRate:
                                    damageCount += target.LoseHp(
                                        ComputeDamage(
                                            attack * (ed.effectValue / 100f) - target.Def / 2,
                                            (hit - target.Eva) / 100f + 1f,
                                            target.GetElementRate(elementType),
                                            weaponEffectRate,
                                            RandomCft()
                                        )
                                    );
                                    break;
                                case BE.DamageType.NonDefenceRate:
                                    damageCount += target.LoseHp(
                                        ComputeDamage(
                                            attack - target.Def * (1f - ed.effectValue / 100f) / 2,
                                            (hit - target.Eva) / 100f + 1f,
                                            target.GetElementRate(elementType),
                                            weaponEffectRate,
                                            RandomCft()
                                        )
                                    );
                                    break;
                                case BE.DamageType.Durability:
                                    damageCount += target.LoseDp(ed.effectValue);
                                    break;
                                case BE.DamageType.DefenceRate:
                                    damageCount += target.LoseHp(
                                        ComputeDamage(
                                            owner.Def * (ed.effectValue / 100f) - target.Def / 2,
                                            (hit - target.Eva) / 100f + 1f,
                                            target.GetElementRate(elementType),
                                            weaponEffectRate,
                                            RandomCft()
                                        )
                                    );
                                    break;
                            }
                            break;
                        case BET.Recover:
                            switch ((BE.RecoverType)be.type1)
                            {
                                case BE.RecoverType.LifeRate:
                                    recoverCount += target.GainHp(
                                        (int)(ed.effectValue / 100f * target.Mhp)
                                    );
                                    break;
                                case BE.RecoverType.LifeConst:
                                    recoverCount += target.GainHp(ed.effectValue);
                                    break;
                                case BE.RecoverType.LifeLevel:
                                    recoverCount += target.GainHp(ed.effectValue * owner.Level);
                                    break;
                                case BE.RecoverType.RebornRate:
                                    recoverCount += target.Reborn(
                                        (int)(ed.effectValue / 100f * target.Mhp)
                                    );
                                    break;
                                case BE.RecoverType.RebornConst:
                                    recoverCount += target.Reborn(ed.effectValue);
                                    break;
                                case BE.RecoverType.RebornLevel:
                                    recoverCount += target.Reborn(ed.effectValue * owner.Level);
                                    break;
                                case BE.RecoverType.Durability:
                                    recoverCount += target.GainDp(ed.effectValue);
                                    break;
                            }
                            break;
                        case BET.RemoveState:
                            target.RemoveState((BE.StateType)be.type1, ed.effectValue);
                            break;
                        case BET.Special:
                            switch ((BE.SpecialType)be.type1)
                            {
                                case BE.SpecialType.Escape:
                                    Debug.Log("暂无此功能");
                                    break;
                                case BE.SpecialType.Protect:
                                    Debug.Log("暂无此功能");
                                    break;
                                case BE.SpecialType.Suicide:
                                    owner.GoToDie();
                                    break;
                                case BE.SpecialType.CallHelper:
                                    Debug.Log("暂无此功能");
                                    break;
                            }
                            break;
                    }
                }
            }

            return (damageCount, recoverCount);
        }
    }
}
