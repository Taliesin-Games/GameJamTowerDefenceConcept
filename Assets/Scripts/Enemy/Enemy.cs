using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

[RequireComponent(typeof(EnemyNavigation))
]
[RequireComponent(typeof(Health))
]
public class Enemy : MonoBehaviour
{

    static int count = 0;
    public static int EnemyCount => count;

    enum EnemyState
    {
        Idle,      // not moving or attacking
        Walking,   // moving to a static target (goal/tower)
        Chasing,   // chasing a moving target (player)
        Attacking  // attacking current target (goal/tower/player)
    }
    enum TargetKind
    {
        None,
        Goal,
        Player,
        Tower
    }

    #region Confguration
    //[SerializeField] GameObject goal;
    [SerializeField] float attackRange = 1.75f;            // how close we need to be to start attacking
    [SerializeField] float chaseRepathInterval = 0.2f;      // how often to re-issue paths while chasing
    [SerializeField] float goalPathRecheckInterval = 0.25f; // how often to verify goal path while walking
    [SerializeField] float pathEndTolerance = 0.25f;        // how close the path end must be to the target to count as "reaches target"
    [SerializeField] bool drawDebug = false;
    #endregion

    #region Cached References
    EnemyNavigation enemyNavigation;
    #endregion


    #region Runtime Variables
    GameObject goal;
    Transform currentTarget;
    EnemyState currentState = EnemyState.Idle;
    TargetKind currentTargetKind = TargetKind.None;

    float chaseRepathTimer = 0f;
    float goalPathRecheckTimer = 0f;
    #endregion

    private void OnEnable()
    {
        count++;
    }

    void Awake()
    {
        enemyNavigation = GetComponent<EnemyNavigation>();
        goal = EnemySpawner.EnemyGoal;

    }

