using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmiter_TestWwise : MonoBehaviour
{
    public bool m_StartSound = false;
    // Start is called before the first frame update
    public void Start()
    {
        AkSoundEngine.PostEvent("testCoin", this.gameObject);
    }
    public void Update()
    {
        if(m_StartSound)
        {
            AkSoundEngine.PostEvent("testCoin", this.gameObject);
            m_StartSound = false;
        }
    }
}
