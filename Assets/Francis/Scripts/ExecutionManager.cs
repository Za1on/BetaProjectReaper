using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionManager : MonoBehaviour
{
    
    [Header("Player and Enemy Execution Manager")]
    public PlayerController m_PlayerManager;
    public FearPropagation m_FearPropa;
    [HideInInspector] public EnemyManager m_EnemyManager;


    [Header("Fear System")]
    public Transform m_FearPropPos;
    public float m_FearPropStartSize;
    public float m_FearPropEndSize;
    protected float m_FearPropCurrentSize;
    public float m_FearPropScaleDuration;

    [Tooltip("Player teleport speed to execute")]
    [SerializeField] private float m_TeleportSpeed = 100f;

    [Header("Should Enemy spawn a Scarecrow")]
    public bool m_ScarecrowDrop;

    private bool m_TeleportToEnemy = false;
    private bool m_UpdateSpriteTeleport;
    private bool m_PlayerSpriteFlipRight;
    private bool m_PlayerSpriteFlipLeft;
    private Transform m_EnemyExecutionPos;




    public virtual void Update()
    {
        if (m_TeleportToEnemy)
        {
            m_PlayerManager.transform.position = Vector3.MoveTowards(m_PlayerManager.transform.position, m_EnemyManager.m_PlayerTeleport.position, m_TeleportSpeed * Time.deltaTime * 5f);
            if(m_PlayerManager.transform.position == m_EnemyManager.transform.position)
            {
                m_TeleportToEnemy = false;
            }
        }        
    }

    public void CheckForExecute(bool monsterExec)
    {
        if (!m_EnemyManager.m_CanBeExecuted)
        {
            return;
        }
        if(monsterExec)
        {
            StartCoroutine(StartExecutionLoop());
        }
        else if(!monsterExec)
        {
            Debug.Log("Scarecrow Loop exec staarted");
            StartCoroutine(StartScarecrowLoop());
        }
    }

    IEnumerator StartScarecrowLoop()
    {
        m_EnemyExecutionPos = m_EnemyManager.transform;
        CheckPlayerFacing();
        DeactivatePlayer();
        StartCoroutine(TeleportToExecute());
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(PlayerExecuteAnimation());
        yield return new WaitForSeconds(0.6f);
        TeleportToEnemyLocation();
        ActivatePlayer();
        PlayerToIdle();
    }

    IEnumerator StartExecutionLoop()
    {
        m_EnemyExecutionPos = m_EnemyManager.transform;
        CheckPlayerFacing();
        DeactivatePlayer();
        DeactivateEnemy();       
        StartCoroutine(TeleportToExecute());
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(PlayerExecuteAnimation());
        yield return new WaitForSeconds(0.6f);
        Destroy(m_EnemyManager.gameObject);
        StartFearPropa();
        TeleportToEnemyLocation();
        ActivatePlayer();
        m_EnemyManager.CreateScareCrow();
        PlayerToIdle();
        ProcessExp(m_EnemyManager.m_EnemyExp);
    }

    public void StartFearPropa()
    {
        m_FearPropa.StartFearPropagation();
    }

    public void ProcessExp(int exp)
    {
        m_PlayerManager.ReceiveExp(exp);
    }

    public void CheckPlayerFacing()
    {
        if(m_PlayerManager.m_PlayerSpriteRend.flipX == true)
        {
            m_PlayerManager.m_PlayerSpriteRend.flipX = false;
        }
    }    

    public void DeactivatePlayer()
    {
        m_PlayerManager.GetComponent<PlayerController>().enabled = false;
        m_PlayerManager.GetComponent<Rigidbody2D>().simulated = false;
        m_PlayerManager.GetComponent<BoxCollider2D>().enabled = false;
    }
    public void ActivatePlayer()
    {
        m_PlayerManager.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        m_PlayerManager.m_PlayerSpriteRend.flipX = true;
        m_PlayerManager.GetComponent<PlayerController>().enabled = true;
        m_PlayerManager.GetComponent<Rigidbody2D>().simulated = true;
        m_PlayerManager.GetComponent<BoxCollider2D>().enabled = true;
        m_UpdateSpriteTeleport = false;
    }
    public void DeactivateEnemy()
    {
        m_EnemyManager.GetComponent<Rigidbody2D>().simulated = false;
        m_EnemyManager.GetComponent<BoxCollider2D>().enabled = false;
    }
    IEnumerator TeleportToExecute()
    {
            if (m_PlayerManager.transform.position.x < m_EnemyManager.transform.position.x)
            {
                if (!m_PlayerManager.m_IsSpriteLookingRight)
                {
                    m_PlayerManager.m_PlayerSpriteRend.flipX = false;
                }
            }
            if (m_PlayerManager.transform.position.x > m_EnemyManager.transform.position.x)
            {
                if (m_PlayerManager.m_IsSpriteLookingRight)
                {
                    m_PlayerManager.m_PlayerSpriteRend.flipX = true;
                }
            }
        yield return new WaitForSeconds(.1f);
        m_EnemyManager.m_VirtualCameraEnemy.gameObject.SetActive(true);
        m_TeleportToEnemy = true;
        m_UpdateSpriteTeleport = true;
        m_PlayerManager.m_PlayerAnimator.SetTrigger("Teleporting");
    }
    public void TeleportToEnemyLocation()
    {
        m_PlayerManager.transform.position = m_EnemyExecutionPos.position;
        m_EnemyManager.m_VirtualCameraEnemy.gameObject.SetActive(false);
    }
    IEnumerator PlayerExecuteAnimation()
    {
        m_PlayerManager.m_PlayerSpriteRend.flipX = false;
        m_PlayerManager.m_PlayerAnimator.SetTrigger("Spellcast");
        yield return new WaitForSeconds(.2f);
        m_EnemyManager.GetComponent<EnemyAI>().m_EnemyAnimator.SetTrigger("Dead");
        if(m_PlayerManager.m_HealExecution)
        {
            m_PlayerManager.m_PlayerCurrentHP += m_PlayerManager.m_ExecutionHealAmount;
          
            if (m_PlayerManager.m_PlayerCurrentHP >= m_PlayerManager.m_PlayerMaxHP)
            {
                m_PlayerManager.m_PlayerCurrentHP = m_PlayerManager.m_PlayerMaxHP;
            }
            m_PlayerManager.updateUI();
        }    
    }

    public void PlayerToIdle()
    {
        m_PlayerManager.m_PlayerSpriteRend.flipX = false;
        m_PlayerManager.m_PlayerAnimator.SetTrigger("Idle");
        m_PlayerManager.m_IsJumping = false;
    }
}
