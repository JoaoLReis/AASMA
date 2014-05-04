﻿using UnityEngine;
using System.Collections;

using Pathfinding;

public class ReactiveFireFighterMove : MonoBehaviour {

    /************************/

    /*PATHFINDING VARIABLES AND FUNCTIONS**/
    //Objective position.
    private Vector3 targetPosition;
    //Int Radius that the unit will be able to see around.
    public int visionRadius = 10;
    //Forward radius wich the unit will randomly try to move.
    public float frontRadius = 6;
    //RotationAngle.
    public float rotationAngle = 60;
    //The AI's speed per second
    public float speed = 10;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

    private ReactiveFireFighter agent;

    private Seeker seeker;

    //The calculated path
    public Path path;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    public void Start()
    {
        //Get a reference to the Seeker component we added earlier
        seeker = GetComponent<Seeker>();
        //OnPathComplete will be called every time a path is returned to this seeker
        seeker.pathCallback += OnPathComplete;
        //Generating random position
        genCompRandomPos();
        //Starting path from transform.position to targetPosition
        seeker.StartPath(transform.position, targetPosition);

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        agent = GetComponent<ReactiveFireFighter>();

    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
        else
        {
            //Debug.Log("End Of Path Reached");
            currentWaypoint = 0;
			genRandomPos(false);
            seeker.StartPath(transform.position, targetPosition);
        }
    }

    //Generates a completly new random position based of the visionRadius.
    private void genInicialRandomPos()
    {
        targetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + targetPosition.x, 0, Random.Range(-visionRadius, visionRadius) + targetPosition.z);
    }

    //Generates a completly new random position based of the visionRadius.
    private void genCompRandomPos()
    {
        targetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + transform.position.x, 0, Random.Range(-visionRadius, visionRadius) + transform.position.z);
    }

    //Generates a random position based on previous direction and position
    private void genRandomPos(bool right)
    {
        Vector3 randomizedDir, tmpTargetPosition;


        if (right)
        {
            randomizedDir = Quaternion.AngleAxis(Random.Range(0f, rotationAngle), Vector3.up) * transform.forward;
            tmpTargetPosition = transform.position + randomizedDir * frontRadius;
        }
        else
        {
            randomizedDir = Quaternion.AngleAxis(Random.Range(-rotationAngle, rotationAngle), Vector3.up) * transform.forward;
            tmpTargetPosition = transform.position + randomizedDir * frontRadius;
        }

        Pathfinding.NNInfo node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default);
        Pathfinding.NNInfo node2 = AstarPath.active.GetNearest(tmpTargetPosition, NNConstraint.Default);
       /* while (!Pathfinding.GraphUpdateUtilities.IsPathPossible(node1.node, node2.node))
        {
            Debug.Log("Oh noes, there is no path between those nodes!");
            if (right)
            {
                tmpTargetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + transform.position.x, 0, Random.Range(-visionRadius, visionRadius) + transform.position.z);
            }
            else
            {
                tmpTargetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + transform.position.x, 0, Random.Range(-visionRadius, visionRadius) + transform.position.z);
            }
            node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.Default);
            node2 = AstarPath.active.GetNearest(tmpTargetPosition, NNConstraint.Default);
        }*/
        targetPosition = tmpTargetPosition;
    }

    public void recalculateRight()
    {
        currentWaypoint = 0;
        genRandomPos(true);
        seeker.StartPath(transform.position, targetPosition);
    }

    public void recalculate()
    {
        currentWaypoint = 0;
        genRandomPos(false);
        seeker.StartPath(transform.position, targetPosition);
    }

    public bool checkEndOfPath()
    {
        if (path == null)
        {
            //We have no path to move after yet
            return false;
        }
        if (currentWaypoint > path.vectorPath.Count)
            return false;
        if (currentWaypoint == path.vectorPath.Count)
        {
            //Debug.Log("End Of Path Reached");
            currentWaypoint = 0;
            genRandomPos(false);
            seeker.StartPath(transform.position, targetPosition);
            return false;
        }
        return true;
    }

    public void testActivity()
    {
        if (!agent.preparingToPutOutFire && !agent.collided)
        {
            if(checkEndOfPath())
            {
                //Direction to the next waypoint
                Vector3 dir = new Vector3(0f, 0f, 0f);
                dir = (path.vectorPath[currentWaypoint] - transform.position);
                dir.y = 0f;
                if (dir.x != dir.x || dir.y != dir.y || dir.z != dir.z)
                    dir = transform.forward;

                Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 1.5f * Time.fixedDeltaTime * gameSpeed, 360);

                Quaternion rot = transform.rotation;
                rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

                if (dir != newdir)
                {
                    if (Quaternion.Angle(transform.rotation, rot) > 20f)
                        transform.rotation = Quaternion.LookRotation(newdir);
                    else transform.rotation = Quaternion.LookRotation(dir);
                }
                else
                {
                    agent.readyToMove = true;
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * gameSpeed * Time.fixedDeltaTime);


                    //Check if we are close enough to the next waypoint
                    //If we are, proceed to follow the next waypoint

                }
                if ((transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance)
                //if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                {
                    currentWaypoint++;
                    return;
                }
            }
        }
    }

    public bool testRefill()
    {
        if (agent.preparingToRefill)
        {
            Vector3 d = Vector3.RotateTowards(transform.forward, agent.refillPosition - transform.position, 1.5f * Time.fixedDeltaTime * gameSpeed, 360);
            d.y = 0f;
            Quaternion rot = transform.rotation;
            rot.SetLookRotation(d, new Vector3(0f, 1f, 0f));
            if (Vector3.Angle(transform.forward, d) < 1f)
            {
                agent.preparingToRefill = false;
                agent.moveToRefill = true;
            }
            transform.rotation = rot;
            return true;
        }
        else if (agent.moveToRefill)
        {
            Vector3 tmp = agent.refillPosition;
            tmp.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, tmp, 3.5f * Time.fixedDeltaTime * gameSpeed);
            if ((agent.refillPosition - transform.position).magnitude < 1f)
            {
                agent.moveToRefill = false;
                agent.reffiling = true;
            }
            return true;
        }
        else if(agent.reffiling)
        {
            if(!agent.AddjustCurrentWater(1))
            {
                return true;
            }
            else
            {
                agent.reffiling = false;
            }
        }
        return false;   
    }

    public void FixedUpdate()
    {
        gameSpeed = hub.gameSpeed;
        if (!agent.collided)
        {
            if (!testRefill())
            {
                testActivity();
            }
        }   
    }
}
