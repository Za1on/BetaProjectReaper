using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public float m_DialogueSpeed = 1f;
    public bool m_DialogueIsDone;
    public string m_DialogueItself;
    ///public List<string> m_Dialogues = new List<string>(); <--- for more then 1 page of dialogue.
    public TextMeshProUGUI m_DialogueText;
    public GameObject m_Canvas;
    public SpriteRenderer m_DialoguePrompt;
    public GameObject m_Continue;
    public CanvasGroup m_DialogueUI; //keep so can add effect like fade in

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer != 8)
        {
            return;
        }
        m_DialoguePrompt.gameObject.SetActive(true);

        StartCoroutine(DoDialogue()); 
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 8)
        {
            return;
        }
        m_DialoguePrompt.gameObject.SetActive(false);
        DesactivateDialogue();
    }
   private IEnumerator DoDialogue()
    {
        while(!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }
        yield return null;
        ActivateDialogue();

        StartCoroutine(DoLetterAnimation());

        while (!Input.GetKeyDown(KeyCode.E) && m_DialogueIsDone == false)
        {
            yield return null;
        }
        yield return null;
        m_DialogueIsDone = true;

        m_Continue.SetActive(true);

        while (!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }
        yield return null;

        DesactivateDialogue();

    }

    private IEnumerator DoLetterAnimation()
    {
        char[] dialogueCharArray = m_DialogueItself.ToCharArray();
        string finalString = "";

        foreach (char c in dialogueCharArray)
        {
            if(m_DialogueIsDone)
            {
                break;
            }
            finalString += c;
            m_DialogueText.text = finalString;
            yield return new WaitForSeconds(m_DialogueSpeed);
        }

        m_DialogueText.text = m_DialogueItself;
        m_DialogueIsDone = true;
    }

    public void ActivateDialogue()
    {
        m_DialoguePrompt.gameObject.SetActive(false);
        m_Canvas.gameObject.SetActive(true);
        m_DialogueUI.gameObject.SetActive(true);
        m_DialogueText.text = "";
    }
    private void DesactivateDialogue()
    {
        m_DialogueIsDone = false;
        m_Canvas.gameObject.SetActive(false);
        m_DialogueUI.gameObject.SetActive(false);
        m_Continue.gameObject.SetActive(false);
    }
    
}
