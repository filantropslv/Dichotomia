using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject theEnemy;
    public int xPos;
    public int yPos;
    public int enemyCount;
    public int levelIndex = 100;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
        while (true)
        {
            if (enemyCount < (int)(levelIndex / 20))
            {
                xPos = Random.Range(-3, 7);
                yPos = Random.Range(1, 8);

                Instantiate(theEnemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
                enemyCount += 1;
                levelIndex += 2;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
