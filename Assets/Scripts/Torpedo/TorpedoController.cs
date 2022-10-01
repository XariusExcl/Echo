using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoController : MonoBehaviour
{
    public float speed = 0.005f;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * speed;
    }
}
