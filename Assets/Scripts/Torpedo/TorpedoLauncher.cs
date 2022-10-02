using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TorpedoLauncher : MonoBehaviour
{
    public GameObject torpedo;

    public void Fire(float angle)
    {
        Instantiate(torpedo, transform.position, Quaternion.Euler(0, 0, angle));
    }
}
