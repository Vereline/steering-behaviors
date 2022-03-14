using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingGameObject : AbstractSteeringGameObject
{
    [SerializeField]
    protected float neighbourRadius = 8.0f;

    protected Vector3 acceleration, location;
    protected float maxForce = 3.0f;

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


    private void Separate()
    {
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            float d = Vector3.Distance(location, spawnerAgent.transform.position);

            if (d > 0 && d < neighbourRadius)
            {
                Vector3 diff = location - spawnerAgent.transform.position;
                diff.Normalize();
                diff /= d;
                separationForce += diff;
                count++;
            }
        }

        if (count > 0) separationForce /= count;

        if (separationForce.magnitude > 0)
        {
            separationForce.Normalize();
            separationForce *= maxSpeed;
            separationForce = Vector3.ClampMagnitude(separationForce - Velocity, maxForce);
            separationForce *= 5;
            ApplyForce(separationForce);
        }
    }

    private void Align()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            float d = Vector3.Distance(transform.position, spawnerAgent.transform.position);

            if (d > 0 && d < neighbourRadius)
            {
                sum += spawnerAgent.Velocity;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxSpeed;
            Vector3 alignmentForce = sum - Velocity;
            alignmentForce = Vector3.ClampMagnitude(alignmentForce, maxForce);
            ApplyForce(alignmentForce);
        }
    }

    private void Cohesion()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            float d = Vector3.Distance(location, spawnerAgent.transform.position);

            if (d > 0 && d < neighbourRadius)
            {
                sum += spawnerAgent.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            sum /= count;
            Steer(sum);
        }
    }
    protected void UpdateFlockingBehavior()
    {
        //foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        //{
        // TODO

        //Vector3 cohesion = ComputeCohesion(spawnerAgent);
        //Vector3 alignment = ComputeAlignment(spawnerAgent);
        //Vector3 separation = ComputeSeparation(spawnerAgent);

        //Vector3 velocity = cohesion + alignment + separation;

        //Velocity = Vector3.MoveTowards(Velocity, velocity, maxSpeed);
        //LookDirection = Vector3.Lerp(LookDirection, velocity.normalized, Time.deltaTime * maxSpeed);
        //}

        //Vector3 cohesion = Vector3.zero;
        //Vector3 alignment = Vector3.zero;
        //Vector3 separation = Vector3.zero;

        //int neighbours = 0;

        //foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        //{
        //    if (spawnerAgent != this)
        //    {
        //        float distance = Vector3.Distance(transform.position, spawnerAgent.transform.position);
        //        if (distance < neighbourRadius)
        //        {
        //            separation += Vector3.Normalize(transform.position - spawnerAgent.transform.position) / (distance * distance);

        //            alignment += spawnerAgent.Velocity;

        //            separation += spawnerAgent.transform.position;
        //        }
        //    }
        //}

        //separation = separation.normalized;
        //if (neighbours > 0)
        //{
        //    alignment /= neighbours;

        //    separation /= neighbours;
        //    //separation -= transform.position;
        //}

        //alignment = (alignment - Velocity).normalized;

        //Vector3 velocity = cohesion + alignment + separation;

        //Velocity = Vector3.MoveTowards(Velocity, velocity, Time.deltaTime);
        //LookDirection = Vector3.Lerp(LookDirection, velocity.normalized, Time.deltaTime);

        //Steer();

        //Align();
        //Separate();
        //Cohesion();
        //ApplySteeringToMotion();
    }
    protected void ApplySteeringToMotion()
    {
        //add the acceleration to the velocity as we want acceleration a.k.a the force to act on the agent
        //acceleration is the rate of change of velocity and therefore, we want it to effect the velocity and change it.
        //limit the magnitude of the velocity to the user defined maximum speed
        Velocity = Vector3.ClampMagnitude(Velocity + acceleration, maxSpeed);

        //the location of the agent is calculated after the steering force is added to it's velocity
        //add the velocity to the location as velocity is the rate of change in location
        //multiply it with Time.deltaTime to get a smooth transition
        location += Velocity * Time.deltaTime;

        //it is important to set the acceleration to zero as we dont want the acceleration (the force)
        //to pile up and get too large eventually making it very hard for the agent to move
        //before the start of the next frame we want to get rid of all the forces from the previous frame
        //and calculate a new net force to act on the agent in the next frame.
        acceleration = Vector3.zero;
        LookDirection = Vector3.Lerp(LookDirection, Velocity.normalized, Time.deltaTime * maxSpeed);
        //make the agent rotate towards the target
        //RotateTowardTarget();

        //set the new position to the calculated location based on the velocity and steering force
        //this causes the agent to steer and smoothly move towards the targets location every frame
        //transform.position = location;
    }


    protected void ApplyForce(Vector3 force)
    {
        //acceleration is a force, and this is the only kind of force we want to be acting on the agent
        //the steering force is thus added to the acceleration which is the net force acting on the agent
        acceleration += force;
    }

    protected virtual void Steer(Vector3 targetPosition)
    {
        //Calculate the desired velocity, which is a vector pointing from the target to the position of this object
        //Note that the position is refering to the position of the agent in the previous frame as this function
        //is called before the position of the agent in the current frame is set.
        Vector3 desiredVelocity = targetPosition - location;

        //get a unit lenght vector as all we want is the direction of the desired velocity, which points to the target's location
        desiredVelocity.Normalize();

        //and then scale it by max speed to control how fast the agent moves towards this desired velocity (target's position)
        desiredVelocity *= maxSpeed;


        //make sure the steering force does not exceed the limit of the maxforce
        //what this means is that we don't want the magnitude of the steering force to be too large
        //causing a rapid change in the motion of the agent. In order to achieve a consistent and smooth motion
        //we limit the magnitude of the steering force to make sure it does not exceed the user defined maxforce.
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - Velocity, maxSpeed);

        //call the function to apply the steering force
        ApplyForce(steer);

        Velocity = Vector3.ClampMagnitude(Velocity + acceleration, maxSpeed);
        LookDirection = Vector3.Lerp(LookDirection, Velocity.normalized, Time.deltaTime);
    }

    protected void Seek(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        Vector3 desiredVelocity = (direction.normalized * maxSpeed);
        Velocity = Vector3.Lerp(Velocity, desiredVelocity, Time.deltaTime * maxSpeed);

        // TODO think about smooth rotation - use interpolation/curves????
        LookDirection = Vector3.Lerp(LookDirection, direction, Time.deltaTime * maxSpeed);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected Vector3 ComputeSeparation(FlockingGameObject agent)
    {
        // TODO Task 3 Flocking – separation
        Vector3 position = Vector3.zero;
        //int neighbours = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            if (spawnerAgent != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, spawnerAgent.transform.position);
                if (distance < neighbourRadius)
                {
                    //Vector3 diff = agent.transform.position - spawnerAgent.transform.position;
                    //diff.Normalize();
                    //diff /= d;

                    position += Vector3.Normalize(agent.transform.position - spawnerAgent.transform.position) / (distance * distance);
                    //neighbours++;
                }
            }
        }
       
        return position.normalized;
    }

    protected Vector3 ComputeAlignment(FlockingGameObject agent)
    {
        // TODO Task 3 Flocking – alignment
        Vector3 velocity = Vector3.zero;
        int neighbours = 0;

        foreach(FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            if (spawnerAgent != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, spawnerAgent.transform.position);
                if (distance < neighbourRadius)
                {
                    velocity += spawnerAgent.Velocity;
                    neighbours++;
                }
            }
        }

        if (neighbours == 0)
        {
            return velocity;
        }

        velocity /= neighbours;

        return (velocity - Velocity).normalized;
    }

    protected Vector3 ComputeCohesion(FlockingGameObject agent)
    {
        // TODO Task 3 Flocking – cohesion
        Vector3 position = Vector3.zero;
        int neighbours = 0;

        foreach (FlockingGameObject spawnerAgent in Spawner.Agents)
        {
            if (spawnerAgent != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, spawnerAgent.transform.position);
                if (distance < neighbourRadius)
                {
                    position += spawnerAgent.transform.position;
                    neighbours++;
                }
            }
        }

        if (neighbours == 0)
        {
            return position;
        }

        position /= neighbours;
        position -= agent.transform.position;

        return position;
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
