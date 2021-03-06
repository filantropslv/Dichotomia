using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player killed an enemy
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class PlayerMeleeEnemy : Simulation.Event<PlayerMeleeEnemy>
    {
        public EnemyController enemy;
        public PlayerController player;
        public override void Execute()
        {
            if (enemy != null && !enemy.isDead)
            { 
                var enemyHealth = enemy.GetComponent<Health>();
                player.killCount += 1;
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = enemy;
                    } else
                    {
                        enemy.enemyAnimator.SetTrigger("hurt");
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                }
            }
        }
    }
}