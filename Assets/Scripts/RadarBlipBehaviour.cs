using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarBlipBehaviour : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
 
    float _spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>();
        _spawnTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        // Value from 1f -> 0 representing time since last blip
        float lifespan = 1f - ((Time.realtimeSinceStartup - _spawnTime) / 10f);
        spriteRenderer.color = new Color(1f, 1f, 1f, lifespan);

        if (lifespan < 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
