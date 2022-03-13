using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected float neighbourRadius = 8.0f;

    public FlockSpawner Spawner { get; set; }

    protected override void Start()
    {
        base.Start();
        
        Velocity = new Vector3(Random.insideUnitSphere.x, 0.0f, Random.insideUnitSphere.y).normalized * maxSpeed;
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

        UpdateFlochingBehavior();
    }

    protected void UpdateFlochingBehavior()
    {
        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            // TODO
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected Vector3 ComputeSeparation()
    {
        // TODO Task 3 Flocking – separation
        return Vector3.zero;
    }

    protected Vector3 ComputeAlignment()
    {
        // TODO Task 3 Flocking – alignment
        return Vector3.zero;
    }

    protected Vector3 ComputeCohesion()
    {
        // TODO Task 3 Flocking – cohesion
        return Vector3.zero;
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
