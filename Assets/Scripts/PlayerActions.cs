using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public Toggle[] playerAction;
    public AudioSource clickSfx;

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
