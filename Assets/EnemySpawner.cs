using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_myself;
    public GameObject m_EnemyPrefab;
    public Transform m_LocationOne;
    public Transform m_LocationTwo;
    public bool m_SpawnThem = false;



    // Update is called once per frame
    void Update()
    {
        if (m_myself == null)    
        {
            m_SpawnThem = true;   
        }
        if(m_SpawnThem)
        {
            SpawnEnemmies();
            m_SpawnThem = false;
        }
        
        
    }
    public void SpawnEnemmies()
    {
            GameObject enemy1 = Instantiate(m_EnemyPrefab, m_LocationOne.transform.position, Quaternion.identity);
            enemy1.GetComponent<EnemyManager>().m_PlayerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            GameObject enemy2 = Instantiate(m_EnemyPrefab, m_LocationTwo.transform.position, Quaternion.identity);
            enemy2.GetComponent<EnemyManager>().m_PlayerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Destroy(gameObject);
    }
}
