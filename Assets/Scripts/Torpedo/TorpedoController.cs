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
    GameManager _gameManager;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        rigidbody2D.velocity = transform.right * speed;
        _spawnTime = Time.realtimeSinceStartup;
        _gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        if(!_armed && Time.realtimeSinceStartup - _spawnTime > 0.1f) // FIXME 
        {
            collider2D.enabled = _armed = true;
        }

        // If torpedo is out of screen
        if(Vector2.SqrMagnitude(transform.position - new Vector3(0.0f, 0.6f, 0.0f)) > 16f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Torpedo collision");
        Explosion();
    }

    public void Explosion()
    {
        rigidbody2D.velocity = new Vector2(0f, 0f);
        GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
