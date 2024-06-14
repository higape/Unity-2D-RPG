using System.Collections;
using System.Collections.Generic;
using Dynamic;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    public class DisplayActor : DisplayBattler
    {
        private const float MoveTime = 0.25f;
        private const float MoveDistance = 1f;

        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private SpriteRenderer bodyRenderer;

        [SerializeField]
        private SpriteRenderer weaponRenderer;

        [SerializeField]
        private Animator animator;

        /// <summary>
        /// 触发回调以发射子弹
        /// </summary>
        private UnityAction Callback { get; set; }

        private Actor Source { get; set; }

        private Static.Actor.ActorSkin BattleSkin => Source?.BattleSkin;

        public override Vector3 Position => transform.position;

        public override Vector3 FirePosition => transform.position;

        public void MoveToLeft()
        {
            StartCoroutine(ToLeft());
        }

        private IEnumerator ToLeft()
        {
            float speed = -MoveDistance / MoveTime;
            float deltaX = 0;

            while (deltaX > -MoveDistance)
            {
                float x = speed * Time.deltaTime;
                deltaX += x;
                rootObject.transform.Translate(new Vector3(x, 0, 0));
                yield return null;
            }
        }

        public void MoveToRight()
        {
            StartCoroutine(ToRight());
        }

        private IEnumerator ToRight()
        {
            float speed = MoveDistance / MoveTime;
            float deltaX = 0;

            while (deltaX < MoveDistance)
            {
                float x = speed * Time.deltaTime;
                deltaX += x;
                rootObject.transform.Translate(new Vector3(x, 0, 0));
                yield return null;
            }
        }

        public void Setup(Actor source)
        {
            Source = source;
            if (!Source.IsAlive)
                GoToDie();
        }

        public void GoToAlive()
        {
            animator.SetTrigger("Alive");
        }

        public override void GoToDie()
        {
            animator.SetTrigger("Dead");
        }

        /// <summary>
        /// 拿起武器并在合适时机触发回调
        /// </summary>
        public void ShowMotion(Static.ActorWeaponSkin weaponSkin, UnityAction callback)
        {
            //设置武器图像
            weaponRenderer.sprite = weaponSkin.sprite;

            Callback = callback;
            //触发角色动画
            animator.SetInteger("Motion", (int)weaponSkin.motion);
            animator.SetTrigger("Trigger");
        }

        /// <summary>
        /// 由动画事件调用，开始发射子弹
        /// </summary>
        public void Emit()
        {
            if (Callback != null)
            {
                Callback.Invoke();
                Callback = null;
            }
        }

        public void MotionIdle()
        {
            bodyRenderer.sprite = BattleSkin?.idle;
            weaponRenderer.sprite = null;
        }

        public void MotionRaiseGun()
        {
            bodyRenderer.sprite = BattleSkin?.raiseGun;
        }

        public void MotionLiftItem()
        {
            bodyRenderer.sprite = BattleSkin?.liftItem;
        }

        public void MotionThrow()
        {
            bodyRenderer.sprite = BattleSkin?.throwItem;
        }

        public void MotionDead()
        {
            bodyRenderer.sprite = BattleSkin?.dead;
        }
    }
}
