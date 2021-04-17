using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Choose if movement Ai can be influenced by 'Turn around Targets'")]
    public bool m_TargetAi;
    [Header("Choose if movement Ai can rush the player down'")]
    public bool m_RushPlayer;
    private bool m_DoRushPlayer;
    [Header("Choose if Ai uses attack animation or not")]
    public bool m_AttackEnable;


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
    public float m_EnemyAttackSize;

    [Header("Enemy Stats")]
    public float m_TimeBeforeEnemyHits;
    public float m_AiMoveSpeed;
    public float m_RushAiMoveSpeed;
    private float m_AiMoveSpeedInitial;
    public float m_AiMoveDistance;
    public float m_EnemyAttackSpeed;
    public float m_EnemyIdleTimer;
    public bool m_ChangeBehaviour;
    public int m_ChangedDamage;
    public float m_EnemyBehaviourChangeThreshold;
    public float m_ChangedMoveSpeed;
    public bool m_ChangeSizeBehaviour = false;
    public float m_ChangeSize;
    private bool m_ChangedBehaviour = false;


    [Header("Enemy Knockback Settings")]
    public float m_EnemyKnockback;
    public float m_KnockbackDuration;
    public float m_EnemyKnockbackKnockbackCount;
    [HideInInspector] public bool m_EnemyKBRight;

    private Transform m_CurrentSide;
    [HideInInspector] public float m_DistanceBetween;
    private bool m_TargetFound;
    private bool m_EnemyAttacking;
    private bool m_EnemyIdle;
    private bool m_StopAttack = false;
    private bool m_WasGoingRight;
    private bool m_WasGoingLeft;
    [HideInInspector] public bool EnemyKnockback = false;

    public void Start()
    {
        m_AiMoveSpeedInitial = m_AiMoveSpeed;
        m_CurrentSide = m_PlayerCheckLeft;
        m_EnemyIdle = false;
        m_EnemyAttacking = false;
        m_TargetFound = false;
        m_Enemy.m_MoveAi = false;
        m_Enemy.m_MovingLeft = true;
        StartCoroutine(IdleStart());
        if (this.CompareTag("ScareCrow"))
        {
            m_EnemyAnimator = null;
        }
    }

    private void FixedUpdate()
    {
            if (m_Enemy.m_MoveAi && !m_DoRushPlayer)
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
            if(m_RushPlayer && m_Enemy.m_MoveAi)
            {
               if(m_DoRushPlayer)
               {
                    if (m_DistanceBetween <= 0.0f)
                    {
                    MoveRight();
                    m_Enemy.m_MovingLeft = false;
                    RaycastHit2D groundInfoRight = Physics2D.Raycast(m_GroundCheckAiR.position, Vector2.down, m_GroundCheckSize);
                    if (groundInfoRight.collider == null)
                    {
                        m_EnemyIdle = true;
                    }
                }
                  else
                  {
                    MoveLeft();
                    m_Enemy.m_MovingLeft = true;
                    RaycastHit2D groundInfoLeft = Physics2D.Raycast(m_GroundCheckAiL.position, Vector2.down, m_PlayerCheckDistance);
                    if (groundInfoLeft.collider == null)
                    {
                        m_EnemyIdle = true;
                    }
                }
               }
            }
        if (m_Enemy.m_MovingLeft)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
        if (EnemyKnockback)
        {
            if (m_EnemyKBRight)
            {
                m_EnemyRigid.velocity = new Vector2(m_EnemyKnockback, 0);
            }
            else
            {
                m_EnemyRigid.velocity = new Vector2(-m_EnemyKnockback, 0);
            }
        }


    }

    public void Update()
    {      
        if (m_StopAttack)
        {
            StopCoroutine(AttackLoop());
        }
        if (m_ChangeBehaviour)
        {
            ChangeBehaviour();
        }
        if(m_ChangeSizeBehaviour && m_ChangedBehaviour)
        {
            ChangeSizeBehaviour();
        }
        if(m_Enemy.m_PlayerHittedEnemy)
        {
            StartCoroutine(PlayerHitted());
        }
        CheckForRush();
    }

    public void CheckForRush()
    {
        if(m_RushPlayer)
        {
            float distance = m_Enemy.transform.position.y - GameObject.FindGameObjectWithTag("Player").transform.position.y;
            if (distance <= 1.0f && distance >= -1.0f)
            {
                m_DoRushPlayer = true;
                m_AiMoveSpeed = m_RushAiMoveSpeed;
            }
            else
            {
                m_EnemyIdle = false;
                m_DoRushPlayer = false;
                m_AiMoveSpeed = m_AiMoveSpeedInitial;
            }
            if (m_DoRushPlayer)
            {
                m_DistanceBetween = m_Enemy.transform.position.x - GameObject.FindGameObjectWithTag("Player").transform.position.x;
            }
        }    
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_TargetAi)
        {
            if (collision.CompareTag("EnemyReturn"))
            {
                if (m_Enemy.m_MovingLeft)
                {
                    m_Enemy.m_MovingLeft = false;
                    m_Enemy.m_MovingRight = true;
                }
                else
                {
                    m_Enemy.m_MovingRight = false;
                    m_Enemy.m_MovingLeft = true;                 
                }
            }
        }
    }

    public void ChangeBehaviour()
    {
        int damage = m_Enemy.m_EnemyDamage;
        m_Enemy.m_EnemyDamage = 0;
        if (m_Enemy.m_FearHp <= m_EnemyBehaviourChangeThreshold)
        {
            m_AiMoveSpeed = m_ChangedMoveSpeed;
            m_Enemy.m_EnemyDamage = m_ChangedDamage;
            m_ChangedBehaviour = true;
            m_ChangeBehaviour = false;
        }
    }
    public void ChangeSizeBehaviour()
    {
        if (m_ChangeSizeBehaviour)
        {
            transform.localScale = new Vector3(m_ChangeSize, m_ChangeSize, transform.localScale.z);
            m_EnemyAttackSize = 3.0f;
            m_ChangeSizeBehaviour = false;
        }
    }

    IEnumerator EnemyKnockbackStart()
    {
        m_EnemyAnimator.SetBool("IsAttacking", false);
        m_StopAttack = true;
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
        m_StopAttack = false;
        StartCoroutine(IdleForTime());
    }
    public void CheckPlayerSide(bool isPlayerRight)
    {
        if(isPlayerRight)
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
        m_CurrentSide = m_PlayerCheckRight;
        if (!m_EnemyIdle)
        {
            if (!m_EnemyAttacking && !EnemyKnockback)
            {
                m_EnemyAnimator.SetBool("WalkingLeft", false);
                m_EnemyAnimator.SetBool("WalkingRight", true);
                m_EnemyRigid.velocity = new Vector2(m_AiMoveSpeed * Time.deltaTime * 25.0f, m_EnemyRigid.velocity.y);
                if (!m_TargetFound)
                {
                    if(m_AttackEnable)
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
    }
    public void MoveLeft()
    {
        m_CurrentSide = m_PlayerCheckLeft;
        if (!m_EnemyIdle)
        {
            if (!m_EnemyAttacking && !EnemyKnockback)
            {
                m_EnemyAnimator.SetBool("WalkingRight", false);
                m_EnemyAnimator.SetBool("WalkingLeft", true);
                m_EnemyRigid.velocity = new Vector2(-m_AiMoveSpeed * Time.deltaTime * 25.0f, m_EnemyRigid.velocity.y);
                if (!m_TargetFound)
                {
                    if(m_AttackEnable)
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
        yield return new WaitForSeconds(0.5f);
        m_Enemy.m_MoveAi = true;
    }

    public void AttackPlayer()
    {
        m_EnemyAnimator.SetBool("IsAttacking", true);
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        ManageMovement();
        m_EnemyRigid.velocity = Vector2.zero;
        m_EnemyAnimator.SetTrigger("Attack");
        m_EnemyIdle = true;
        m_TargetFound = false;
        m_EnemyAttacking = false;
        yield return new WaitForSeconds(m_EnemyAttackSpeed);   
        StartCoroutine(IdleForTime());
        Collider2D player = Physics2D.OverlapCircle(m_CurrentSide.position, m_EnemyAttackSize, m_WhatIsPlayer);
        if(player)
        {
            var playerScript = player.GetComponent<PlayerController>();
            if (m_Enemy.GetComponent<EnemyManager>().m_PlayerHittedEnemy == false)
            {
                playerScript.GetDamage(m_Enemy.m_EnemyDamage);
                playerScript.m_PlayerKnockbackCount = playerScript.m_KnockbackDuration;
                playerScript.m_GotHit = true;
                if (player.transform.position.x < transform.position.x)
                {
                    playerScript.m_PlayerKBRight = true;
                }
                else
                {
                    playerScript.m_PlayerKBRight = false;
                }
            }
            else
            {
                StopCorouIdleStart();
            }
        }
        else
        {
            StopCorouIdleStart();
        }
        StopCorouIdleStart();
    }
    IEnumerator IdleForTime()
    {
        m_EnemyAnimator.SetBool("WalkingRight", false);
        m_EnemyAnimator.SetBool("WalkingLeft", false);
        yield return new WaitForSeconds(m_EnemyIdleTimer);
        m_EnemyAnimator.SetBool("IsAttacking", false);
        m_EnemyAnimator.SetTrigger("Idle");
        m_EnemyIdle = false;
    }
    IEnumerator PlayerHitted()
    {
        yield return new WaitForSeconds(m_TimeBeforeEnemyHits);
        m_Enemy.m_PlayerHittedEnemy = false;
    }

    public void StopCorouIdleStart()
    {
        m_EnemyAttacking = false;
        StopAllCoroutines();
        StartCoroutine(IdleForTime());
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_PlayerCheckLeft.position, m_EnemyAttackSize);
        Gizmos.DrawWireSphere(m_PlayerCheckRight.position, m_EnemyAttackSize);
    }
}


//------------------------------------------------------------------------------------------------------------------------//
                                        //LEFTOVER MOVEMENT CODE//
//------------------------------------------------------------------------------------------------------------------------//
/*if(m_TargetAi)
{
    if(m_Enemy.m_MoveAi)
    {
        m_Ratio = (Time.time - m_StartTime) / m_TravelingSpeed;

        if (m_Ratio > 1)
        {
            m_Ratio = 1;
        }

        if (m_GoingLeft)
        {
            transform.position = m_LeftLocation.position + (m_RightLocation.position - m_LeftLocation.position) * m_Ratio;
            m_CurrentSide = m_PlayerCheckRight;
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
        else
        {
            transform.position = m_RightLocation.position + (m_LeftLocation.position - m_RightLocation.position) * m_Ratio;
            m_CurrentSide = m_PlayerCheckLeft;
            if (!m_EnemyIdle)
            {
                if (!m_EnemyAttacking && !EnemyKnockback)
                {
                    m_EnemyAnimator.SetBool("WalkingRight", false);
                    m_EnemyAnimator.SetBool("WalkingLeft", true);
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
        if (m_Ratio >= 1)
        {
            m_StartTime = Time.time;
            m_GoingLeft = !m_GoingLeft;
        }
    } 
}*/
