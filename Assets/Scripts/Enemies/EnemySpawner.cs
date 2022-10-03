using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public float spawnRadius;
    int _enemiesLength;

    void Start()
    {
        _enemiesLength = enemies.Length;
    }

    public void SpawnEnemy()
    {
        Vector2 spawnPoint = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(
            enemies[Random.Range(0, _enemiesLength - 1)],
            spawnPoint,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.left, spawnPoint))
        );
    }   
}
