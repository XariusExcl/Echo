using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public UIDriver uiDriver;
    public ViewFinderSwiper viewFinderSwiper;
    public PlayerActions playerActions;
    public EnemySpawner enemySpawner;
    RadarEnemy[] radarEnemies;
    CameraController cameraController;
    PlayerController player;
    public Light2D globalLight;
    [Header("Ambient Colors")]
    public Color normalColor = new Color(0.85f, 0.86f, 0.89f);
    public Color alarmColor = new Color(0.88f, 0.64f, 0.63f);

    [Header("DEBUG ZONE")]
    public float timeScale = 1f;
    public float loopTime = 10f;
    public float startTimerAt = 7f;

    int _enemiesKilled;
    float _timeElapsed;
    float _loopTimeElapsed;
    float _spawnOdds;
    bool _isGameOver = false;
    List<EnemyTorpedoController> _etcList;

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void Start()
    {
        radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
        Time.timeScale = timeScale;
        _loopTimeElapsed = startTimerAt;
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _etcList = new List<EnemyTorpedoController>();
        globalLight.color = new Color(0.85f, 0.86f, 0.89f);
    }


    void Update()
    {
        if (!_isGameOver)
        {
            _timeElapsed += Time.deltaTime;
            _loopTimeElapsed += Time.deltaTime;
            if (_loopTimeElapsed > loopTime)
            {
                radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
                Debug.Log(radarEnemies.Length);
                
                // Calculate odds to spawn a new enemy 
                if (radarEnemies.Length > Mathf.Min(1 + _timeElapsed / 60f, 3.99f)) // Max 4 Enemies on screen.
                    _spawnOdds = 0f;
                else if (radarEnemies.Length < _timeElapsed / 60f ) 
                    _spawnOdds = 1f;
                else
                    _spawnOdds = 0.5f;

                // Spawn
                for(int i = 0; i < Mathf.FloorToInt(_spawnOdds); i++)
                    enemySpawner.SpawnEnemy();

                // Reset AlreadyRevealed for next swipe
                foreach(RadarEnemy radarEnemy in radarEnemies)
                {
                    radarEnemy.AlreadyRevealed = false;
                }

                viewFinderSwiper.Swipe();
                _loopTimeElapsed = 0f;
                
                playerActions.EnableInteractivity();
        
                Debug.Log($"Swipe, current odds: {_spawnOdds}");
            }

            UpdateUI();
        }

    }

    public void WarnCritical(EnemyTorpedoController etc)
    {
        if (!_etcList.Contains(etc))
            _etcList.Add(etc);

    }

    public void RemoveWarnCritical(EnemyTorpedoController etc)
    {
        _etcList.Remove(etc);
        if (_etcList.Count == 0)
        {
            uiDriver.TurnAlarmOff();
        }
    }

    public void EnemyKilled()
    {
        Debug.Log("Enemy Killed!");
        _enemiesKilled++;
    }

    public void GameOver()
    {
        StartCoroutine(cameraController.CameraShake(0.5f));
        Debug.Log("GameManager: Game over");
        _isGameOver = true;
        uiDriver.TurnAlarmSoundOff();

        StartCoroutine(ShowRetryPromptDelayed());

        // Show enemies in clear ?
    }

    public IEnumerator ShowRetryPromptDelayed()
    {
        // Format Time
        TimeSpan time = TimeSpan.FromSeconds(_timeElapsed);
        yield return new WaitForSeconds(0.7f);
        uiDriver.ShowRetryPrompt($"Enemies Killed:\n{_enemiesKilled} \nTime survived: \n{time.ToString(@"mm\:ss\.ff")}");
    }

    void UpdateUI()
    {
        uiDriver.TimeTillNextScan = loopTime - _loopTimeElapsed;

        Vector2 playerPos = player.transform.position;
        bool _keepAlarmOn = false;      
        // Check if alarm should be turned on or off.
        foreach(EnemyTorpedoController etc in _etcList)
        {
            if (etc == null)
                break;

            if(!uiDriver.IsAlarmOn && Vector2.SqrMagnitude((Vector2)etc.transform.position - playerPos) < 2f)
            {
                uiDriver.TurnAlarmOn();
                globalLight.color = alarmColor;
                _keepAlarmOn = true;
                break;
            }

            if(uiDriver.IsAlarmOn && Vector2.SqrMagnitude((Vector2)etc.transform.position - playerPos) < 2f)
            {
                _keepAlarmOn = true;
                break;
            }
        }

        // Remove orphans 
        _etcList.FindAll(etc => etc == null).ForEach(etc => RemoveWarnCritical(etc));
        
        // Remove if too far
        _etcList.FindAll(etc => Vector2.SqrMagnitude((Vector2)etc.transform.position - playerPos) > 4f).ForEach(etc => RemoveWarnCritical(etc));

        if (uiDriver.IsAlarmOn && !_keepAlarmOn)
        {
            uiDriver.TurnAlarmOff();
            globalLight.color = normalColor;
        }
    }
}