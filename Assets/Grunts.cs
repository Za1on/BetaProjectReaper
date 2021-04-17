using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grunts : MonoBehaviour
{
    #region Damage and Exp
    public int m_EnemyHp;
    public Slider m_EnemyHPBar;
    public int m_EnemyExp;

    public void ReceiveDamage(int damage)
    {
        m_EnemyHPBar.gameObject.SetActive(true);
        m_EnemyHp -= damage;
        m_EnemyHPBar.value = m_EnemyHp;
        if (m_EnemyHp <= 0)
        {
            m_EnemyHp = 0;
            StartCoroutine(EnemyDie());
        }
        Debug.Log(m_EnemyHp);
    }
    public void ProcessExp(int exp)
    {
        m_PlayerManager.ReceiveExp(exp);
    }

    IEnumerator EnemyDie()
    {
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Grunts>().enabled = false;
        gameObject.GetComponent<Grunts>().m_EnemyAnimator.SetTrigger("Dead");
        yield return new WaitForSeconds(.4f);
        Destroy(gameObject);
        ProcessExp(m_EnemyExp);
    }
    #endregion
    #region FearPropagation
    public PlayerController m_PlayerManager;
    private EnemyManager m_CurrentGO;
    public Transform m_FearPropPos;
    public float m_FearPropStartSize;
    public float m_FearPropEndSize;
    protected float m_FearPropCurrentSize;
    public float m_FearPropScaleDuration;
    public void StartFearPropagation()
    {
        StartCoroutine(DoScalePropagation());
    }

    private void CheckForTargets()
    {
        Collider2D[] _ExecuteEnemy = Physics2D.OverlapCircleAll(m_FearPropPos.position, m_FearPropCurrentSize, m_PlayerManager.m_EnemyLayers);
        foreach (Collider2D _CurrentEnemy in _ExecuteEnemy)
        {
            if (_CurrentEnemy.isTrigger == true)
            {
                Debug.Log("Fear is propagated");
                m_CurrentGO = _CurrentEnemy.GetComponent<EnemyManager>();
                m_CurrentGO.ReceiveFear(m_CurrentGO.m_FearPropagationDamage);
                if (m_CurrentGO.m_FearHp <= m_CurrentGO.m_FearExecutionThreshold)
                {
                    m_CurrentGO.SetFearEffect();
                    m_CurrentGO.m_CanBeExecuted = true;
                }
            }
        }
    }

    private IEnumerator DoScalePropagation()
    {
        float timer = 0f;

        float lerpPercentage = 0f;

        transform.position = m_PlayerManager.transform.position;


        while (timer < m_FearPropScaleDuration)
        {
            lerpPercentage += Time.deltaTime / m_FearPropScaleDuration;
            m_FearPropCurrentSize = Mathf.Lerp(m_FearPropStartSize, m_FearPropEndSize, lerpPercentage);
            CheckForTargets();
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (m_FearPropPos != null) 
        {
            Gizmos.DrawWireSphere(m_FearPropPos.position, m_FearPropEndSize);
        }
        if (m_FearPropPos != null)
        {
            Gizmos.DrawWireSphere(m_FearPropPos.position, m_FearPropStartSize);
        }
        if (m_FearPropPos != null)
        {
            Gizmos.DrawWireSphere(m_FearPropPos.position, m_FearPropCurrentSize);
        }
    }
    #endregion
    #region EnemyMovement and Attack
    [Header("Enemy Game Object Manager")]
    public EnemyManager m_Enemy;
    public Animator m_EnemyAnimator;
    public SpriteRenderer m_EnemySprite;
    public Transform m_EnemyTransform;
    public Rigidbody2D m_EnemyRigid;

    [Header("Enemy Ground Detect")]
    [SerializeField] private float m_GroundCheckSize;
    public Transform m_GroundCheckAiL;
    public Transform m_GroundCheckAiR;

    [Header("Enemy check player position")]
    [SerializeField] private float m_PlayerCheckDistance;
    [SerializeField] private LayerMask m_WhatIsPlayer;
    public Transform m_PlayerCheckRight;
    public Transform m_PlayerCheckLeft;

    [Header("Enemy Movement")]
    public float m_AiMoveSpeed;
    public float m_AiMoveDistance;


    [Header("Enemy Knockback Settings")]
    public float m_EnemyKnockback;
    public float m_KnockbackDuration;
    public float m_EnemyKnockbackKnockbackCount;
    [HideInInspector] public bool m_EnemyKBRight;

    private bool m_TargetFound;
    private bool m_EnemyAttacking;
    private bool m_EnemyIdle;
    [HideInInspector] public bool EnemyKnockback = false;

    public void Start()
    {
        m_EnemyIdle = false;
        m_EnemyAttacking = false;
        m_TargetFound = false;
        m_Enemy.m_MoveAi = false;
        m_Enemy.m_MovingLeft = true;
        StartCoroutine(IdleStart());

    }


    private void FixedUpdate()
    {
        if (m_Enemy.m_MoveAi)
        {
            if (!m_Enemy.m_MovingLeft)
            {
                MoveRight();
                RaycastHit2D groundInfoRight = Physics2D.Raycast(m_GroundCheckAiR.position, Vector2.down, m_GroundCheckSize);
                if (groundInfoRight.collider == null)
                {
                    m_Enemy.m_MovingLeft = true;
                }
            }
            if (m_Enemy.m_MovingLeft)
            {
                MoveLeft();
                RaycastHit2D groundInfoLeft = Physics2D.Raycast(m_GroundCheckAiL.position, Vector2.down, m_PlayerCheckDistance);
                if (groundInfoLeft.collider == null)
                {
                    m_Enemy.m_MovingLeft = false;
                }
            }
        }
        if (EnemyKnockback)
        {
            if (m_EnemyKBRight)
            {
                Debug.Log("Enemy knock back to RIGHT");
                m_EnemyRigid.velocity = new Vector2(m_EnemyKnockback, 0);
            }
            else
            {
                Debug.Log("Enemy knock back to LEFT");
                m_EnemyRigid.velocity = new Vector2(-m_EnemyKnockback, 0);
            }
        }
    }

    IEnumerator EnemyKnockbackStart()
    {
        StopCoroutine(EnemyKnockbackStart());
        m_Enemy.m_MoveAi = false;
        if (!m_EnemyKBRight)
        {
            m_EnemyAnimator.SetTrigger("EnemyHurtLeft");
        }
        else
        {
            m_EnemyAnimator.SetTrigger("EnemyHurtRight");
        }

        EnemyKnockback = true;
        m_EnemyKnockbackKnockbackCount = m_KnockbackDuration;
        m_EnemyKnockbackKnockbackCount -= Time.deltaTime;
        yield return new WaitForSeconds(0.3f);
        EnemyKnockback = false;
        m_Enemy.m_MoveAi = true;
        m_EnemyAnimator.SetTrigger("Idle");

    }
    public void CheckPlayerSide(bool isPlayerRight)
    {
        if (isPlayerRight)
        {
            m_EnemyKBRight = false;
        }
        else
        {
            m_EnemyKBRight = true;
        }
        StartCoroutine(EnemyKnockbackStart());
    }

    public void MoveRight()
    {
        if (!m_EnemyIdle)
        {
            if (!m_EnemyAttacking && !EnemyKnockback)
            {
                m_EnemyAnimator.SetBool("WalkingLeft", false);
                m_EnemyAnimator.SetBool("WalkingRight", true);
                m_EnemyRigid.velocity = new Vector2(m_AiMoveSpeed * Time.deltaTime * 25.0f, m_EnemyRigid.velocity.y);
                if (!m_TargetFound)
                {
                    RaycastHit2D playerRight = Physics2D.Raycast(m_PlayerCheckRight.position, Vector2.right, m_PlayerCheckDistance, m_WhatIsPlayer);
                    if (playerRight)
                    {
                        m_EnemyAttacking = true;
                        m_TargetFound = true;
                        AttackPlayer();
                    }
                }
            }
        }
    }
    public void MoveLeft()
    {
        if (!m_EnemyIdle)
        {
            if (!m_EnemyAttacking && !EnemyKnockback)
            {
                m_EnemyAnimator.SetBool("WalkingRight", false);
                m_EnemyAnimator.SetBool("WalkingLeft", true);
                m_EnemyRigid.velocity = new Vector2(-m_AiMoveSpeed * Time.deltaTime * 25.0f, m_EnemyRigid.velocity.y);
                if (!m_TargetFound)
                {
                    RaycastHit2D playerLeft = Physics2D.Raycast(m_PlayerCheckLeft.position, Vector2.left, m_PlayerCheckDistance, m_WhatIsPlayer);
                    if (playerLeft)
                    {
                        m_TargetFound = true;
                        m_EnemyAttacking = true;
                        AttackPlayer();
                    }
                }
            }
        }
    }

    public void ManageMovement()
    {
        if (m_Enemy.m_MoveAi)
        {
            m_Enemy.m_MoveAi = false;
        }
        else
        {
            m_Enemy.m_MoveAi = true;
        }
    }

    IEnumerator IdleStart()
    {
        m_EnemyAnimator.SetTrigger("Idle");
        yield return new WaitForSeconds(2f);
        ManageMovement();
    }

    public void AttackPlayer()
    {
        m_EnemyAnimator.SetBool("IsAttacking", true);
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        m_EnemyRigid.velocity = Vector2.zero;
        m_EnemyAnimator.SetTrigger("Attack");
        m_EnemyIdle = true;
        m_TargetFound = false;
        m_EnemyAttacking = false;
        yield return new WaitForSeconds(.2f);
        StartCoroutine(IdleForTime());
        Collider2D player = Physics2D.OverlapCircle(m_PlayerCheckLeft.position, 1f, m_WhatIsPlayer);
        if (player)
        {
            var playerScript = player.GetComponent<PlayerController>();
            playerScript.m_PlayerKnockbackCount = playerScript.m_KnockbackDuration;
            playerScript.m_GotHit = true;
            playerScript.GetDamage(m_Enemy.m_EnemyDamage);
            if (player.transform.position.x < transform.position.x)
            {
                playerScript.m_PlayerKBRight = true;
            }
            else
            {
                playerScript.m_PlayerKBRight = false;
            }
        }
    }
    IEnumerator IdleForTime()
    {
        m_EnemyAnimator.SetBool("WalkingRight", false);
        m_EnemyAnimator.SetBool("WalkingLeft", false);
        yield return new WaitForSeconds(2.0f);
        m_EnemyAnimator.SetBool("IsAttacking", false);
        m_EnemyAnimator.SetTrigger("Idle");
        m_EnemyIdle = false;
    }
}
#endregion