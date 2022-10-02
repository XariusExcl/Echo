using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTorpedoController : RadarEnemy
{
    public GameObject radarBlip;

    new void Start()
    {
        base.Start();
        // AlreadyRevealed = true;
    }
    
    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!AlreadyRevealed)
        {
            if (viewFinderSwiper.Rotation > vfAngle )
            {
                RevealItself();
            }
        }
    }
    
    new void RevealItself()
    {
        Debug.Log("Torpedo Revealed");
        base.RevealItself();
        Instantiate(radarBlip, transform.position, transform.rotation);
        lastRevealTime = Time.realtimeSinceStartup;
    }
}
