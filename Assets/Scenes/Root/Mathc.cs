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

            //角度除以2后转换为弧度
            float r0 = angle / 2 * Mathf.PI / 180;
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

        public static List<Enemy> GetSectorTarget(
            float angle,
            Vector3 startPosition,
            Vector3 endPosition,
            List<Enemy> targetList
        )
        {
            //此方法做的事：计算目标的身体矩形是否包含在扇形范围内，并返回符合条件的目标
            //可抽象为计算矩形的任一顶点是否在扇形的两条边之间
            //计算步骤：过矩形的顶点作一条垂直于y轴的线，求出线与扇形两条边相交的点，
            //如果有任一顶点处于其对应的两交点之间，或者交点处于两个顶点之间，则认为目标在范围内
            //判断点的位置关系时，仅比较y坐标，因此计算过程中不求出点的x坐标

            //获取扇形参数
            Vector4 kbkb = GetSectorParam(angle, startPosition, endPosition);

            //计算两条直线在垂直方向的位置关系
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
                var border = battler.BodyBorder;

                //两条直线对应的y值
                float leftUpY = k0 * border.x + b0;
                float leftDownY = k1 * border.x + b1;
                float rightUpY = k0 * border.y + b0;
                float rightDownY = k1 * border.y + b1;

                //根据y坐标确定位置关系
                if (
                    (border.z < leftUpY && border.z > leftDownY)
                    || (border.z < rightUpY && border.z > rightDownY)
                    || (border.w < rightUpY && border.w > rightDownY)
                    || (border.w < leftUpY && border.w > leftDownY)
                    || (leftUpY >= border.z && leftUpY <= border.w)
                    || (rightUpY >= border.z && rightUpY <= border.w)
                )
                    newList.Add(battler);
            }

            return newList;
        }

        /// <summary>
        /// 获取构成矩形的直线的k值和b值
        /// </summary>
        public static Vector3 GetRayParam(
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
            float b = endPosition.y - k * endPosition.x;

            //计算矩形两条边所在直线的b值
            float offsetY = rectWidth / 32f * Mathf.Sqrt(dx * dx + dy * dy) / Mathf.Abs(dx) / 2;
            return new Vector3(k, b + offsetY, b - offsetY);
        }

        public static List<Enemy> GetRayTarget(
            float rectWidth,
            Vector3 startPosition,
            Vector3 endPosition,
            List<Enemy> targetList
        )
        {
            //获取直线参数
            Vector3 kbb = GetRayParam(rectWidth, startPosition, endPosition);
            //计算两条直线在垂直方向的位置关系
            float k = kbb.x;
            float b0,
                b1;
            if (kbb.y > kbb.z)
            {
                b0 = kbb.y;
                b1 = kbb.z;
            }
            else
            {
                b0 = kbb.z;
                b1 = kbb.y;
            }

            //选取两线之间的目标
            List<Enemy> newList = new();
            foreach (Enemy battler in targetList)
            {
                var border = battler.BodyBorder;

                //两条直线对应的y值
                float leftUpY = k * border.x + b0;
                float leftDownY = k * border.x + b1;
                float rightUpY = k * border.y + b0;
                float rightDownY = k * border.y + b1;

                //根据y坐标确定位置关系
                if (
                    (border.z < leftUpY && border.z > leftDownY)
                    || (border.z < rightUpY && border.z > rightDownY)
                    || (border.w < rightUpY && border.w > rightDownY)
                    || (border.w < leftUpY && border.w > leftDownY)
                    || (leftUpY >= border.z && leftUpY <= border.w)
                    || (rightUpY >= border.z && rightUpY <= border.w)
                )
                    newList.Add(battler);
            }

            return newList;
        }

        public static List<Enemy> GetCircleTarget(
            float radius,
            Vector2 targetPosition,
            List<Enemy> targetList
        )
        {
            //单位转换
            float r = radius / 32f;

            List<Enemy> newList = new();
            foreach (Enemy battler in targetList)
            {
                var border = battler.BodyBorder;

                // 计算圆心到矩形的最近点的距离
                // float closestX = Mathf.Clamp(targetPosition.x, border.x, border.y);
                // float closestY = Mathf.Clamp(targetPosition.y, border.z, border.w);
                // Vector2 closestPoint = new(closestX, closestY);
                // float distance = Vector2.Distance(targetPosition, closestPoint);

                // 合并以上计算
                float distance = Vector2.Distance(
                    targetPosition,
                    new(
                        Mathf.Clamp(targetPosition.x, border.x, border.y),
                        Mathf.Clamp(targetPosition.y, border.z, border.w)
                    )
                );

                // 判断是否相交
                if (distance <= r)
                {
                    newList.Add(battler);
                }
            }

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
        /// 处理多个道具效果
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
                //过滤非目标种族的效果
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
                    continue;
                }

                var be = ed.GetBattleEffect();
                var pair = ProcessItemEffect(
                    be.type0,
                    be.type1,
                    ed.effectValue,
                    owner,
                    target,
                    elementType,
                    attack,
                    hit,
                    weaponEffectRate
                );
                damageCount += pair.Item1;
                recoverCount += pair.Item2;
            }

            return (damageCount, recoverCount);
        }

        /// <summary>
        /// 处理一个道具效果
        /// </summary>
        /// <returns>伤害量,治疗量</returns>
        public static (int, int) ProcessItemEffect(
            BET type0,
            int type1,
            int effectValue,
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
            switch (type0)
            {
                case BET.Damage:
                    switch ((BE.DamageType)type1)
                    {
                        case BE.DamageType.Rate:
                            damageCount += target.LoseHp(
                                ComputeDamage(
                                    attack - target.Def / 2,
                                    (hit - target.Eva) / 100f + 1f,
                                    target.GetElementRate(elementType),
                                    effectValue / 100f * weaponEffectRate,
                                    RandomCft()
                                )
                            );
                            break;
                        case BE.DamageType.Constant:
                            damageCount += target.LoseHp(
                                ComputeDamage(
                                    effectValue,
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
                                    attack * (effectValue / 100f) - target.Def / 2,
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
                                    attack - target.Def * (1f - effectValue / 100f) / 2,
                                    (hit - target.Eva) / 100f + 1f,
                                    target.GetElementRate(elementType),
                                    weaponEffectRate,
                                    RandomCft()
                                )
                            );
                            break;
                        case BE.DamageType.Durability:
                            damageCount += target.LoseDp(effectValue);
                            break;
                        case BE.DamageType.DefenceRate:
                            damageCount += target.LoseHp(
                                ComputeDamage(
                                    owner.Def * (effectValue / 100f) - target.Def / 2,
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
                    switch ((BE.RecoverType)type1)
                    {
                        case BE.RecoverType.LifeRate:
                            recoverCount += target.GainHp((int)(effectValue / 100f * target.Mhp));
                            break;
                        case BE.RecoverType.LifeConst:
                            recoverCount += target.GainHp(effectValue);
                            break;
                        case BE.RecoverType.LifeLevel:
                            recoverCount += target.GainHp(effectValue * owner.Level);
                            break;
                        case BE.RecoverType.RebornRate:
                            recoverCount += target.Reborn((int)(effectValue / 100f * target.Mhp));
                            break;
                        case BE.RecoverType.RebornConst:
                            recoverCount += target.Reborn(effectValue);
                            break;
                        case BE.RecoverType.RebornLevel:
                            recoverCount += target.Reborn(effectValue * owner.Level);
                            break;
                        case BE.RecoverType.Durability:
                            recoverCount += target.GainDp(effectValue);
                            break;
                    }
                    break;
                case BET.RemoveState:
                    target.RemoveState((BE.StateType)type1, effectValue);
                    break;
                case BET.Special:
                    switch ((BE.SpecialType)type1)
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

            return (damageCount, recoverCount);
        }
    }
}
