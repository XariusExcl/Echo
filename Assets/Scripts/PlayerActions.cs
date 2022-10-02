using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public Toggle[] playerAction;
    public AudioSource clickSfx;
    public bool isDisabled;

    public void EnableInteractivity()
    {
        foreach (Toggle toggle in playerAction)
        {
            toggle.interactable = true;
            isDisabled = false;
        }
    }
    
    public void DisableInteractivity()
    {
        foreach (Toggle toggle in playerAction)
        {
            toggle.interactable = false;
            isDisabled = true;
        }
    }
    
    public void ResetDefault()
    {
        foreach (Toggle toggle in playerAction)
        {
            if (toggle.name == "Move")
            {
                toggle.isOn = true;
            }
        }
    }

    private void Update()
    {
        foreach (Toggle toggle in playerAction)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                if (toggle.isOn)
                {
                    clickSfx.Play();
                }
            });
        }
    }
}
