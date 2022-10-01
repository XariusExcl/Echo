using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{ 
    public SpriteRenderer zone;
    public LineRenderer line;
    public int speed;

    void Update()
    {
        Vector2 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 zone_center = this.transform.position;
        float distance = Vector2.Distance(mouse_pos, zone_center);
        
        line.SetPosition(0, zone_center);
        if (distance > zone.transform.localScale.x)
        {
            Vector2 diff = new Vector2(mouse_pos.x - this.transform.position.x, mouse_pos.y - this.transform.position.y);
            diff = Vector2.ClampMagnitude(diff, zone.transform.localScale.x);
            Vector2 point = new Vector2(this.transform.position.x + diff.x, this.transform.position.y + diff.y);
            line.SetPosition(1, point);
        }
        else
        {
            line.SetPosition(1, mouse_pos);
        }
        
        if (distance <= zone.transform.localScale.x && Input.GetButtonDown("Fire1"))
        {
            this.transform.position = mouse_pos;
        }
    }
}