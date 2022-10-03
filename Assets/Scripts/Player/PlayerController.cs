using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform zone;
    public LineRenderer line;
    public GameObject sprite;
    public GameObject explosion;
    new Rigidbody2D rigidbody2D;
    public Collider2D counterMeasure;
    public TorpedoLauncher torpedoLauncher;
    public ToggleGroup playerActions;
    public AudioSource moveSfx;

    [Header("DEBUG ZONE")]
    public float _speed = 0.125f;
    public float _turningSpeed = 25f;

    public Vector2 Velocity {get => rigidbody2D.velocity;}
    bool _isDead;
    public bool IsDead {get => _isDead; private set { _isDead = value;}}
    bool _isMuted;
    bool _isCountermeasureReady;
    public bool IsCountermeasureReady { get => _isCountermeasureReady; }
    public float CountermeasureProgress;
    
    GameManager _gameManager;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _isDead = false;
        _isCountermeasureReady = true;
        CountermeasureProgress = 1.0f;
    }

    private Vector2 _nextPosition;
    float _timeSinceLastCounterMeasure;
    void Update()
    {
        if (!_isDead)
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 _zoneCenter = this.transform.position;

            // Line between submarine and pointer
            float distance = Vector2.Distance(_mousePos, _zoneCenter);
            line.SetPosition(0, _zoneCenter);
            if (distance > zone.localScale.x)
            {
                if (getActiveToggle() == "Move")
                {
                    Vector2 _diff = new Vector2(_mousePos.x - transform.position.x, _mousePos.y - transform.position.y);
                    _diff = Vector2.ClampMagnitude(_diff, zone.localScale.x);
                    Vector2 point = new Vector2(transform.position.x + _diff.x, transform.position.y + _diff.y);
                    line.SetPosition(1, point);
                }

                if (getActiveToggle() == "Shoot")
                {
                    line.SetPosition(1, _mousePos);
                }
            }
            else
            {
                line.SetPosition(1, _mousePos);
            }

            // Countermeasure OGCD
            if(!_isCountermeasureReady)
            {
                if (!counterMeasure.enabled)
                {
                    CountermeasureProgress += Time.deltaTime / 24f;
                    if (CountermeasureProgress > 1f)
                    {
                        CountermeasureProgress = 1f;
                        _isCountermeasureReady = true;
                    }
                } else {
                    CountermeasureProgress = 1f - ((Time.realtimeSinceStartup - _timeSinceLastCounterMeasure)/4f);
                }
            }

            // If pointer is inside zone - rotate submarine to pointer and move
            if (getActiveToggle() == "Move" && distance <= zone.localScale.x && Input.GetButtonDown("Fire1") &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                moveSfx.Play();
                _nextPosition = _mousePos;
                playerActions.GetComponent<PlayerActions>().DisableInteractivity();
            }

            if (getActiveToggle() == "Mute")
            {
                _isMuted = true;
                playerActions.GetComponent<PlayerActions>().DisableInteractivity();
                playerActions.GetComponent<PlayerActions>().ResetDefault();
            }

            // Move submarine to next position
            float sqrDistanceToNextPosition = Vector2.SqrMagnitude(_nextPosition - (Vector2)transform.position);

            if (sqrDistanceToNextPosition > 0.02f)
            {
                float _angleToTarget = Vector2.SignedAngle(transform.right, _nextPosition - (Vector2)transform.position);

                transform.rotation = Quaternion.Euler(
                    0f, 
                    0f,
                    transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed * Time.deltaTime
                );

                rigidbody2D.velocity = _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right;
            } else {
                // Move target position forward to keep moving
                _nextPosition += 0.1f * (Vector2)transform.right;
            }
            
            // If pointer is outside zone - shoot torpedo
            if (getActiveToggle() == "Shoot"  && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
            {
                torpedoLauncher.Fire(Vector2.SignedAngle(Vector2.right, _mousePos - (Vector2)transform.position));
                playerActions.GetComponent<PlayerActions>().DisableInteractivity();
            }
        }
    } 
    
    public string getActiveToggle()
    {
        if (!playerActions.GetComponent<PlayerActions>().isDisabled)
        {
            return playerActions.ActiveToggles().First().name;
        }
        return "";
    }

    public IEnumerator DeployCountermeasure()
    {
        _timeSinceLastCounterMeasure = Time.realtimeSinceStartup;
        _isCountermeasureReady = false;
        counterMeasure.enabled = true;
        yield return new WaitForSecondsRealtime(4f);
        counterMeasure.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"TriggerEnter {col.gameObject.tag}");
        GameObject colGo = col.gameObject;
        if (colGo.tag == "EnemyTorpedo")
        {
            colGo.GetComponent<TorpedoController>().Explosion();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "EnemyTorpedo")
        {
            Debug.Log("Player ouchie");
            Destroy(GetComponent<Collider2D>());
            Destroy(sprite);
            _isDead = true;
            rigidbody2D.velocity = new Vector2(0f, 0f);
            _gameManager.GameOver();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
} 