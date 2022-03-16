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
    protected float rayLength = 1.2f;

    [SerializeField]
    protected float rayAngle = 4.0f;
    
    [SerializeField]
    protected float secondaryRayLength = 0.6f;
    
    [SerializeField]
    protected float secondaryRayAngle = 25.0f;
    
    [SerializeField]
    protected float avoidDistance = 2.0f;

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
        
        DetectCollision();
    }

    protected void Steer(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        //Vector3 direction = DesiredDirection;
        float distance = direction.magnitude;

        Vector3 desiredVelocity = (direction.normalized * maxSpeed);
        Velocity = Vector3.Lerp(Velocity, desiredVelocity, Time.deltaTime * maxSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);
        
        // TODO think about smooth rotation - use interpolation/curves????
        LookDirection = Vector3.Lerp(LookDirection, direction, Time.deltaTime * maxSpeed);
    }

    protected Ray CreateRay(Vector3 position, Vector3 destination, float length, Color color)
    {
        Ray ray = new Ray(transform.position, destination);
        Debug.DrawRay(ray.origin, ray.direction * length, color);

        return ray;
    }

    protected void AvoidCollision(Ray ray, RaycastHit hit)
    {
        Debug.DrawLine(ray.origin, hit.transform.position, Color.red);
        Debug.DrawLine(ray.origin, hit.point, Color.white);
        Debug.DrawLine(hit.point, hit.normal.normalized * avoidDistance, Color.yellow);

        Vector3 avoidance = hit.point + hit.normal * avoidDistance;

        Steer(avoidance);
    }

    protected void DetectCollision()
    {
        //// TODO: refactor - put all rays to the loop

        Ray leftRay = CreateRay(transform.position, Quaternion.AngleAxis(-rayAngle, Vector3.up) * transform.forward, rayLength + 0.5f, Color.blue);
        Ray rightRay = CreateRay(transform.position, Quaternion.AngleAxis(rayAngle, Vector3.up) * transform.forward, rayLength, Color.green);

        Ray secondaryLeftRay = CreateRay(transform.position, Quaternion.AngleAxis(-secondaryRayAngle, Vector3.up) * transform.forward, secondaryRayLength, Color.blue);
        Ray secondaryRightRay = CreateRay(transform.position, Quaternion.AngleAxis(secondaryRayAngle, Vector3.up) * transform.forward, secondaryRayLength, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(leftRay, out hit, rayLength))
        {
            AvoidCollision(leftRay, hit);
        }
        else if (Physics.Raycast(rightRay, out hit, rayLength))
        {
            AvoidCollision(rightRay, hit);
        }

        else if (Physics.Raycast(secondaryLeftRay, out hit, secondaryRayLength))
        {
            AvoidCollision(secondaryLeftRay, hit);
        }

        else if (Physics.Raycast(secondaryRightRay, out hit, secondaryRayLength))
        {
            AvoidCollision(secondaryRightRay, hit);
        }
        else
        {
            Steer(currentDestination);
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        Debug.DrawLine(transform.position + debugLinesOffset, 
            transform.position + debugLinesOffset + DesiredDirection.normalized, Color.black);
    }

    protected void CheckDestinationUpdate()
    {
        if (Vector3.SqrMagnitude(currentDestination - transform.position) <= destinationTolerance * destinationTolerance)
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
