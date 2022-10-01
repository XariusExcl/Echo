using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer zone;
    public LineRenderer line;
    float _speed = 0.002f;
    float _turningSpeed = 0.5f;
    private float _angle;
    private Vector2 _nextPosition;


    void Update()
    {
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 _zoneCenter = this.transform.position;
        float distance = Vector2.Distance(_mousePos, _zoneCenter);
        line.SetPosition(0, _zoneCenter);

        if (distance > zone.transform.localScale.x)
        {
            Vector2 _diff = new Vector2(_mousePos.x - transform.position.x, _mousePos.y - transform.position.y);
            _diff = Vector2.ClampMagnitude(_diff, zone.transform.localScale.x);
            Vector2 point = new Vector2(transform.position.x + _diff.x, transform.position.y + _diff.y);
            line.SetPosition(1, point);
        }
        else
        {
            line.SetPosition(1, _mousePos);

        }

        if (distance <= zone.transform.localScale.x && Input.GetButtonDown("Fire1"))
        {
            _nextPosition = _mousePos;
        }

        float distanceToNextPosition =
            Vector2.Distance(_nextPosition, new Vector2(transform.position.x, transform.position.y));

        if (distanceToNextPosition > 0.1f)
        {
            float _angleToTarget = Vector2.SignedAngle(transform.right, _nextPosition - (Vector2)transform.position);
            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                transform.rotation.eulerAngles.z + Mathf.Sign(_angleToTarget) * _turningSpeed
            );
            transform.position += _speed * (Mathf.Clamp(-0.015f * Mathf.Abs(_angleToTarget) + 1.38f, 0f, 1f)) * transform.right;
        }
    }
}