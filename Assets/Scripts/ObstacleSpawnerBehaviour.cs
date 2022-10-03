using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerBehaviour : MonoBehaviour
{
    public GameObject[] obstacles;
    public int spawnAmount;
    int _obstaclesLength;
    List<Vector2> _spawnPoints;

    void Start()
    {
        _obstaclesLength = obstacles.Length;
        _spawnPoints = new List<Vector2>();
        SpawnObstacles(spawnAmount);
    }


    public void SpawnObstacles(int amount)
    {
        for (int i = 1; i <= amount; i++)
        {
            Vector2 spawnPoint;
            int iterationCounter = 0;
            do {
                spawnPoint = (Vector2)transform.position + Random.insideUnitCircle.normalized * 2f + new Vector2(Random.Range(-0.7f, 0.7f),Random.Range(-0.7f, 0.7f));
                iterationCounter++;
            } while (_spawnPoints.FindAll(sp => Vector2.SqrMagnitude(spawnPoint - sp) < 8f).Count > 0 && iterationCounter < 20);
            if (iterationCounter == 20)
                Debug.LogWarning("Could not find a suitable position in 20 iterations.");

            Instantiate(
                obstacles[Random.Range(0, _obstaclesLength - 1)],
                spawnPoint,
                Quaternion.Euler(0f, 0f, 90f*(int)(Random.value*4f))
            );
            
            _spawnPoints.Add(spawnPoint);
        }
    }  
}
