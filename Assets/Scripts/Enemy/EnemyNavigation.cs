using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    NavMeshAgent agent;
    Transform target;


    [SerializeField] bool debugPath = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    

    public void MoveTo(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
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
