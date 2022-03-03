using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sphere
{
    public Vector3 WorldCenter { get; set; }

    public float Radius { get; set; }
}

public class ObstacleAvoidanceGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected LayerMask obstacleLayer;

    [SerializeField]
    protected float destinationTolerance = 1.5f;

    [SerializeField]
    protected Transform destinationsParent;

    protected Vector3 currentDestination;

    protected Sphere[] obstacles;

    protected Vector3[] destinationLocations;

    protected Vector3 DesiredDirection => (currentDestination - transform.position).normalized;

    protected override void Start()
    {
        base.Start();

        currentDestination = transform.position;

        // It is not best practice to use LINQ in Unity but it is not a big deal in this case.
        // For the curious ones, the reason to rather avoid LINQ in Unity is bad performance & garbage collection.
        obstacles = FindObjectsOfType<SphereCollider>()
            .Where(x => obstacleLayer == (obstacleLayer | (1 << x.gameObject.layer)))
            .Select(x => new Sphere
            {
                WorldCenter = x.transform.TransformPoint(x.center),
                Radius = x.bounds.extents.x
            })
            .ToArray();

        destinationLocations = destinationsParent.GetComponentsInChildren<Transform>()
            .Where(x => x != destinationsParent)
            .Select(x => x.position)
            .ToArray();
    }

    protected override void Update()
    {
        base.Update();
        CheckDestinationUpdate();

        // TODO Task 2 Obstacle Avoidance 
        //      Information about sphere obstacles is stored in "obstacles" array.
        //      The variable "desiredDirection" holds information about the direction in which the agent wants to move.
        //      Set the final velocity to "Velocity" property. The maximum speed of the agent is determined by "maxSpeed".
        //      In case you would prefer to modify the transform.position directly, you can change the movementControl to Manual (see AbstractSteeringGameObject class for info).
        //      Feel free to extend the codebase. However, make sure it is easy to find your solution.
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        Debug.DrawLine(transform.position + debugLinesOffset, 
            transform.position + debugLinesOffset + DesiredDirection.normalized, Color.black);
    }

    protected void CheckDestinationUpdate()
    {
        if(Vector3.SqrMagnitude(currentDestination - transform.position) <= destinationTolerance * destinationTolerance)
        {
            currentDestination = destinationLocations[Random.Range(0, destinationLocations.Length)];
        }
    }

    public override void SetDebugObjectsState(bool newState)
    {
        base.SetDebugObjectsState(newState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(currentDestination, currentDestination + 
            new Vector3(0.0f, 2.0f, 0.0f));
    }
}
