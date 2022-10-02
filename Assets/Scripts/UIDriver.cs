using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDriver : MonoBehaviour
{
    private float _timeTillNextScan;
    public float TimeTillNextScan {get => _timeTillNextScan; set {_timeTillNextScan = value;}}

    private bool _isAlarmOn = false;
    public bool IsAlarmOn {get => _isAlarmOn; private set{_isAlarmOn = value;}}
    public PlayerController PlayerController;
    public TMP_Text timeTillNextScanNumberText;
    public GameObject speedArrow;

    void Update()
    {
        timeTillNextScanNumberText.text = _timeTillNextScan.ToString("0.00");
        // assing rotation to arrow depending on the velocity
        speedArrow.transform.rotation = Quaternion.Euler(0, 0, PlayerController.Velocity.magnitude * 500);
    }

    public void TurnAlarmOn()
    {
        // TODO
        Debug.LogWarning("Turn Alarm on!!!! we gon die!!!!");

        _isAlarmOn = true;
    }

    public void TurnAlarmOff()
    {
        // TODO
        Debug.LogWarning("Turn Alarm off :)");

        _isAlarmOn = false;
    }
}
