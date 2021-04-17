using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPromp : MonoBehaviour
{
    
    public float m_letterDelay = 0.1f;
    public Text m_TextBox;
    public string m_Information;
    public bool m_skiping;
    public bool m_DoneReading;
    public GameObject m_theMessage;
    public PlayerController m_Player;
    public GameObject m_DialogueBox;
    public GameObject m_Arrow;

    public void Update()
    {
        if(m_DoneReading)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == ("Player"))
        {
            m_theMessage.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            m_theMessage.SetActive(false);
            ConveyInformation();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == ("Player"))
        {
            m_theMessage.SetActive(false);
            gameObject.SetActive(true);
        }
    }
    IEnumerator ConveyInformation()
    {
        m_theMessage.SetActive(true);
        m_TextBox.text += m_Information;
        foreach (char letter in m_Information.ToCharArray())        
        {
            m_TextBox.text += m_Information;
            yield return new WaitForSeconds(m_letterDelay);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_letterDelay *= 2;
            m_skiping = true;
        }
        if(m_skiping)
        {
            m_Arrow.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.E) && m_skiping)
        {
            m_DialogueBox.SetActive(false);
            m_Arrow.SetActive(false);
            m_DoneReading = true;
        }
    }
}
