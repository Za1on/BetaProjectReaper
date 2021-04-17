using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointMaster : MonoBehaviour
{
    private static CheckPointMaster m_GMinstance;
    public PlayerController m_Player;
    public Vector2 m_LastCheckPointPos;
    [HideInInspector] public Vector3 m_PlayerInitialSpawnPos;
    public bool m_SpawnCheckPoint = false;
    public bool m_ChangePos = false;
    [HideInInspector] public GameObject[] m_CheckPointList;

    private void Awake()
    {
        if (m_GMinstance == null)
        {
            m_GMinstance = this;
            DontDestroyOnLoad(m_GMinstance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        m_CheckPointList = GameObject.FindGameObjectsWithTag("CPP");

    }

    public void Update()
    {
        if (m_Player == null)
        {
            m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            m_PlayerInitialSpawnPos = m_Player.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            StartCoroutine(SpawnPlayer());
        }
    }


    IEnumerator SpawnPlayer()
    {
        m_ChangePos = false;
        Debug.Log("Changing player pos to the checkpoint");
        yield return new WaitForSeconds(0.001f);
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_Player.transform.position = m_LastCheckPointPos;
    }
    public void PlayedDied()
    {
        SceneManager.LoadScene(1);
        StartCoroutine(SpawnPlayer());
    }
    public void RetryProto()
    {
        SceneManager.LoadScene(1);
        StartCoroutine(SpawnPlayer());
    }
    public void ResetCP()
    {
        m_LastCheckPointPos = m_PlayerInitialSpawnPos;
    }
}


