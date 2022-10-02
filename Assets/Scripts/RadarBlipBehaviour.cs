using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarBlipBehaviour : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color color;
    public GameObject trajectory;
    bool _isCritical;
    public bool IsCritical {get => _isCritical; set{ _isCritical = value;}}
    float _lifespan;

    float _spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        _spawnTime = Time.realtimeSinceStartup;
        spriteRenderer.color = color;
        if (_isCritical)
            trajectory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Value from 1f -> 0 representing time since last blip
        _lifespan = 1f - ((Time.realtimeSinceStartup - _spawnTime) / (_isCritical ? 1f : 10f));

        if (_lifespan < 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
