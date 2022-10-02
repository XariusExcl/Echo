using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoTrajectory : MonoBehaviour
{
    public LineRenderer trajectory;
    public float opacity = 1f;
 

    void Update()
    {
        trajectory.SetPosition(0, transform.position);
        trajectory.SetPosition(1, transform.right * 100);
    }
}
