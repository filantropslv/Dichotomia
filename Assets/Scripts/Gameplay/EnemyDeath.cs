using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            enemy._collider.isTrigger = true;
            
            enemy.EnemyDeathAnimation();
            EnemySpawn enemySpawn = GameObject.FindObjectOfType<EnemySpawn>();
            enemySpawn.enemyCount = enemySpawn.enemyCount - 1;
            if (enemy._audio && enemy.death)
            {
                enemy._audio.PlayOneShot(enemy.death);
            }
        }
    }
}