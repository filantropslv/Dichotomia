using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.Tilemaps;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        public AudioClip deathAudio;

        public AudioClip jekyllTheme;
        public AudioClip hydeTheme;

        public Sprite jekyllSprite;
        public Sprite hydeSprite;


        /// <summary>
        /// Is the Player transformed
        /// </summary>
        public bool transformed = false;
        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;
        /// <summary>
        /// Is the Player frozen
        /// </summary>
        public bool frozen = false;
        public float musictime = 0f;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public BoxCollider2D boxCollider2d;
        public AudioSource audioSource;
        public AudioSource audioSourceParent;
        public Camera mainCamera;
        public Health health;
        public bool controlEnabled = true;

        public Color color1 = Color.red;
        public Color color2 = Color.blue;
        public float duration = 3.0F;
        public Tilemap[] levelTilemaps;
        public Transform frontCheck;
        public float meleeRange = 0.5f;
        public int transformationCooldown = 90;
        public int currentCountdown = 90;
        public int hydeCooldown = 5;
        public int currentHydeCooldown = 5;
        public bool isGettingHurt = false;
        public Coroutine levelCountdownCoroutine;
        public int killCount = 0;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            audioSourceParent = mainCamera.gameObject.GetComponent<AudioSource>();
            StartCountdown();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");

                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (transformed)
                {
                    MeleeAttack();
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetFloat("velocityY", Mathf.Abs(velocity.y));

            targetVelocity = move * maxSpeed;
        }

        public void Transform()
        {
            controlEnabled = false;
            transformed = !transformed;
            Vector2 newColliderVector = transformed ? new Vector2(0.9f, 1.4f) : new Vector2(0.65f, 1f);
            boxCollider2d.size = newColliderVector;
            animator.SetBool("transformed", transformed);
            animator.SetTrigger("transformTrigger");
            switch (transformed)
            {
                // Hyde code
                case true:
                    Debug.Log("Transformed into Hyde");
                    ChangeMusic(hydeTheme);
                    spriteRenderer.sprite = hydeSprite;
                    maxSpeed = 3;
                    jumpTakeOffSpeed = 6;
                    health.maxHP = 4;
                    StartCoroutine(TransformCountDownCoroutine());
                    break;
                // Jykell code
                case false:
                    Debug.Log("Transformed into Jykell");
                    ChangeMusic(jekyllTheme);
                    spriteRenderer.sprite = jekyllSprite;
                    maxSpeed = 4f;
                    jumpTakeOffSpeed = 8;
                    health.maxHP = 2;
                    break;
            }
            health.isRegenerating = transformed;
            health.DecrementByValue(0);
            health.ToggleRegen();
            ChangeBackgroundColor();
            controlEnabled = true;
        }
        public void ChangeBackgroundColor()
        {
            mainCamera.backgroundColor = transformed ? color2 : color1;
            foreach (Tilemap levelTilemap in levelTilemaps)
            {
                levelTilemap.color = transformed ? color2 : color1;
            }
        }
        public void ChangeMusic(AudioClip clip)
        {
            musictime = audioSourceParent.time;
            audioSourceParent.Stop();
            audioSourceParent.clip = clip;
            audioSourceParent.time = musictime;
            audioSourceParent.Play();
        }
        public void MeleeAttack()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Melee"))
            {
                animator.SetTrigger("melee");
            }
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(frontCheck.position, meleeRange, LayerMask.GetMask("Enemies"));
            foreach (Collider2D enemy in hitEnemies)
            {
                var ev = Schedule<PlayerMeleeEnemy>();
                ev.enemy = enemy.gameObject.GetComponent<EnemyController>();
                ev.player = this;
            }
        }

        public void StartCountdown()
        {
            if (levelCountdownCoroutine == null)
            {
                levelCountdownCoroutine = StartCoroutine(CountDownCoroutine());
            }
        }

        IEnumerator CountDownCoroutine()
        {
            while (currentCountdown > 0)
            {
                Debug.Log("CountDownCoroutine " + currentCountdown);
                Debug.Log("KillCount: " + killCount);
                currentCountdown--;
                yield return new WaitForSeconds(1);
            }
            Debug.Log("CountDownCoroutine Ended Gameover");
        }

        IEnumerator TransformCountDownCoroutine()
        {
            hydeCooldown += 5;
            hydeCooldown += killCount;
            currentHydeCooldown = hydeCooldown;
            while (currentHydeCooldown > 0)
            {
                Debug.Log("TransformCountDownCoroutine " + currentHydeCooldown);
                currentHydeCooldown--;
                yield return new WaitForSeconds(1);
            }
            Transform();
            Debug.Log("TransformCountDownCoroutine Ended");
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}