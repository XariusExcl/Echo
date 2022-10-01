using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFinderSwiper : MonoBehaviour
{
    float m_ActualRot = 0f;
    public float Rotation {get => m_ActualRot; private set { m_ActualRot = value;}}

    public void Swipe()
    {
        StartCoroutine(Co_Swipe());
    }
    
    IEnumerator Co_Swipe()
    {
        while(m_ActualRot != 360f)
        {
            m_ActualRot += 5f;
            transform.rotation = Quaternion.Euler(0f, 0f, -m_ActualRot);
            yield return new WaitForEndOfFrame();
        }
        m_ActualRot = 0f;
    }
}
