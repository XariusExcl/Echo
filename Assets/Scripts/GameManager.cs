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
    void Update()
    {
        _timeElapsed += Time.deltaTime;
        _loopTimeElapsed += Time.deltaTime;
        if (_loopTimeElapsed > loopTime)
        {
            if (Random.value < 0.3f)
                enemySpawner.SpawnEnemy();
                
            // Reset AlreadyRevealed de tt le monde
            radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
            foreach(RadarEnemy radarEnemy in radarEnemies)
            {
                radarEnemy.AlreadyRevealed = false;
            }

            viewFinderSwiper.Swipe();
            _loopTimeElapsed = 0f;
            
            playerActions.GetComponent<PlayerActions>().EnableInteractivity();
    
            Debug.Log("Swipe");
        }


        UpdateUI();
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