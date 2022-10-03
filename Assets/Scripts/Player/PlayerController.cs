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
    public SpriteRenderer zone;
    public LineRenderer line;
    public GameObject explosion;
    new Rigidbody2D rigidbody2D;
    float _speed = 0.125f;
    float _turningSpeed = 25f;
    private float _angle;
    private Vector2 _nextPosition;
    public TorpedoLauncher torpedoLauncher;
    public ToggleGroup playerActions;
    bool _isDead;
    public bool isMuted;
    public bool IsDead {get => _isDead; private set { _isDead = value;}}
    public Vector2 Velocity {get => rigidbody2D.velocity;}
    GameManager _gameManager;
    public AudioSource moveSfx;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        _isDead = false;
    }

    void Update()
    {
        if (!_isDead)
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 _zoneCenter = this.transform.position;

            // Line between submarine and pointer

            float distance = Vector2.Distance(_mousePos, _zoneCenter);
            line.SetPosition(0, _zoneCenter);
            if (distance > zone.transform.localScale.x)
            {
                if (getActiveToggle() == "Move")
                {
                    Vector2 _diff = new Vector2(_mousePos.x - transform.position.x, _mousePos.y - transform.position.y);
                    _diff = Vector2.ClampMagnitude(_diff, zone.transform.localScale.x);
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


            // If pointer is inside zone - rotate submarine to pointer and move

            if (getActiveToggle() == "Move" && distance <= zone.transform.localScale.x && Input.GetButtonDown("Fire1") &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                moveSfx.Play();
                _nextPosition = _mousePos;
                playerActions.GetComponent<PlayerActions>().DisableInteractivity();
            }

            if (getActiveToggle() == "Mute")
            {
                isMuted = true;
                playerActions.GetComponent<PlayerActions>().DisableInteractivity();
                playerActions.GetComponent<PlayerActions>().ResetDefault();
            }

            // move submarine to pointer
            
            float distanceToNextPosition =
                Vector2.Distance(_nextPosition, new Vector2(transform.position.x, transform.position.y));

            if (distanceToNextPosition > 0.1f)
            {
                float _angleToTarget = Vector2.SignedAngle(transform.right, _nextPosition - (Vector2)transform.position);

                transform.rotation = Quaternion.Euler(
                    0f, 
                    0f,
                    transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed * Time.deltaTime
                );

                rigidbody2D.velocity = _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right;
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "EnemyTorpedo")
        {
            Debug.Log("Player ouchie");
            Destroy(GetComponent<Collider2D>());
            _isDead = true;
            rigidbody2D.velocity = new Vector2(0f, 0f);
            _gameManager.GameOver();
            DieAnimation();
        }
    }

    public void DieAnimation()
    {
        
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
} 