using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public BattleCharacteristics[] characteristicsArray;
    [Header("Settings")]
    public int scaleMultiplier = 10;
    public float minSpawnDistance = 2;

    public List<Enemy> Enemies { get; private set; }

    private void Awake()
    {
        int enemyCount = characteristicsArray.Length;

        if (enemyCount == 0)
            return;
        
        Enemies = new List<Enemy>(enemyCount);

        Transform platform = transform;
        Vector3 scale = platform.localScale * scaleMultiplier;

        for (int i = 0; i < enemyCount; i++)
        {
            if (characteristicsArray[i].hp <= 0)
                continue;
            
            Vector3 pos = new (Random.Range(-scale.x * 0.5f, scale.x * 0.5f), 
                1, Random.Range(-scale.z * 0.5f, scale.z * 0.5f));
            pos += transform.position;
            
            if (Enemies.Count != 0)
            {
                bool canSpawn = true;
                for (int j = 0; j < Enemies.Count; j++)
                {
                    float distance = (pos - Enemies[j].transform.position).sqrMagnitude;

                    if (distance < minSpawnDistance * minSpawnDistance)
                        canSpawn = false;
                }

                int iterationsCount = 0;
                while (!canSpawn)
                {
                    if (iterationsCount > 50)
                        break;
                    
                    pos = new Vector3(Random.Range(-scale.x * 0.5f, scale.x * 0.5f), 
                        1, Random.Range(-scale.z * 0.5f, scale.z * 0.5f));
                    pos += transform.position;

                    canSpawn = true;
                    for (int j = 0; j < Enemies.Count; j++)
                    {
                        float distance = (pos - Enemies[j].transform.position).sqrMagnitude;

                        if (distance >= minSpawnDistance * minSpawnDistance)
                            continue;
                        
                        canSpawn = false;
                        break;
                    }
                    
                    iterationsCount++;
                }
            }
            
            SpawnEnemy(pos, characteristicsArray[i]);
        }
    }

    private void SpawnEnemy(Vector3 pos, BattleCharacteristics characteristics)
    {
        GameObject enemyGameObject = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemyGameObject.transform.parent = transform;

        if (!enemyGameObject.TryGetComponent(out Enemy enemy))
        {
            Destroy(enemyGameObject);
            return;
        }
                
        enemy.Platform = this;
        enemy.Characteristics = characteristics;
        
        Enemies.Add(enemy);
    }
}
