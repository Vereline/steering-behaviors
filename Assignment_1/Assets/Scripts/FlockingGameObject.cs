using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGameObject : AbstractSteeringGameObject
{
    //[SerializeField]
    protected float neighbourRadius = 8.0f;

    //[SerializeField]
    protected float alignmentWeight = 0.3f;

    //[SerializeField]
    protected float cohesionWeight = 0.1f;

    //[SerializeField]
    protected float separationWeight = 20.0f;

    [SerializeField]
    protected float driveFactor = 5f;

    float squareMaxSpeed;
    float squareAvoidanceRadius;

    public FlockSpawner Spawner { get; set; }

    protected override void Start()
    {
        base.Start();
        
        Velocity = new Vector3(Random.insideUnitSphere.x, 0.0f, Random.insideUnitSphere.y).normalized * maxSpeed;
        
        movementControl = MovementControl.Manual;

        squareMaxSpeed = maxSpeed * maxSpeed;
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
        List< FlockingGameObject> neighbours = new List<FlockingGameObject>();

        Vector3 move = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;

        int nAvoid = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            if (spawnerAgent != this)
            {
                float distance = Vector3.Distance(transform.position, spawnerAgent.transform.position);
                if (distance > 0 && distance < neighbourRadius)
                {
                    neighbours.Add(spawnerAgent);
                    alignment += ComputeAlignment(spawnerAgent);

                    if (Vector3.SqrMagnitude(spawnerAgent.transform.position - transform.position) < squareAvoidanceRadius)
                    {
                        nAvoid++;
                        separation += ComputeSeparation(spawnerAgent);
                    }

                    cohesion += ComputeCohesion(spawnerAgent);
                }
            }
        }

        if (nAvoid > 0)
        {
            separation /= nAvoid;
        }

        if (neighbours.Count > 0)
        {
            cohesion /= neighbours.Count;
            cohesion -= transform.position;
            
            alignment /= neighbours.Count;
        } else
        {
            alignment = transform.forward;
        }

        move = cohesion.normalized * cohesionWeight + separation.normalized * separationWeight + alignment.normalized * alignmentWeight;

        move *= driveFactor;
        if (move.sqrMagnitude > squareMaxSpeed)
        {
            move = move.normalized * maxSpeed;
        }
        Move(move);
    }

    protected void Move(Vector3 velocity)
    {
        //transform.forward = velocity;
        LookDirection = Vector3.Lerp(LookDirection, velocity.normalized, Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected Vector3 ComputeSeparation(FlockingGameObject agent)
    {
        return transform.position - agent.transform.position;
    }

    protected Vector3 ComputeAlignment(FlockingGameObject agent)
    {
        return agent.transform.forward;
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
