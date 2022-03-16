using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected float neighbourRadius = 8.0f;

    protected Vector3 acceleration, location;

    [SerializeField]
    protected float alignmentWeight = 0.5f;

    [SerializeField]
    protected float cohesionWeight = 0.3f;

    [SerializeField]
    protected float separationWeight = 0.5f;
    
    public FlockSpawner Spawner { get; set; }

    protected override void Start()
    {
        base.Start();
        
        Velocity = new Vector3(Random.insideUnitSphere.x, 0.0f, Random.insideUnitSphere.y).normalized * maxSpeed;

        acceleration = Vector3.zero;
        location = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        // TODO Task 3 Flocking
        //      Use methods below to compute individual steering behaviors.
        //      Information about other agents is stored in "Spawner.Agents".
        //      The variable "neighbourRadius" holds the radius which should be used for neighbour detection.
        //      Set the final velocity to "Velocity" property. The maximum speed of the agent is determined by "maxSpeed".
        //      In case you would prefer to modify the transform.position directly, you can change the movementControl to Manual (see AbstractSteeringGameObject class for info).
        //      Feel free to extend the codebase. However, make sure it is easy to find your solution.

        UpdateFlockingBehavior();
    }

    protected void UpdateFlockingBehavior()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;

        int neighbours = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            if (spawnerAgent != this)
            {
                float distance = Vector3.Distance(transform.position, spawnerAgent.transform.position);
                if (distance > 0 && distance < neighbourRadius)
                {
                    neighbours++;
                    alignment += spawnerAgent.Velocity;
                    cohesion += spawnerAgent.transform.position;
                    separation += spawnerAgent.transform.position - transform.position;
                }
            }
        }

        if (neighbours > 0)
        {
            alignment /= neighbours;
            alignment.Normalize();

            cohesion /= neighbours;
            cohesion -= transform.position;
            cohesion.Normalize();

            separation /= neighbours;
            separation *= -1;
            separation.Normalize();
        }

        Vector3 desiredVelocity = cohesion * cohesionWeight + alignment * alignmentWeight + separation * separationWeight;
        Velocity = Vector3.MoveTowards(Velocity, desiredVelocity, Time.deltaTime);
        LookDirection = Vector3.Lerp(LookDirection, desiredVelocity.normalized, Time.deltaTime);
    }

    protected void Seek(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        Vector3 desiredVelocity = (direction.normalized * maxSpeed);
        Velocity = Vector3.Lerp(Velocity, desiredVelocity, Time.deltaTime * maxSpeed * 5);

        // TODO think about smooth rotation - use interpolation/curves????
        LookDirection = Vector3.Lerp(LookDirection, direction, Time.deltaTime * maxSpeed * 5);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected Vector3 ComputeSeparation(FlockingGameObject agent)
    {
        return agent.transform.position - transform.position;
    }

    protected Vector3 ComputeAlignment(FlockingGameObject agent)
    {
        return agent.Velocity;
    }

    protected Vector3 ComputeCohesion(FlockingGameObject agent)
    {
        return agent.transform.position;
    }

    public override void SetDebugObjectsState(bool newState)
    {
        base.SetDebugObjectsState(newState);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, neighbourRadius);
    }
}
