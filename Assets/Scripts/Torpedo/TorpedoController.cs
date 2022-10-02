using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoController : MonoBehaviour
{
    public float speed = 0.3125f;
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    public GameObject explosion;
    float _spawnTime;
    bool _armed = false;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        rigidbody2D.velocity = transform.right * speed;
        _spawnTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if(!_armed && Time.realtimeSinceStartup - _spawnTime > 0.1f) // FIXME 
        {
            Debug.Log("Torpedo armed.");
            collider2D.enabled = _armed = true;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Torpedo collision");
        Destroy(GetComponent<Collider2D>());
        rigidbody2D.velocity = new Vector2(0f, 0f);
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(newExplosion, 2.9f);
        Destroy(this.gameObject, 3f);
    }
}
