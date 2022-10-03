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

    List<EnemyTorpedoController> _etcList;

    void Start()
    {
        Application.targetFrameRate = 60;
        radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
        Time.timeScale = timeScale;
        _loopTimeElapsed = startTimerAt;
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _etcList = new List<EnemyTorpedoController>();
        globalLight.color = new Color(0.85f, 0.86f, 0.89f);
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
            if (radarEnemies.Length < _timeElapsed / 40f )
                _spawnOdds = 1.0f;
            else if (radarEnemies.Length > 1 + _timeElapsed / 40f )
                _spawnOdds = 0f;
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
    }

    public void GameOver()
    {
        StartCoroutine(cameraController.CameraShake(0.5f));
        Debug.Log("GameManager: Game over");
    }

    void UpdateUI()
    {
        uiDriver.TimeTillNextScan = loopTime - _loopTimeElapsed;

        Vector2 playerPos = player.transform.position;
        bool _keepAlarmOn = false;      
        // Check if alarm should be turned on or off.
        foreach(EnemyTorpedoController etc in _etcList)
        {
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

        List<EnemyTorpedoController> etcTooFar = _etcList.FindAll(etc => Vector2.SqrMagnitude((Vector2)etc.transform.position - playerPos) > 4f);
        foreach (EnemyTorpedoController etc in  etcTooFar)
        {
            RemoveWarnCritical(etc);
        }

        if (uiDriver.IsAlarmOn && !_keepAlarmOn)
        {
            uiDriver.TurnAlarmOff();
            globalLight.color = normalColor;
        }
    }
}