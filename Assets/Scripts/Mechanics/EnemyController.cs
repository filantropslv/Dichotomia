using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;
        public AnimationController control;
        public Animator enemyAnimator;
        public Health health;
        internal PatrolPath.Mover mover;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        internal bool isPlayingDeathAnimation = false;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyAnimator = GetComponent<Animator>();
            health = GetComponent<Health>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
            var enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log("test");
                Physics2D.IgnoreCollision(enemy.gameObject.GetComponent<Collider2D>(), this._collider);
            }
        }

        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

        public void EnemyDeathAnimation()
        {
            StartCoroutine(EnemyDeathAnimationCoroutine());
        }

        public IEnumerator EnemyDeathAnimationCoroutine()
        {
            isPlayingDeathAnimation = true;
            enemyAnimator.SetTrigger("death");
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
            isPlayingDeathAnimation = false;
        }

    }
}