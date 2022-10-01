using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarEnemy : MonoBehaviour
{
    protected ViewFinderSwiper viewFinderSwiper;

    protected float m_angle;
    bool m_alreadyRevealed = false;
    public bool AlreadyRevealed {get => m_alreadyRevealed; set { m_alreadyRevealed = value;}}

    protected void Start()
    {
        viewFinderSwiper = GameObject.FindWithTag("ViewFinderSwiper").GetComponent<ViewFinderSwiper>();
        UpdatePosition(transform.position);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position;
        m_angle = Vector2.SignedAngle((Vector2)transform.position, Vector2.up);
        // Make angle go from -180->180 to 0->360
        if (m_angle < 0f)
        {
            m_angle = 360f + m_angle;
        }
        Debug.Log(m_angle);
    } 

    protected void RevealItself()
    {
        m_alreadyRevealed = true;
    }
}