using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;
    bool HasPath => agent.hasPath;

    [SerializeField] bool debugPath = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

   
    public bool MoveTo(Vector3 targetPos)
    {
        return agent.SetDestination(targetPos);
    }

    // Synchronously query a path from current position to a target and report if it truly reaches it.
    public PathQueryResult QueryPathTo(Vector3 targetPos, float endTolerance = 0.25f)
    {
        var path = new NavMeshPath();
        bool ok = NavMesh.CalculatePath(agent.transform.position, targetPos, NavMesh.AllAreas, path);

        Vector3 end = agent.transform.position;
        if (path.corners != null && path.corners.Length > 0)
        {
            end = path.corners[path.corners.Length - 1];
        }

        var status = path.status;
        bool reachesTarget = ok && status == NavMeshPathStatus.PathComplete &&
                             (end - targetPos).sqrMagnitude <= (endTolerance * endTolerance);

        if (debugPath && path.corners != null && path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], reachesTarget ? Color.green : Color.yellow);
            }
        }

        return new PathQueryResult
        {
            Found = ok,
            Status = status,
            EndPosition = end,
            ReachesTarget = reachesTarget
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath && !debugPath)
        {
            return;
        }

        NavMeshPath path = agent.path;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    public bool HasReachedDestination()
    {
        if (agent.pathPending)
        {
            return false;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }
}

public struct PathQueryResult
{
    public bool Found;                  // CalculatePath succeeded
    public NavMeshPathStatus Status;    // Complete / Partial / Invalid
    public Vector3 EndPosition;         // End of the computed path
    public bool ReachesTarget;          // True only if the path actually reaches the target
}
