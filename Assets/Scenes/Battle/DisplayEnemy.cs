using System.Collections;
using System.Collections.Generic;
using Dynamic;
using UnityEngine;

namespace Battle
{
    public class DisplayEnemy : DisplayBattler
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Enemy Source { get; set; }
        public override Vector3 Position => transform.position;
        public Rect ScopeRect => Source.ScopeRect;
        public override Vector3 FirePosition => transform.position;
        private bool IsGoingDie { get; set; } = false;

        public void Setup(Enemy source)
        {
            Source = source;
            spriteRenderer.sprite = Source.Skin;
        }

        public override void GoToDie()
        {
            if (!IsGoingDie)
            {
                IsGoingDie = true;
                StartCoroutine(Fade());
            }
        }

        /// <summary>
        /// 褪色结束后销毁对象
        /// </summary>
        private IEnumerator Fade()
        {
            Color c = spriteRenderer.color;
            for (float alpha = 1f; alpha >= 0; alpha -= 0.1f)
            {
                c.a = alpha;
                spriteRenderer.color = c;
                yield return new WaitForSeconds(.1f);
            }
            Destroy(gameObject);
        }
    }
}