    void Update()
    {
        // Here for debugging to manually trigger death
        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                TickIdle();
                break;

            case EnemyState.Walking:
                TickWalking();
                break;

            case EnemyState.Chasing:
                TickChasing();
                break;

            case EnemyState.Attacking:
                TickAttacking();
                break;
        }
    }

    void TickIdle()
    {
        // 1) Check if the path truly reaches the goal (not partial to the edge of the NavMesh)
        if (goal != null)
        {
            var goalQuery = enemyNavigation.QueryPathTo(goal.transform.position, pathEndTolerance);
            if (goalQuery.ReachesTarget)
            {
                // Valid full path to goal -> prioritize the goal
                enemyNavigation.MoveTo(goal.transform.position);
                BeginPursuit(goal.transform, TargetKind.Goal, EnemyState.Walking);
                return;
            }

            // 2) Path to goal is blocked/partial: find something to clear the way near the end of the blocked path
            if (TryFindNearestAttackable(out var nearest, out var kind, goalQuery.EndPosition))
            {
                if (kind == TargetKind.Player)
                {
                    BeginPursuit(nearest.transform, TargetKind.Player, EnemyState.Chasing);
                    return;
                }

                // Tower: walk to it to clear the path
                if (enemyNavigation.MoveTo(nearest.transform.position))
                {
                    BeginPursuit(nearest.transform, TargetKind.Tower, EnemyState.Walking);
                    return;
                }
            }
        }
    }
    void TickWalking()
    {
        // If target disappeared while walking, reset.
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        // Reached our static destination?
        if (enemyNavigation.HasReachedDestination())
        {
            currentState = EnemyState.Attacking;
            return;
        }

        // While walking to the goal, periodically verify the path still truly reaches it.
        if (currentTargetKind == TargetKind.Goal)
        {
            goalPathRecheckTimer -= Time.deltaTime;
            if (goalPathRecheckTimer <= 0f)
            {
                var goalQuery = enemyNavigation.QueryPathTo(currentTarget.position, pathEndTolerance);
                if (!goalQuery.ReachesTarget)
                {
                    // Path became partial/invalid -> reset to Idle so we can pick an attackable near the blocked end next frame
                    ResetTarget();
                    return;
                }
                goalPathRecheckTimer = goalPathRecheckInterval;
            }
        }
    }
    void TickChasing()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        // Re-path periodically towards the moving target
        chaseRepathTimer -= Time.deltaTime;
        if (chaseRepathTimer <= 0f)
        {
            enemyNavigation.MoveTo(currentTarget.position);
            chaseRepathTimer = chaseRepathInterval;
        }

        // If within attack range, start attacking
        if (IsWithinAttackRange(currentTarget.position))
        {
            currentState = EnemyState.Attacking;
            return;
        }
    }
    void TickAttacking()
    {
        // If target vanished (destroyed), reset and try again.
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        // Face target (optional)
        Vector3 toTarget = currentTarget.position - transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget), 10f * Time.deltaTime);
        }

        // Ensure we are in range; if not, resume appropriate movement.
        if (!IsWithinAttackRange(currentTarget.position))
        {
            if (currentTargetKind == TargetKind.Player)
            {
                currentState = EnemyState.Chasing;
                return;
            }
            else
            {
                // For goal/tower, walk back into range
                if (enemyNavigation.MoveTo(currentTarget.position))
                {
                    currentState = EnemyState.Walking;
                    return;
                }
                else
                {
                    // If we can no longer path to the static target, reset and try other options
                    ResetTarget();
                    return;
                }
            }
        }

        // TODO: Implement actual attack logic (deal damage, attack rate, etc.)
        if (drawDebug)
        {
            DebugDrawCircle(transform.position, attackRange, Color.red);
        }
    }
    void BeginPursuit(Transform target, TargetKind kind, EnemyState state)
    {
        currentTarget = target;
        currentTargetKind = kind;
        currentState = state;
        chaseRepathTimer = 0f;

        if (kind == TargetKind.Goal && state == EnemyState.Walking)
            goalPathRecheckTimer = 0f;
    }
    void ResetTarget()
    {
        currentTarget = null;
        currentTargetKind = TargetKind.None;
        goalPathRecheckTimer = 0f;
        currentState = EnemyState.Idle;
    }
    bool IsWithinAttackRange(Vector3 targetPos)
    {
        return (targetPos - transform.position).sqrMagnitude <= (attackRange * attackRange);
    }

    // Overload that searches from a specific origin (e.g., the end of a blocked goal path)
    bool TryFindNearestAttackable(out GameObject target, out TargetKind kind, Vector3 origin)
    {
        target = null;
        kind = TargetKind.None;

        GameObject[] candidates;
        try
        {
            candidates = GameObject.FindGameObjectsWithTag("Attackable");
        }
        catch
        {
            return false;
        }

        float minDistSqr = float.MaxValue;
        foreach (var obj in candidates)
        {
            if (obj == null || obj == gameObject) continue;

            float distSqr = (obj.transform.position - origin).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                target = obj;
            }
        }

        if (target == null) return false;

        var likelyAgent = target.GetComponentInParent<CharacterController>();
        if (likelyAgent != null && likelyAgent.gameObject != gameObject)
        {
            kind = TargetKind.Player;
        }
        else
        {
            if (target.name.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0)
                kind = TargetKind.Player;
            else
                kind = TargetKind.Tower;
        }
        return true;
    }

    // Default origin = enemy position (retains previous behavior when needed)
    bool TryFindNearestAttackable(out GameObject target, out TargetKind kind)
    {
        return TryFindNearestAttackable(out target, out kind, transform.position);
    }
    public void Die()
    {
        Debug.Log($"{gameObject.name} (Enemy) is handling death logic.");
        count--;
        // Example: play animation, spawn loot, disable AI, etc.
        Destroy(gameObject, 1f); // simple cleanup for demo
    }

    private void OnDestroy()
    {
        GameManager.Instance.CheckWinGame();
    }

    // Small helper to visualize attack range when debugging
    void DebugDrawCircle(Vector3 center, float radius, Color color, int segments = 24)
    {
        if (!drawDebug) return;
        Vector3 prev = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float ang = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(ang) * radius, 0, Mathf.Sin(ang) * radius);
            Debug.DrawLine(prev, next, color);
            prev = next;
        }
    }



}
