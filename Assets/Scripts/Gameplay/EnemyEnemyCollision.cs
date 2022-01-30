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
 
            if (player.Bounds.center.y > enemy.Bounds.center.y)
            {
                Schedule<EnemyDeath>().enemy = enemy;
            }
            else if (player.Bounds.center.y < enemy.Bounds.center.y)
            {
                Schedule<EnemyDeath>().enemy = player;
            }
        }
    }
}