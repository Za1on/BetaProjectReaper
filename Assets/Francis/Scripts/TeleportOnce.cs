using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnce : MonoBehaviour
{
    [SerializeField] private Transform m_PlayerDestination;
    public PlayerController player;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.transform.position = m_PlayerDestination.transform.position;
        }
    }
}
