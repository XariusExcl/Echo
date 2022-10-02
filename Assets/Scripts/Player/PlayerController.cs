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
    float _speed = 0.125f;
    float _turningSpeed = 25f;
    private float _angle;
    private Vector2 _nextPosition;
    public TorpedoLauncher torpedoLauncher;
    public ToggleGroup playerActions;

    void Update()
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
        
        if (getActiveToggle() == "Move" && distance <= zone.transform.localScale.x && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            _nextPosition = _mousePos;
            playerActions.gameObject.SetActive(false);
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
            transform.position += _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right * Time.deltaTime;;
        }
        
        // If pointer is outside zone - shoot torpedo
        
        if (getActiveToggle() == "Shoot"  && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            torpedoLauncher.Fire(Vector2.SignedAngle(Vector2.right, _mousePos - (Vector2)transform.position));
            playerActions.gameObject.SetActive(false);
        }
    } 
    
    public string getActiveToggle()
    {
        if (playerActions.gameObject.activeSelf)
        {
            return playerActions.ActiveToggles().First().name;
        }
        return "";
    }
} 