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
    public class EnemyEnemyCollision : Simulation.Event<EnemyEnemyCollision>
    {
        public EnemyController enemy;
        public EnemyController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
 
            if (player.Bounds.center.y > 8)
            {
                Schedule<EnemyDeath>().enemy = enemy;
                EnemySpawn enemySpawn = GameObject.FindObjectOfType<EnemySpawn>();
                enemySpawn.enemyCount = enemySpawn.levelIndex - 2;
            }
            else if (enemy.Bounds.center.y > 8)
            {
                Schedule<EnemyDeath>().enemy = player;
                EnemySpawn enemySpawn = GameObject.FindObjectOfType<EnemySpawn>();
                enemySpawn.enemyCount = enemySpawn.levelIndex - 2;
            }
        }
    }
}