using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoController : MonoBehaviour
{
    public float speed = 0.3125f;
    new Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();   
        rigidbody2D.velocity = transform.right * speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(GetComponent<Collider2D>());
    }
}
