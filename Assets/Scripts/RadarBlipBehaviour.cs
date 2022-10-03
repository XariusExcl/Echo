using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RadarBlipBehaviour : MonoBehaviour
{
    public Light2D radarSprite;
    public Light2D radarWave;
    public Color color;
    public LineRenderer trajectory;
    bool _isCritical;
    public bool IsCritical {get => _isCritical; set{ _isCritical = value;}}
    float _lifespan;

    float _spawnTime;

    // Start is called before the first frame update
    void Start()
    {
        _spawnTime = Time.realtimeSinceStartup;
        radarSprite.color = color;
        radarWave.color = color;
        if (_isCritical)
            trajectory.gameObject.SetActive(false);
    }

    public void SetLinePositions(Vector3 start, Vector3 end)
    {
        trajectory.SetPositions(new Vector3[]{start, end});
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
