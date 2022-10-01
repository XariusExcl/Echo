using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public GameObject radarBlip;
    public GameObject nextPosGo;
    GameObject _nextPosGo;
    public GameObject wave;

    new void Start()
    {
        base.Start();
        _nextPosGo = Instantiate(nextPosGo, nextPosition, Quaternion.identity);
    }

    void Update()
    {
        if (!AlreadyRevealed)
        {
            // If swiper has passed enemy ship
            if (viewFinderSwiper.Rotation > vfAngle)
            {
                RevealItself();
                CalculateNextPosition();
            }
        }

        transform.position = Vector3.Lerp(lastPosition, nextPosition, (Time.realtimeSinceStartup - lastRevealTime) / 10f);
    }

    void CalculateNextPosition()
    {
        lastPosition = transform.position;
        nextPosition = transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        _nextPosGo.transform.position = nextPosition; // + random ?
    }


    new void RevealItself()
    {
        base.RevealItself();
        Instantiate(radarBlip, transform.position, Quaternion.identity);
        Debug.Log("Ship found at " + vfAngle);
        lastRevealTime = Time.realtimeSinceStartup;
        // Spawn wave
        // Sound Effect
    }

    void Kill()
    {
        // Destroy itself and child
    }
}