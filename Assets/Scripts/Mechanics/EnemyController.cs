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
        public AudioClip death;
        public AnimationController control;
        public Animator enemyAnimator;
        public Health health;
        public Coroutine deathCoroutine;
        public Coroutine spottedCoroutine;
        public bool isDead = false;
        public GameObject player;


        internal PatrolPath.Mover mover;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyAnimator = GetComponent<Animator>();

            health = GetComponent<Health>();
            StartWatch();

        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && !player.isGettingHurt)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }

            var enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                var ev = Schedule<EnemyEnemyCollision>();
                ev.player = enemy;
                ev.enemy = this;
                Physics2D.IgnoreCollision(enemy._collider, this._collider);
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
            deathCoroutine = StartCoroutine(EnemyDeathAnimationCoroutine());
        }

        public IEnumerator EnemyDeathAnimationCoroutine()
        {
            isDead = true;
            enemyAnimator.SetTrigger("death");
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
        public void StartWatch()
        {
            if (spottedCoroutine == null)
            {
                spottedCoroutine = StartCoroutine(StartWatchCoroutine());
            }
        }

        IEnumerator StartWatchCoroutine()
        {
            while (this.gameObject != null)
            {
                var distance = Vector3.Distance(player.transform.position, this.transform.position);
                if (distance < 10)
                {
                    player.GetComponent<PlayerController>().enemyInSigth = true;
                    player.GetComponent<PlayerController>().IncreaseStress(3);
                } 
                yield return new WaitForSeconds(1);
            }
        }
    }
}