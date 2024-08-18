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
        public override Vector3 FirePosition => transform.position;
        private bool IsGoingDie { get; set; } = false;

        public void Setup(Enemy source)
        {
            Source = source;
            spriteRenderer.sprite = Source.Skin;
            transform.localPosition = new Vector3(
                source.LayoutPosition.x / source.Skin.pixelsPerUnit,
                source.LayoutPosition.y / source.Skin.pixelsPerUnit,
                0
            );
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
            float alpha = 1f;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * 2;
                c.a = alpha;
                spriteRenderer.color = c;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
