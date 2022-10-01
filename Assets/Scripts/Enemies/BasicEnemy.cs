using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : RadarEnemy
{
    public SpriteRenderer spriteRenderer;
    public GameObject wave;

    new void Start()
    {
        base.Start();
        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (!AlreadyRevealed)
        {
            // If swiper has passed enemy ship
            Debug.Log(viewFinderSwiper.Rotation + " " + m_angle);
            if (viewFinderSwiper.Rotation > m_angle)
            {
                // reveal it
                RevealItself();
            }
        }
    }

    new void RevealItself()
    {
        base.RevealItself();
        spriteRenderer.enabled = true;
        Debug.Log("Ship found at " + m_angle);
        // Show sprite
        // Spawn wave
        // Sound Effect
    }
}