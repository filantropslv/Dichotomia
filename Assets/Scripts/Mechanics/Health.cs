using System;
using System.Collections;
using Platformer.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        /// <summary>
        /// The maximum hit points for the entity.
        /// </summary>
        public int maxHP = 2;
        public int currentHP;

        /// <summary>
        /// Indicates if the entity should be considered 'alive'.
        /// </summary>
        public bool IsAlive => currentHP > 0;
        public bool LowHealth => currentHP == 1;

        public bool isRegenerating = false;

        public Coroutine regenCoroutine;
        public Coroutine stressCoroutine;

        public Button[] healthBar;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment()
        {
            currentHP = Mathf.Clamp(currentHP + 1, 0, maxHP);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement()
        {
            currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
        }

        public void DecrementByValue(int value)
        {
            currentHP = Mathf.Clamp(currentHP - value, 0, maxHP);
        }


        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Die()
        {
            while (currentHP > 0) Decrement();
        }

        public void ToggleRegen()
        {
            if (regenCoroutine == null)
            {
                regenCoroutine = StartCoroutine(RegenerateCoroutine());
            }
        }

        void Awake()
        {
            currentHP = maxHP;
        }

        private void Update()
        {
            switch (currentHP)
            {
                case 0:
                    healthBar[0].gameObject.SetActive(false);
                    healthBar[1].gameObject.SetActive(false);
                    healthBar[2].gameObject.SetActive(false);
                    healthBar[3].gameObject.SetActive(false);
                    healthBar[10].gameObject.SetActive(false);
                    healthBar[11].gameObject.SetActive(false);

                    break;
                case 1:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(false);
                    healthBar[2].gameObject.SetActive(false);
                    healthBar[3].gameObject.SetActive(false);
                    healthBar[10].gameObject.SetActive(false);
                    healthBar[11].gameObject.SetActive(false);
                    break;
                case 2:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(true);
                    healthBar[2].gameObject.SetActive(false);
                    healthBar[3].gameObject.SetActive(false);
                    healthBar[10].gameObject.SetActive(false);
                    healthBar[11].gameObject.SetActive(false);
                    break;
                case 3:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(true);
                    healthBar[2].gameObject.SetActive(true);
                    healthBar[3].gameObject.SetActive(false);
                    healthBar[10].gameObject.SetActive(false);
                    healthBar[11].gameObject.SetActive(false);
                    break;
                case 4:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(true);
                    healthBar[2].gameObject.SetActive(true);
                    healthBar[3].gameObject.SetActive(true);
                    healthBar[10].gameObject.SetActive(false);
                    healthBar[11].gameObject.SetActive(false);
                    break;
                case 5:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(true);
                    healthBar[2].gameObject.SetActive(true);
                    healthBar[3].gameObject.SetActive(true);
                    healthBar[10].gameObject.SetActive(true);
                    healthBar[11].gameObject.SetActive(false);
                    break;
                case 6:
                    healthBar[0].gameObject.SetActive(true);
                    healthBar[1].gameObject.SetActive(true);
                    healthBar[2].gameObject.SetActive(true);
                    healthBar[3].gameObject.SetActive(true);
                    healthBar[10].gameObject.SetActive(true);
                    healthBar[11].gameObject.SetActive(true);
                    break;
            }
            
        }
        IEnumerator RegenerateCoroutine()
        {
            while (true)
            {
                if (isRegenerating)
                {
                    Increment();
                }
                yield return new WaitForSeconds(1.5f);

            }
        }
    }
}
