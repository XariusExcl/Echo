using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDriver : MonoBehaviour
{
    private float _timeTillNextScan;

    public float TimeTillNextScan {get => _timeTillNextScan; set {_timeTillNextScan = value;}}
    public PlayerController PlayerController;
    public TMP_Text timeTillNextScanNumberText;
    public GameObject speedArrow;

    void Update()
    {
        timeTillNextScanNumberText.text = _timeTillNextScan.ToString("0.00");
        // assing rotation to arrow depending on the velocity 
        Debug.Log(PlayerController.Velocity.magnitude);
        speedArrow.transform.rotation = Quaternion.Euler(0, 0, PlayerController.Velocity.magnitude * 500);
    }
}
