using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoTrajectory : MonoBehaviour
{
    public LineRenderer trajectory;

    void Update()
    {
        trajectory.SetPosition(0, transform.position);
        trajectory.SetPosition(1, transform.position + transform.right * 10f);
    }
}
