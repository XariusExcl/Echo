using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public GameObject radarBlip;
    public GameObject nextPosGo;
    public Collider2D fovTrigger;
    public TorpedoLauncher torpedoLauncher;
    GameObject _nextPosGo;
    PlayerController player;
    float _lastPlayerSeenTime;
    float _lastTorpedoSentTime;
    [Header("DEBUG ZONE")]
    public float _speed = 0.125f;
    public float _turningSpeed = 25f;
    public float _seekChance = 1f;


    new void Start()
    {
        base.Start();
        _nextPosGo = Instantiate(nextPosGo, nextPosition, Quaternion.identity);
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
            transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed * Time.deltaTime
        );
        
        // Move to target position (factor for when angle is big, <25° = 1, >90° = 0)
        transform.position += _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right * Time.deltaTime;
        
        if (Vector2.SqrMagnitude(nextPosition - (Vector2)transform.position) < 0.02f)
        {
            // Move target position forward till next blip, our distance estimate was too short
            nextPosition += 0.1f * (Vector2)transform.right;
        }

        if (_aiMode == AiMode.Chase)
        {
            // Fire a torpedo every 10s when chasing
            if (Time.realtimeSinceStartup - _lastTorpedoSentTime > 10f)
            {
                // Anticipate position
                float _timeToHit = (player.transform.position - transform.position).magnitude / 0.3125f; // Speed of a torpedo / second

                torpedoLauncher.Fire(
                    Vector2.SignedAngle(
                        Vector2.right,
                        player.transform.position - transform.position + (Vector3)(player.Velocity * _timeToHit)
                    )
                );
                _lastTorpedoSentTime = Time.realtimeSinceStartup;
            }
            
            // Lose the player after 20s not seen
            if (Time.realtimeSinceStartup - _lastPlayerSeenTime > 20f)
            {
                _aiMode = AiMode.Seek;
            }
        }

    }

    enum AiMode {
        Seek,
        Chase
    }

    AiMode _aiMode = AiMode.Seek;
    int _wallLayerMask = 1; // 

    void CalculateNextPosition()
    {
        lastPosition = transform.position;

        Vector3 leftRelative = Quaternion.AngleAxis(35f, Vector3.forward) * transform.right;
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftRelative, 2f, _wallLayerMask);
        // Debug.DrawRay(transform.position, leftRelative * 2f, Color.yellow, 10f);

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.right, 2f, _wallLayerMask);
        // Debug.DrawRay(transform.position, transform.right * 2f, Color.red, 10f);

        Vector3 rightRelative = Quaternion.AngleAxis(-35f, Vector3.forward) * transform.right;
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightRelative, 2f, _wallLayerMask);
        // Debug.DrawRay(transform.position, rightRelative * 2f, Color.blue, 10f);

        float _playerAngle = Vector2.SignedAngle(transform.right, player.transform.position - transform.position);

        switch(_aiMode) {
            case AiMode.Seek:
                // Chose to "luckily" orient itself towards player (max 75 degrees so no sus u-turns)
                float _angleBias = (Random.value < _seekChance) ? Mathf.Clamp(_playerAngle, -75f, 75f) : 0f; 

                if (_angleBias == _playerAngle) { Debug.Log("Lucky!");}

                float leftHitDistance = (leftHit.distance == 0f ) ? 2f : leftHit.distance;
                float forwardHitDistance = (forwardHit.distance == 0f ) ? 2f : forwardHit.distance;
                float rightHitDistance = (rightHit.distance == 0f ) ? 2f : rightHit.distance;
                
                if (!(leftHitDistance == forwardHitDistance && forwardHitDistance == rightHitDistance))
                {
                    // One of the raycasts hit something
                    if (leftHitDistance < rightHitDistance)
                    {
                        // LeftHit is smaller
                        if (Mathf.Min(forwardHitDistance, leftHitDistance) < 1f)
                        {
                            // Wall is close, turn right harder
                            nextPosition = TryNextTargetPosition(-90f + _angleBias, -35f + _angleBias);
                        } else {
                            // Wall is near, turn right softly
                            nextPosition = TryNextTargetPosition(-35f + _angleBias, 0f + _angleBias);
                        }
                    } else {
                        // RightHit is smaller
                        if (Mathf.Min(forwardHitDistance, rightHitDistance) < 1f)
                        {
                            // Wall is close, turn left harder
                            nextPosition = TryNextTargetPosition(-90f + _angleBias, 0f + _angleBias);
                        } else {
                            // Wall is near, turn left softly
                            nextPosition = TryNextTargetPosition(-35f + _angleBias, 0f + _angleBias);
                        }
                    }
                } else {
                    nextPosition = TryNextTargetPosition(-45f + _angleBias, 45f + _angleBias);
                }
            break;
            case AiMode.Chase:
                nextPosition = TryNextTargetPosition(-5f + _playerAngle, 5f + _playerAngle);
            break;
        }

        // Show expected pos at next scan
        _nextPosGo.transform.position = nextPosition; // + random ?
    }

    Vector2 TryNextTargetPosition(float minAzimuth, float maxAzimuth)
    {
        Vector3 candidate;
        int iterationCount = 0;
        do
        {
            candidate = transform.position + Quaternion.AngleAxis(Random.Range(minAzimuth, maxAzimuth), Vector3.forward) * transform.right * (10f * _speed);
            iterationCount++;
        } while (!ValidateNextPosition(candidate) && iterationCount < 10);
        if (iterationCount == 10)
        {
            Debug.LogWarning("BasicEnemy: Iteration Count for TryNextTargetPosition reached 10 without a suitable target position. Seached between " + minAzimuth + " and " + maxAzimuth + ".");
            if (!(-minAzimuth == maxAzimuth && maxAzimuth == 180f))
                candidate = TryNextTargetPosition(Mathf.Clamp(minAzimuth - 30f, -180f, 180f), Mathf.Clamp(maxAzimuth + 30f, -180f, 180f));
            else
                Debug.LogError("BasicEnemy: Something went terribly wrong!");
        }

        return candidate;
    }

    bool ValidateNextPosition(Vector3 nextPositionCandidate)
    {
        RaycastHit2D _hit = Physics2D.Raycast(
            transform.position,
            (nextPositionCandidate * 1.1f) - transform.position
        );
        
        if (_hit.distance < Vector2.Distance(nextPositionCandidate, transform.position))
            return false;

        return true;
    }

    new void RevealItself()
    {
        base.RevealItself();
        Instantiate(radarBlip, transform.position, transform.rotation);
        // Debug.Log("Ship found at " + vfAngle);
        lastRevealTime = Time.realtimeSinceStartup;
        // Sound Effect
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!player.IsDead)
            {
                Debug.Log(gameObject.name + " found Player! Chasing");
                _lastPlayerSeenTime = Time.realtimeSinceStartup;
                _aiMode = AiMode.Chase;
            } else {
                _aiMode = AiMode.Seek;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "FriendlyTorpedo")
        {
            foreach(Collider2D selfCol in GetComponents<Collider2D>())
            {
                Destroy(selfCol);
            }
            DieAnimation();
        }
    }

    public void DieAnimation()
    {
        
    }

    public void Die()
    {
        // Destroy itself and child
        Destroy(nextPosGo);
        Destroy(this.gameObject);
    }
}