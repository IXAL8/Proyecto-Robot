using System.Collections;
using Robot;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class Range : MonoBehaviour, IAttackable
{
    [Header("Movimiento")]
    public float speed = 2f;
    public GameObject pointA;
    public GameObject pointB;
    public float waitTime = 2f;
    private float stuckTimer = 0f;
    private float maxStuckTime = 20f;
    private Vector3 patrolCenter;
    private float patrolRange;
    
    [Header("Vision")]
    public float viewDistance = 8f;
    public float viewAngle = 60f; 
    public float headOffset = 1.5f; 
    public LayerMask playerLayer;

    [Header("Combate")]
    public int maxHealth = 50;
    public int currentHealth;
    private bool isDead = false;
    public float attackCooldown = 2f;
    public float attackRange = 10f;
    public float attackTimer = 0f;
    public bool canAttack = true;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private bool isAttacking = false;
    
    [Header("Retroceso")]
    public float retreatDistance = 2.5f;
    //public float safeDistance = 4f;
    public float wallCheckDistance = 1.5f;
    public LayerMask wallLayer;
    private bool isRetreating = false;
    
    [Header("Sistema de Drops")]
    public GameObject[] drops;
    public float dropChance = 0.5f;
    
    [Header("Debug")] public bool showVisionGizmo = true;

    private Rigidbody rb;
    private Animator animator;
    private Transform currentPoint;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentPoint = pointB.transform;
        currentHealth = maxHealth;
        
        // Calcular centro y rango de patrulla
        patrolCenter = (pointA.transform.position + pointB.transform.position) / 2f;
        float distAB = Vector3.Distance(pointA.transform.position, pointB.transform.position);
        patrolRange = distAB * 0.8f;

        InitialFlip();
    }

    private void Update()
    {
        if (isDead) return;
        
        Transform target = DetectPlayer();
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= retreatDistance)
            {
                RetreatFromPlayer(target);
            }
            else if (distance <= attackRange && canAttack)
            {
                Attack(target);
            }
            else
            {
                if(!isAttacking) PatrolLogic();
            }
        }
        else
        {
            if(!isAttacking) PatrolLogic();
        }

        if(!isAttacking) CheckStuck();
        UpdateAnimations();
        UpdateAttackTimer();
    }

    private void Attack(Transform target)
    {
        canAttack = false;
        isAttacking = true;
        attackTimer = attackCooldown;
        
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (dirToTarget.x != 0)
        {
            float  dirSign = Mathf.Sign(dirToTarget.x);
            float rightSign = Mathf.Sign(transform.right.x);
            if(dirSign != rightSign) Flip();
        }

        if (bulletPrefab != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            RangeEnemyBullet bulletScript = bullet.GetComponent<RangeEnemyBullet>();
            if(bulletScript != null)
            {
                Vector3 shootDir = (target.position - spawnPos).normalized;
                bulletScript.SetDirection(shootDir);
            }
        }
        
        animator.SetTrigger("Attack");
    }

    private void UpdateAttackTimer()
    {
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canAttack = true;
                isAttacking = false;
            }
        }
    }
    
    private void PatrolLogic()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;
                Flip();
            }
            return;
        }

        Vector2 direction = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * speed, 0, 0);

        // Corregir orientación si no coincide con la dirección de movimiento
        if (direction.x != 0)
        {
            float dirSign = Mathf.Sign(direction.x);
            float rightSign = Mathf.Sign(transform.right.x);
            if (dirSign != rightSign)
            {
                Flip();
            }
        }

        if (Mathf.Abs(transform.position.x - currentPoint.position.x) < 1f)
        {
            isWaiting = true;
            waitTimer = waitTime;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private Transform DetectPlayer()
    {
        Vector3 headPos = transform.position + Vector3.up * headOffset;
        Collider[] hits = Physics.OverlapSphere(headPos, viewDistance, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector3 dirToPlayer = (hit.transform.position - headPos).normalized;
                float angle = Vector3.Angle(transform.right, dirToPlayer);
                if (angle <= viewAngle * 0.5f)
                {
                    RaycastHit rayHit;
                    if (Physics.Raycast(headPos, dirToPlayer, out rayHit, viewDistance))
                    {
                        if (rayHit.collider.CompareTag("Player"))
                        {
                            //Debug.Log("Jugador detectado (sin reacción)");
                            return hit.transform;
                        }
                    }
                }
            }
        }
        return null;
    }

    private void RetreatFromPlayer(Transform target)
    {
        isRetreating = true;

        Vector3 dirAway = (transform.position - target.position).normalized;
        //dirAway.y = 0;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dirAway, out hit, wallCheckDistance, wallLayer))
        {
            return;
        }
        
        rb.linearVelocity = new Vector3(dirAway.x * speed, 0, 0);
        
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (dirToTarget.x != 0)
        {
            float  dirSign = Mathf.Sign(dirToTarget.x);
            float rightSign = Mathf.Sign(transform.right.x);
            if(dirSign != rightSign) Flip();
        }
    }

    private void CheckStuck()
    {
        if (isWaiting || isRetreating) 
        {
            stuckTimer = 0f;
            return;
        }

        float distToCenter = Vector3.Distance(transform.position, patrolCenter);
        if (distToCenter > patrolRange)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= maxStuckTime)
            {
                transform.position = patrolCenter;
                rb.linearVelocity = Vector3.zero;
                stuckTimer = 0f;
                Debug.Log("Enemigo teletransportado al centro de patrulla por estar atascado.");

                Vector3 dirToPoint = (currentPoint.position - transform.position).normalized;
                if (dirToPoint.x != 0)
                {
                    float dirSign = Mathf.Sign(dirToPoint.x);
                    float rightSign = Mathf.Sign(transform.right.x);
                    if (dirSign != rightSign) Flip();
                }
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    public void TakeDamage(int damage, bool isMelee)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector3.zero;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        TryDropItem();
        Destroy(gameObject, 1.5f);
    }
    
    void TryDropItem()
    {
        if (drops != null && drops.Length > 0 && Random.value <= dropChance)
        {
            GameObject itemToDrop = drops[Random.Range(0, drops.Length)];
            if (itemToDrop != null)
            {
                Instantiate(itemToDrop, transform.position, Quaternion.identity);
                Debug.Log("Enemigo soltó un item: " + itemToDrop.name);
            }
        }
    }

    private void Flip()
    {
        transform.Rotate(0, 180, 0);
    }

    private void InitialFlip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            bool isIdle = rb.linearVelocity.magnitude < 0.1f;
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            
            animator.SetBool("IsIdle", isIdle);
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null) Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        if (pointA != null && pointB != null) Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

        if (!showVisionGizmo || !Application.isPlaying) return;

        Vector3 headPos = transform.position + Vector3.up * headOffset;
        Vector3 forward = transform.right;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(headPos, forward * viewDistance);

        float halfAngle = viewAngle * 0.5f;
        Quaternion leftRot = Quaternion.Euler(0, 0, halfAngle);
        Quaternion rightRot = Quaternion.Euler(0, 0, -halfAngle);
        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawRay(headPos, leftDir * viewDistance);
        Gizmos.DrawRay(headPos, rightDir * viewDistance);

        Gizmos.color = new Color(1, 1, 0, 0.1f);
        int segments = 20;
        Vector3 prevPoint = headPos + leftDir * viewDistance;
        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            Vector3 dir = rot * forward;
            Vector3 point = headPos + dir * viewDistance;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}
