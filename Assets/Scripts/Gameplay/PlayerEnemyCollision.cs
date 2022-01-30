using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;

            if (willHurtEnemy)
            {
                var enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemy.enemyAnimator.SetTrigger("hurt");
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                        player.Bounce(2);
                    }
                    else
                    {
                        player.Bounce(7);
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                    player.Bounce(2);
                }
            }
            else
            {
                
                var playerHealth = player.GetComponent<Health>();
                Debug.Log("currentHp: " + playerHealth.currentHP + " MaxHP: " + playerHealth.maxHP);
                if (playerHealth != null)
                {
                    player.animator.SetTrigger("hurt");
                    playerHealth.Decrement();
                    Debug.Log("IsAlive: " + playerHealth.IsAlive);
                    if (playerHealth.LowHealth && !player.transformed)
                    {
                        player.Transform();
                    }
                    if (!playerHealth.IsAlive)
                    {
                        Schedule<PlayerDeath>();
                    }
                }
                else
                {
                    Schedule<PlayerDeath>();
                }
            }
        }
    }
}