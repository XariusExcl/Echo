using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public Toggle playerAction;
    public AudioSource clickSfx;

    private void Update()
    {
        playerAction.onValueChanged.AddListener(delegate
        {
            if(playerAction.isOn)
            {
                clickSfx.Play();
            }
        });
    }
}
