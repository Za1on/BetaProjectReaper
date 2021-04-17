using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static bool GameIsPaused;
    private CheckPointMaster m_CPM;

    public GameObject PauzeMenuObject;

    private void Start()
    {
       m_CPM = GameObject.FindGameObjectWithTag("CPM").GetComponent<CheckPointMaster>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        PauzeMenuObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }
    public void Pause()
    {
        PauzeMenuObject.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Continue()
    {
        Resume();
    }
    public void QuitCurrentGame()
    {
        m_CPM.GetComponent<CheckPointMaster>().ResetCP();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        PauzeMenuObject.SetActive(false);
    }

}
