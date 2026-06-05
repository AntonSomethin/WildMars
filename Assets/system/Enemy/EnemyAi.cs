using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private float nextFireTime;
    private float bulletGravity;
    private bool isBursting = false;
    private float strafeTimer;
    private Vector3 currentTargetPos;

    private bool wasPlayerInZone = false;
    private float reactionTimer = 0f;
    private bool isRetreating = false;

    [Header("AI Parameters")]
    public float visionRange = 25f;
    public float stopDistance = 10f;
    public float strafeInterval = 2f;
    public float initialReactionDelay = 2f;

    [Header("Guard Tactics")]
    public Transform guardAnchor;
    public float maxChaseDistanceFromAnchor = 6f;

    [Header("Weapon Setup")]
    public Weapon enemyWeapon;

    [Header("Tactics Setup")]
    [Range(1, 10)] public int burstCount = 3;
    public float minTimeBetweenBursts = 1f;
    public float maxTimeBetweenBursts = 2.5f;

    [Header("Multipliers")]
    public float damageMultiplier = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (enemyWeapon != null && enemyWeapon.bulletPrefab != null)
        {
            var bulletScript = enemyWeapon.bulletPrefab.GetComponent<BallisticBullet>();
            if (bulletScript != null)
            {
                bulletGravity = bulletScript.gravity;
            }
        }

        Health myHealth = GetComponent<Health>();
        if (myHealth != null && enemyWeapon != null)
        {
            if (enemyWeapon.weaponName.ToLower().Contains("revolver"))
            {
                myHealth.SetupLoot("revolver", 12);
            }
            else if (enemyWeapon.weaponName.ToLower().Contains("sharps"))
            {
                myHealth.SetupLoot("sharps", 10);
            }
        }
    }

    void Update()
    {
        if (player == null || enemyWeapon == null || agent == null) return;

        Vector3 anchorPos = guardAnchor != null ? guardAnchor.position : transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromAnchor = Vector3.Distance(anchorPos, transform.position);

        if (isRetreating)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(anchorPos);
            }

            if (distanceFromAnchor <= 1.5f)
            {
                isRetreating = false;
                wasPlayerInZone = false;
                reactionTimer = 0f;
            }
            return;
        }

        if (guardAnchor != null && distanceFromAnchor > maxChaseDistanceFromAnchor)
        {
            isRetreating = true;
            if (agent.isOnNavMesh) agent.isStopped = false;
            return;
        }

        bool canSeePlayer = distanceToPlayer <= visionRange;

        if (canSeePlayer)
        {
            Vector3 lookPos = player.position - transform.position;
            lookPos.y = 0;
            if (lookPos != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);
            }

            if (!wasPlayerInZone)
            {
                wasPlayerInZone = true;
                reactionTimer = 0f;
                nextFireTime = Time.time + initialReactionDelay;
            }

            reactionTimer += Time.deltaTime;

            if (reactionTimer >= initialReactionDelay)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    strafeTimer += Time.deltaTime;

                    if (strafeTimer >= strafeInterval || distanceToPlayer <= stopDistance)
                    {
                        strafeTimer = 0;

                        if (distanceToPlayer <= stopDistance)
                        {
                            Vector3 randomStrafe = transform.right * Random.Range(-4f, 4f);
                            currentTargetPos = transform.position + randomStrafe;
                        }
                        else
                        {
                            Vector3 randomOffset = transform.right * Random.Range(-2f, 2f);
                            currentTargetPos = player.position + randomOffset;
                        }
                    }

                    if (enemyWeapon.currentAmmo <= 0)
                    {
                        Vector3 retreatDir = (transform.position - player.position).normalized;
                        agent.SetDestination(transform.position + retreatDir * 3f);
                    }
                    else
                    {
                        agent.SetDestination(currentTargetPos);
                    }
                }

                if (Time.time >= nextFireTime && !isBursting)
                {
                    if (enemyWeapon.currentAmmo > 0)
                    {
                        StartCoroutine(FireBurst(distanceToPlayer));
                    }
                    else
                    {
                        enemyWeapon.ReloadAll();
                        nextFireTime = Time.time + 2f;
                    }
                }
            }
            else
            {
                if (agent.isOnNavMesh && !agent.isStopped)
                {
                    agent.isStopped = true;
                }
            }
        }
        else
        {
            wasPlayerInZone = false;

            if (agent.isOnNavMesh)
            {
                if (guardAnchor != null && distanceFromAnchor > 1.5f)
                {
                    agent.isStopped = false;
                    agent.SetDestination(anchorPos);
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
    }

    IEnumerator FireBurst(float distance)
    {
        isBursting = true;
        float dynamicFireRate = enemyWeapon.fireRate > 0 ? enemyWeapon.fireRate : 0.2f;

        for (int i = 0; i < burstCount; i++)
        {
            if (enemyWeapon.currentAmmo <= 0) break;

            ExecuteEnemyShoot(distance);
            yield return new WaitForSeconds(dynamicFireRate);
        }

        float randomDelay = Random.Range(minTimeBetweenBursts, maxTimeBetweenBursts);
        nextFireTime = Time.time + randomDelay;
        isBursting = false;
    }

    void ExecuteEnemyShoot(float distance)
    {
        if (enemyWeapon.firePoint == null || enemyWeapon.bulletPrefab == null) return;

        enemyWeapon.currentAmmo--;

        if (enemyWeapon.muzzleFlash != null)
            enemyWeapon.muzzleFlash.Play();

        if (enemyWeapon.shootSound != null)
            AudioSource.PlayClipAtPoint(enemyWeapon.shootSound, enemyWeapon.firePoint.position);

        Vector3 targetCenter = player.position + Vector3.up * 1f;
        float yOffset = targetCenter.y - enemyWeapon.firePoint.position.y;

        float angle = BallisticCalculator.GetAngle(distance, enemyWeapon.bulletSpeed > 0 ? enemyWeapon.bulletSpeed : 50f, bulletGravity, yOffset);

        Vector3 directionToPlayer = (targetCenter - enemyWeapon.firePoint.position).normalized;
        Vector3 rightAxis = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3 baseDir = Quaternion.AngleAxis(-angle, rightAxis) * directionToPlayer;

        Vector2 randomPoint = Random.insideUnitCircle * enemyWeapon.baseSpreadAngle;
        Quaternion spreadRot = Quaternion.Euler(randomPoint.x, randomPoint.y, 0);
        Vector3 finalDir = spreadRot * baseDir;

        float finalDamage = enemyWeapon.damage * damageMultiplier;

        GameObject bulletObj = Instantiate(enemyWeapon.bulletPrefab, enemyWeapon.firePoint.position, Quaternion.LookRotation(finalDir));
        bulletObj.GetComponent<BallisticBullet>().Initialize(enemyWeapon.firePoint.position, finalDir, enemyWeapon.bulletSpeed > 0 ? enemyWeapon.bulletSpeed : 50f, finalDamage);
    }
}