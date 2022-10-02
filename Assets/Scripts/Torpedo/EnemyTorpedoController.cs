using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTorpedoController : RadarEnemy
{
    public GameObject radarBlip;
    PlayerController player;
    GameManager gameManager;

    new void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // AlreadyRevealed = true;
    }
    

    float _lastCriticalBlipTime;
    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!AlreadyRevealed)
        {
            if (viewFinderSwiper.Rotation > vfAngle )
            {
                RevealItself();
            }
        }

        float _sqDistanceToPlayer = Vector2.SqrMagnitude(player.transform.position - transform.position);
        if (_sqDistanceToPlayer < 4f)
        {
            if (Time.realtimeSinceStartup - _lastCriticalBlipTime > (Mathf.Clamp(_sqDistanceToPlayer/2f, 1f, 2f)))
            {
                RevealItselfCritical();
                _lastCriticalBlipTime = Time.realtimeSinceStartup;
            }

        }
    }
    
    new void RevealItself()
    {
        base.RevealItself();
        Instantiate(radarBlip, transform.position, transform.rotation);
        lastRevealTime = Time.realtimeSinceStartup;
    }

    void RevealItselfCritical()
    {

        base.RevealItself();
        GameObject newRadarBlip = Instantiate(radarBlip, transform.position, transform.rotation);
        RadarBlipBehaviour rbb = newRadarBlip.GetComponent<RadarBlipBehaviour>();
        rbb.IsCritical = true;
        rbb.color = Color.red;
    }
}
