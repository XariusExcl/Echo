using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDriver : MonoBehaviour
{
    private float _timeTillNextScan;

    public float TimeTillNextScan {get => _timeTillNextScan; set {_timeTillNextScan = value;}}
    public TMP_Text timeTillNextScanNumberText;

    void Update()
    {
        timeTillNextScanNumberText.text = _timeTillNextScan.ToString("0.00");
    }
}
