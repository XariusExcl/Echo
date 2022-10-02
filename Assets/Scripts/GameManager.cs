using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIDriver uiDriver;
    public ViewFinderSwiper viewFinderSwiper;
    public GameObject playerActions;
    public EnemySpawner enemySpawner;
    RadarEnemy[] radarEnemies;
    CameraController cameraController;

    [Header("DEBUG ZONE")]
    public float timeScale = 1f;
    public float loopTime = 10f;
    public float startTimerAt = 7f;

    void Start()
    {
        Application.targetFrameRate = 60;
        radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
        Time.timeScale = timeScale;
        _loopTimeElapsed = startTimerAt;
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    float _timeElapsed;
    float _loopTimeElapsed;
    float _spawnOdds;
    void Update()
    {
        _timeElapsed += Time.deltaTime;
        _loopTimeElapsed += Time.deltaTime;
        if (_loopTimeElapsed > loopTime)
        {
            radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
            
            // Calculate odds to spawn a new enemy 
            if (radarEnemies.Length == 0)
                _spawnOdds = 1.0f;
            else 
                _spawnOdds = 0.3f + (_timeElapsed / 220f); // at 2:30 of playtime, spawn odds are 100%.

            // Spawn (if odds >100%, spawns an enemy and roll odds on remaining %).
            for(int i = 0; i < Mathf.FloorToInt(_spawnOdds); i++)
                enemySpawner.SpawnEnemy();

            if (Random.value < _spawnOdds % 1)
                enemySpawner.SpawnEnemy();

            // Reset AlreadyRevealed for next swipe
            foreach(RadarEnemy radarEnemy in radarEnemies)
            {
                radarEnemy.AlreadyRevealed = false;
            }

            viewFinderSwiper.Swipe();
            _loopTimeElapsed = 0f;
            
            playerActions.GetComponent<PlayerActions>().EnableInteractivity();
    
            Debug.Log($"Swipe, current odds: {_spawnOdds}");
        }


        UpdateUI();
    }

    public void EnemyKilled()
    {
        Debug.Log("Enemy Killed!");
    }

    public void GameOver()
    {
        StartCoroutine(cameraController.CameraShake(0.5f));
        Debug.Log("GameManager: Game over");
    }

    void UpdateUI()
    {
        uiDriver.TimeTillNextScan = loopTime - _loopTimeElapsed;
    }
}