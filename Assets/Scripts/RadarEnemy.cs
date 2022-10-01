using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarEnemy : MonoBehaviour
{
    protected ViewFinderSwiper viewFinderSwiper;
    protected Vector3 lastPosition;
    protected Vector3 nextPosition;

    protected float vfAngle;
    protected float lastRevealTime;
    
    bool _alreadyRevealed = false;
    float _spawnTime;
    public bool AlreadyRevealed {get => _alreadyRevealed; set { _alreadyRevealed = value;}}

    protected void Start()
    {
        viewFinderSwiper = GameObject.FindWithTag("ViewFinderSwiper").GetComponent<ViewFinderSwiper>();
        nextPosition = transform.position;
        _spawnTime = Time.realtimeSinceStartup;
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = nextPosition;
        vfAngle = Vector2.SignedAngle((Vector2)transform.position, Vector2.up);
        // Make angle go from -180->180 to 0->360
        if (vfAngle < 0f)
        {
            vfAngle = 360f + vfAngle;
        }
    } 

    protected void RevealItself()
    {
        _alreadyRevealed = true;
    }
}