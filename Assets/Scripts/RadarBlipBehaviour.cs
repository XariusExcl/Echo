using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarBlipBehaviour : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color color;
    public bool IsCritical = false;
 
    float _spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        _spawnTime = Time.realtimeSinceStartup;
        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        // Value from 1f -> 0 representing time since last blip
        float lifespan = 1f - ((Time.realtimeSinceStartup - _spawnTime) / 10f);

        if (lifespan < 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
