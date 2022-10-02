using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraShake;

    public IEnumerator CameraShake(float cameraShake)
    {
        while(true)
        {
            if (cameraShake > 0.005f) // Treshold
            {
                cameraShake *= .95f;
                transform.position = new Vector3(Random.Range(-cameraShake, cameraShake), Random.Range(-cameraShake, cameraShake), -10f);
            } else { cameraShake = 0f; break; }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
