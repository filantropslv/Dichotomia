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
    public int lastxPos;
    public int lastyPos;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    IEnumerator EnemyDrop()
    {
        while (true)
        {
            if (enemyCount < (levelIndex / 10))
            {
                lastxPos = xPos;
                lastyPos = yPos;
                xPos = Random.Range(0, 24);
                yPos = Random.Range(10, 41);
                if (xPos == lastxPos || yPos == lastyPos)
                {
                    xPos = Random.Range(0, 24);
                    yPos = Random.Range(10, 41);
                }

                Instantiate(theEnemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
                enemyCount += 1;
                levelIndex += 2;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
