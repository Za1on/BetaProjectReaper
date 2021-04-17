using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointLocation : MonoBehaviour
{
    private CheckPointMaster m_CPM;


    public void Start()
    {
        m_CPM = GameObject.FindGameObjectWithTag("CPM").GetComponent<CheckPointMaster>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            m_CPM.m_SpawnCheckPoint = true;
            m_CPM.m_LastCheckPointPos = transform.position;
            this.GetComponent<CircleCollider2D>().enabled = false;
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
