using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPassThroughPlat : MonoBehaviour
{
    [SerializeField] private float m_TimeBeforeFlip;
    void Update()
    {
        if(this.GetComponent<PlatformEffector2D>().rotationalOffset != 0)
        {
            StartCoroutine(ResetFlip());
        }
    }

    IEnumerator ResetFlip()
    {
        yield return new WaitForSeconds(m_TimeBeforeFlip);
        this.GetComponent<PlatformEffector2D>().rotationalOffset = 0;
    }
}
