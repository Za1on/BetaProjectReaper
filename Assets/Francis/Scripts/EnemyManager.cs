using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : ExecutionManager
{

    [Header("Game Objects")]
    public Transform m_PlayerTeleport;
    public GameObject m_VirtualCameraEnemy;
    public ParticleSystem m_ICanBeExecuted;
    public ExecutionManager m_ExecManager;
    public GameObject m_Scarecrow;
    public bool m_ExecutionPrompt;
    public GameObject m_ShowExecutable;
    [HideInInspector] public EnemyAI m_EnemyAI;

    [Header("Enemy Stats")]
    public int m_FearHp;
    public int m_EnemyHp;
    public Slider m_EnemyHPBar;
    public int m_EnemyExp;
    [Tooltip("Player's  Damage on the Enemy")]
    public int m_PlayerDamage;
    [Tooltip("Player's Fear Damage on the Enemy")]
    public int m_PlayerFearDamage;
    public int m_EnemyDamage;
    [Tooltip("How much fear does the enemy need to be executed")]
    public int m_FearExecutionThreshold;
    [Tooltip("How much damage does the propagation cause")]
    public int m_FearPropagationDamage;

    [Tooltip("What is the Player's Layer?")]
    public LayerMask m_Player;

    public bool m_CanBeExecuted;
    [HideInInspector] public bool m_MoveAi = true;
    [HideInInspector] public bool m_MovingLeft;
    [HideInInspector] public bool m_MovingRight;
    [HideInInspector] public bool m_PlayerHittedEnemy = false;

    public bool m_CanBeFeared = false;
    public GameObject m_FearSys;
    [HideInInspector] public bool m_ActiveExec;
    private bool m_Died = false;

    public void Start()
    {
        m_FearSys = GameObject.FindGameObjectWithTag("FearSys");
        m_EnemyHPBar.value = m_EnemyHp;
        m_CanBeExecuted = false;
    }

    public override void Update()
    {   
        if (m_Died)
        {
            StartFearPropa();
            StartCoroutine(EnemyDie());
        }
        if (m_ActiveExec)
        {
            m_ShowExecutable.SetActive(true);
        }
        else
        {
            m_ShowExecutable.SetActive(false);
        }
    }

    //Give fear damage and manage it
    public void ReceiveFear(int fearDamage)
    {
            m_FearHp -= fearDamage;
            if (m_FearHp <= m_FearExecutionThreshold)
            {
                SetFearEffect();
                Debug.Log("Can be executed");
                m_CanBeExecuted = true;
            }
            if (m_FearHp <= 0)
            {
                m_FearHp = 0;
            }   
    }
    public void ReceiveDamage(int damage)
    {
        m_EnemyHPBar.gameObject.SetActive(true);
        m_EnemyHp -= damage;
        m_EnemyHPBar.value = m_EnemyHp;
        if (m_EnemyHp <= 0)
        {
            m_Died = true;
        }
    }


    IEnumerator EnemyDie()
    {
        m_EnemyManager.GetComponent<Rigidbody2D>().simulated = false;
        m_EnemyManager.GetComponent<BoxCollider2D>().enabled = false;
        m_EnemyManager.GetComponent<EnemyAI>().enabled = false;
        m_EnemyManager.GetComponent<EnemyAI>().m_EnemyAnimator.SetTrigger("Dead");
        yield return new WaitForSeconds(.4f);
        Destroy(m_EnemyManager.gameObject);
        ProcessExp(m_EnemyExp);
        CreateScareCrow();
    }

    public void CreateScareCrow()
    {
        if(m_ScarecrowDrop)
        {
            GameObject Scarecrow = Instantiate(m_Scarecrow);
            Scarecrow.transform.position = this.transform.position;
            Scarecrow.GetComponent<SmallEnemy>().m_PlayerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Scarecrow.GetComponent<SmallEnemy>().m_FearPropa = GameObject.FindGameObjectWithTag("FearSys").GetComponent<FearPropagation>();
            Scarecrow.GetComponent<SmallEnemy>().m_CanBeExecuted = true;
            Scarecrow.GetComponent<SmallEnemy>().enabled = false;
        }
    }
    public void SetFearEffect()
    {
        if(m_CanBeFeared)
        {
            m_ICanBeExecuted.gameObject.SetActive(true);
        }
    }
}
