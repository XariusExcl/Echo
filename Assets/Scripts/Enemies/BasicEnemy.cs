using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public GameObject radarBlip;
    public GameObject nextPosGo;
    GameObject _nextPosGo;
    float _speed = 0.002f;
    float _turningSpeed = 0.5f;

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
                /// CalculateNextPosition();
            }
        }

        
        // Angle to target position
        float _angleToTarget = Vector2.SignedAngle(transform.right, nextPosition - (Vector2)transform.position);
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed
        );
        
        // Move to target position (slower factor for when angle is big, <25° = 1, >90° = 0)
        transform.position += _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right;
        
        if (Vector2.SqrMagnitude(nextPosition - (Vector2)transform.position) < 0.05f)
        {
            CalculateNextPosition();
        }
    }

    void CalculateNextPosition()
    {
        lastPosition = transform.position;
        nextPosition = lastPosition + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
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