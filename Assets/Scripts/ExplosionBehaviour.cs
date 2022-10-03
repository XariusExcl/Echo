using UnityEngine;

public class ExplosionBehaviour : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }
}