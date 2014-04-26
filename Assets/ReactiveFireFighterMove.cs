using UnityEngine;
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
    public float rotationAngle = 70;
    //The AI's speed per second
    public float speed = 300;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

    private ReactiveFireFighter agent;

    private Seeker seeker;
    private CharacterController controller;

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
        //Get a reference to the CharacterController to enact movement
        controller = GetComponent<CharacterController>();
        //OnPathComplete will be called every time a path is returned to this seeker
        seeker.pathCallback += OnPathComplete;
        //Generating random position
        genInicialRandomPos();
        //Starting path from transform.position to targetPosition
        seeker.StartPath(transform.position, targetPosition);

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        agent = GetComponent<ReactiveFireFighter>();

    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yey, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
        else
        {
            Debug.Log("End Of Path Reached");
            currentWaypoint = 0;
            genCompRandomPos();
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
    private void genRandomPos()
    {
        //targetPosition = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
        //targetPosition = (targetPosition - transform.position).normalized * visionRadius;

        //Vector3 right = Vector3.Cross(transform.forward,Vector3.up);
        Vector3 randomizedDir = Quaternion.AngleAxis(Random.Range(-rotationAngle, rotationAngle), Vector3.up) * transform.forward;
        Debug.Log(randomizedDir);
        targetPosition = transform.position + randomizedDir * frontRadius;//new Vector3(Random.Range(transform.position.x, transform.position.x + frontRadius), 0, Random.Range(-transform.position.z, transform.position.z + frontRadius));
    }

    public void recalculate()
    {
        currentWaypoint = 0;
        genCompRandomPos();
        seeker.StartPath(transform.position, targetPosition);
    }

    public void FixedUpdate()
    {

        gameSpeed = hub.gameSpeed;

        if (!agent.preparingToPutOutFire && !agent.collided)
        {
            if (path == null)
            {
                //We have no path to move after yet
                return;
            }
            if (currentWaypoint >= path.vectorPath.Count)
            {
                Debug.Log("End Of Path Reached");
                currentWaypoint = 0;
                genRandomPos();
                seeker.StartPath(transform.position, targetPosition);
                return;
            }
            //Direction to the next waypoint
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            dir.y = 0f;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

            if (transform.rotation != rot)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 100 * Time.fixedDeltaTime * gameSpeed);

            else
            {
                dir *= speed * gameSpeed * Time.fixedDeltaTime;
                controller.SimpleMove(dir);

                //Check if we are close enough to the next waypoint
                //If we are, proceed to follow the next waypoint

            }
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }


    }
}
