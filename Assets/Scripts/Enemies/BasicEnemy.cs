using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public GameObject radarBlip;
    public GameObject nextPosGo;
    public Collider2D fovTrigger;
    GameObject _nextPosGo;
    float _speed = 0.002f;
    float _turningSpeed = 0.5f;
    GameObject player;

    new void Start()
    {
        base.Start();
        _nextPosGo = Instantiate(nextPosGo, nextPosition, Quaternion.identity);
        player = GameObject.FindWithTag("Player");
        CalculateNextPosition();
    }

    new void Update()
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

        // Angle to target position
        float _angleToTarget = Vector2.SignedAngle(transform.right, nextPosition - (Vector2)transform.position);
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed
        );
        
        // Move to target position (factor for when angle is big, <25° = 1, >90° = 0)
        transform.position += _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right;
        
        if (Vector2.SqrMagnitude(nextPosition - (Vector2)transform.position) < 0.02f)
        {
            // Move target position forward till next blip, our distance estimate was too short
            nextPosition += 0.1f * (Vector2)transform.right;
        }
    }

    enum AiMode {
        Seek,
        Chase
    }

    AiMode _aiMode = AiMode.Seek;

    void CalculateNextPosition()
    {
        lastPosition = transform.position;

        Vector3 leftRelative = Quaternion.AngleAxis(-35f, Vector3.forward) * transform.right;
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftRelative, 2f);
        Debug.DrawRay(transform.position, leftRelative * 2f, Color.yellow, 10f);

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.right, 2f);
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red, 10f);

        Vector3 rightRelative = Quaternion.AngleAxis(35f, Vector3.forward) * transform.right;
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightRelative, 2f);
        Debug.DrawRay(transform.position, rightRelative * 2f, Color.blue, 10f);

        

        switch(_aiMode) {
            case AiMode.Seek:
                Debug.Log("LeftHit: " + leftHit.distance);
                Debug.Log("ForwardHit: " + forwardHit.distance);
                Debug.Log("RightHit: " + rightHit.distance);
                
            break;
            case AiMode.Chase:

            break;
        }

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

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            // PlayerController pc = col.gameObject.GetComponent<PlayerController>();
            // if (!pc.IsDead)
                Debug.Log(gameObject.name + " found Player! Chasing");
                _aiMode = AiMode.Chase;
            // else
            //    _aiMode = AiMode.Seek;
        }
    }

    void Kill()
    {
        // Destroy itself and child
        Destroy(nextPosGo);
        Destroy(this.gameObject);
    }
}