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
        CalculateNextPosition();
    }

    void Update()
    {
        base.Update();
        
        if (!AlreadyRevealed)
        {
            // If swiper has passed enemy ship
            if (viewFinderSwiper.Rotation > vfAngle)
            {
                RevealItself();
                CalculateNextPosition();
            }
        }

        transform.position = Vector2.Lerp(lastPosition, nextPosition, (Time.realtimeSinceStartup - lastRevealTime) / 10f);
    }

    void CalculateNextPosition()
    {
        lastPosition = transform.position;
        nextPosition = lastPosition + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        // Calculate angle to face next position
        float facingAngle = Vector2.Angle(Vector2.right, nextPosition - lastPosition);
        Debug.Log(facingAngle);
        Quaternion facingAngleQuat = Quaternion.Euler(0f, 0f, facingAngle);
        transform.rotation = facingAngleQuat;
        
        // Show expected pos at next scan
        _nextPosGo.transform.position = nextPosition; // + random ?
    }


    new void RevealItself()
    {
        base.RevealItself();
        Instantiate(radarBlip, transform.position, transform.rotation);
        Debug.Log("Ship found at " + vfAngle);
        lastRevealTime = Time.realtimeSinceStartup;
        // Sound Effect
    }

    void Kill()
    {
        // Destroy itself and child
        Destroy(nextPosGo);
        Destroy(this.gameObject);
    }
}