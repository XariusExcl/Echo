using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingInterface : MonoBehaviour
{
    VolumeProfile volumeProfile;
    Vignette vignette;
    void Start()
    {
        volumeProfile = GetComponent<Volume>()?.profile;
        if(!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
    }     

    void Update()
    {   
        /*
        if(!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        
        vignette.intensity.Override(0.5f);
        */
    } 

}
