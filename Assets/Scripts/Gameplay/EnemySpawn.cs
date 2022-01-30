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
    public int minX = 0;
    public int maxX = 24;
    public int minY = 10;
    public int maxY = 41;
    public int indexDividor = 20;
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
            if (enemyCount < (levelIndex / indexDividor))
            {
                lastxPos = xPos;
                lastyPos = yPos;
                xPos = Random.Range(minX, maxX);
                yPos = Random.Range(minY, maxY);
                if (xPos == lastxPos || yPos == lastyPos)
                {
                    xPos = Random.Range(minX, maxX);
                    yPos = Random.Range(minY, maxY);
                }

                Instantiate(theEnemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
                enemyCount += 1;
                levelIndex += 2;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
