using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ViewFinderSwiper viewFinderSwiper;
    RadarEnemy[] radarEnemies;

    void Start()
    {
        Application.targetFrameRate = 60;
        radarEnemies = GameObject.FindObjectsOfType<RadarEnemy>();
    }

    float m_TimeElapsed;
    float m_loopTimeElapsed;
    void Update()
    {
        m_TimeElapsed += Time.deltaTime;
        m_loopTimeElapsed += Time.deltaTime;
        if (m_loopTimeElapsed > 3)
        {
            // Reset AlreadyRevealed de tt le monde
            foreach(RadarEnemy radarEnemy in radarEnemies)
            {
                radarEnemy.AlreadyRevealed = false;
            }

            viewFinderSwiper.Swipe();
            m_loopTimeElapsed = 0f;
            Debug.Log("Swipe");
        }
    }
}