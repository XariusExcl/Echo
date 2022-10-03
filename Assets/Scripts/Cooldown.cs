using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
    public SpriteRenderer cooldownBar;
    public PlayerController player;

    // Update is called once per frame
    void Update()
    {
        cooldownBar.transform.localPosition = new Vector3(0, player.CountermeasureProgress / 2f - 0.5f, 0);
        cooldownBar.transform.localScale = new Vector3(1, player.CountermeasureProgress, 1);
    }
}
