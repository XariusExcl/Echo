using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public GameObject radarBlip;
    public GameObject nextPosGo;
    public TorpedoLauncher torpedoLauncher;
    GameObject _nextPosGo;
    RadarBlipBehaviour _currentBlip;
    PlayerController player;
    float _lastPlayerSeenTime;
    float _lastTorpedoSentTime;
    [Header("DEBUG ZONE")]
    public float _speed = 0.125f;
    public float _turningSpeed = 25f;
    public float _seekChance = 1f;
    bool _isDead = false;
    public AudioSource explosionSfx;

    GameManager _gameManager;

    new void Start()
    {
        base.Start();
        _nextPosGo = Instantiate(nextPosGo, nextPosition, Quaternion.identity);
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        _nextPosGo.transform.position = new Vector3(-12f, -12f, 0f); // Hide next pos marker until first reveal
    }

    new void Update()
    {
        base.Update();

        if (!AlreadyRevealed && !_isDead)
        {
            // If swiper has passed enemy ship
            if (viewFinderSwiper.Rotation > vfAngle)
            {
                if (Vector2.SqrMagnitude((Vector2)transform.position - new Vector2(0f, 0.5f)) < 16f)
                    RevealItself();

                AlreadyRevealed = true;
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
            // Fire a torpedo every 15s when chasing, if player has been seen for more than 5 seconds.
            if (Time.realtimeSinceStartup - _lastTorpedoSentTime > 15f && Time.realtimeSinceStartup - _lastPlayerSeenTime > 5f)
            {
                if (!player.IsDead)
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
                } else {
                    _aiMode = AiMode.Seek;
                }
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

        Vector3 leftRelative = Quaternion.AngleAxis(40f, Vector3.forward) * transform.right;
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, leftRelative, 2.5f, _wallLayerMask);
        Debug.DrawRay(transform.position, leftRelative * 2f, Color.yellow, 10f);

        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, transform.right, 2.5f, _wallLayerMask);
        Debug.DrawRay(transform.position, transform.right * 2f, Color.red, 10f);

        Vector3 rightRelative = Quaternion.AngleAxis(-40f, Vector3.forward) * transform.right;
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, rightRelative, 2.5f, _wallLayerMask);
        Debug.DrawRay(transform.position, rightRelative * 2f, Color.blue, 10f);

        float _playerAngle = Vector2.SignedAngle(transform.right, player.transform.position - transform.position);

        switch(_aiMode) {
            case AiMode.Seek:
                // Chose to "luckily" orient itself towards player (max 75 degrees so no sus u-turns)
                float _angleBias = (Random.value < _seekChance) ? Mathf.Clamp(_playerAngle, -75f, 75f) : 0f;

                float leftHitDistance = (leftHit.distance == 0f ) ? 2f : leftHit.distance;
                float forwardHitDistance = (forwardHit.distance == 0f ) ? 2f : forwardHit.distance;
                float rightHitDistance = (rightHit.distance == 0f ) ? 2f : rightHit.distance;
                
                if (!(leftHitDistance == forwardHitDistance && forwardHitDistance == rightHitDistance))
                {
                    // One of the raycasts hit something
                    if (leftHitDistance < rightHitDistance)
                    {
                        // LeftHit is smaller
                        if (forwardHitDistance < 1f || leftHitDistance < 1f)
                        {
                            // Wall is close, turn right harder
                            nextPosition = TryNextTargetPosition(-60f + _angleBias);
                        } else {
                            // Wall is near, turn right softly
                            nextPosition = TryNextTargetPosition(-25f + _angleBias);
                        }
                    } else {
                        // RightHit is smaller
                        if (forwardHitDistance < 1f || rightHitDistance < 1f)
                        {
                            // Wall is close, turn left harder
                            nextPosition = TryNextTargetPosition(60f + _angleBias);
                        } else {
                            // Wall is near, turn left softly
                            nextPosition = TryNextTargetPosition(25f + _angleBias);
                        }
                    }
                } else {
                    nextPosition = TryNextTargetPosition(Random.Range(-35f, 35f) + _angleBias);
                }
            break;
            case AiMode.Chase:
                nextPosition = TryNextTargetPosition(Random.Range(-5f, 5f) + _playerAngle);
            break;
        }

        // Show expected pos at next scan
        _nextPosGo.transform.position = nextPosition; // + random ?
        if (_currentBlip is not null)
            _currentBlip.SetLinePositions(transform.position, nextPosition);
    }

    // Checks if direction at azimuth is not going to hit anything, and if it does, sweeps in 20° intervals to check for a valid azimuth.
    Vector2 TryNextTargetPosition(float azimuth)
    {
        Vector3 candidate = new Vector3();
        for (int i = 1; i < 19; i++)
        {
            float newAzimuth = (float)((2*(i%2)-1) * (i/2)) * 20f + azimuth;
            candidate = transform.position + Quaternion.AngleAxis(Random.Range(newAzimuth - 5f, newAzimuth + 5f), Vector3.forward) * transform.right * (10f * _speed);
            if (ValidateNextPosition(candidate))
                return candidate;
        }
        Debug.LogError("BasicEnemy: Something went terribly wrong!");
        return candidate;
        // Todo, store furethest hit and use that?
    }

    bool ValidateNextPosition(Vector3 nextPositionCandidate)
    {
        RaycastHit2D _hit = Physics2D.Raycast(
            transform.position,
            nextPositionCandidate - transform.position,
            (10f * _speed),
            _wallLayerMask
        );
        
        if (_hit.distance > 0f)
            return false;
            
        return true;
    }

    new void RevealItself()
    {
        if (!_isDead)
        {
            base.RevealItself();
            GameObject newBlip = Instantiate(radarBlip, transform.position, transform.rotation);
            _currentBlip = newBlip.GetComponent<RadarBlipBehaviour>();

            // Debug.Log("Ship found at " + vfAngle);
            lastRevealTime = Time.realtimeSinceStartup;
            // Sound Effect
        }
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
            explosionSfx.Play();
            Debug.Log("Basic Enemy ouchie");
            foreach(Collider2D selfCol in GetComponents<Collider2D>())
            {
                Destroy(selfCol);
            }
            _isDead = true;
            _gameManager.EnemyKilled();
            DieAnimation();
        }
    }

    public void DieAnimation()
    {
        // Show itself or whatever
        Destroy(_nextPosGo, 2.9f);
        Destroy(this.gameObject, 3.0f);
    }
}