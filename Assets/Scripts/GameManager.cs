using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIDriver uiDriver;
    public ViewFinderSwiper viewFinderSwiper;
    public ToggleGroup playerActions;
    RadarEnemy[] radarEnemies;

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
    }

    float _timeElapsed;
    float _loopTimeElapsed;
    void Update()
    {
        _timeElapsed += Time.deltaTime;
        _loopTimeElapsed += Time.deltaTime;
        if (_loopTimeElapsed > loopTime)
        {
            // Reset AlreadyRevealed de tt le monde
            radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
            foreach(RadarEnemy radarEnemy in radarEnemies)
            {
                radarEnemy.AlreadyRevealed = false;
            }

            viewFinderSwiper.Swipe();
            _loopTimeElapsed = 0f;
            
            // User actions visibiliy
            
            playerActions.gameObject.SetActive(true);
    
            Debug.Log("Swipe");
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        uiDriver.TimeTillNextScan = loopTime - _loopTimeElapsed;
    }
}