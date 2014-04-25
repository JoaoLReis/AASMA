using UnityEngine;
using System.Collections;

using Pathfinding;

public class ReactiveAgent : MonoBehaviour {

	/****GENERAL VARIABLES****/
	public int MaxWater = 30;
	private int currentWater = 15;

    private Transform barrelEnd;
    private GameObject waterJetprefab;
    private GameObject waterJet;
    private float waterJetLifeTime = 1;
	//private float waterBarLength = (Screen.width / 6);

	/******GUI FUNCTIONS*****/
    
    void OnGUI()
	{
		Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Box(new Rect(targetPos.x, Screen.height- targetPos.y, 60, 20), currentWater + "/" + MaxWater);
	}

	public void AddjustCurrentWater(int adj) {	
		currentWater += adj;
		if (currentWater < 0)	
			currentWater = 0;

		if (currentWater > MaxWater)
			currentWater = MaxWater;

		if(MaxWater < 1)
			MaxWater = 1;

		//waterBarLength = (Screen.width / 6) * (currentWater /(float)MaxWater);
	}

    private void decreaseWater(int amount)
    {
        if (currentWater - amount < 0)
            currentWater = 0;
        else
            currentWater -= amount;
    }

    public void refillWater()
    {
        currentWater = MaxWater;
    }

    private IEnumerator decreaseFireHealth(GameObject fire, int amount)
    {
        while (fire != null)
        {
            fire.GetComponent<FireStats>().decreaseHealth(1);
            decreaseWater(1);
            yield return new WaitForSeconds(1 / gameSpeed);
        }
        puttingFireOut = false;
        Destroy(waterJet);
    }

    public void fireSensor(GameObject bOnFire)
    {
        if (waterJet == null && currentWater > 0)
        {
            attendFire(bOnFire);
        }
    }

    private void attendFire(GameObject bOnFire)
    {
        Debug.LogWarning("AttendingFire");
        puttingFireOut = true;
        targetPosition = transform.position;
        GameObject fire = bOnFire.GetComponent<BuildingScript>().getFire();
        transform.LookAt(new Vector3(fire.transform.position.x, transform.position.y, fire.transform.position.z));
        barrelEnd.LookAt(fire.transform.position);
        waterJet = (GameObject)Instantiate(waterJetprefab, barrelEnd.position, barrelEnd.rotation);
        waterJet.particleSystem.startSpeed = (fire.transform.position - transform.position).magnitude *gameSpeed;
        waterJet.particleSystem.startLifetime = waterJetLifeTime / gameSpeed;
        StartCoroutine(decreaseFireHealth(fire, 1));
    }

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

    private Hub hub;
    private int gameSpeed = 1;

    private bool puttingFireOut = false; 
	
	private Seeker seeker;
	private CharacterController controller;
	
	//The calculated path
	public Path path;
	
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	public void Start () {
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
        
        //Initialize some objects
        barrelEnd = FindChild("BarrelEnd");
        waterJetprefab = (GameObject)Resources.Load("Prefab/Water Jet");

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        waterJetLifeTime = waterJetprefab.particleSystem.startLifetime;

	}

	public void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
		else {
			Debug.Log ("End Of Path Reached");
			currentWaypoint = 0;
			genCompRandomPos();
			seeker.StartPath (transform.position,targetPosition);
		}
	}

	//Generates a completly new random position based of the visionRadius.
	private void genInicialRandomPos() {
		targetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + targetPosition.x, 0, Random.Range(-visionRadius, visionRadius) + targetPosition.z);
	}
	
	//Generates a completly new random position based of the visionRadius.
	private void genCompRandomPos() {
		targetPosition = new Vector3(Random.Range(-visionRadius, visionRadius) + transform.position.x, 0, Random.Range(-visionRadius, visionRadius) + transform.position.z);
	}

	//Generates a random position based on previous direction and position
	private void genRandomPos() {
		//targetPosition = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
		//targetPosition = (targetPosition - transform.position).normalized * visionRadius;

		//Vector3 right = Vector3.Cross(transform.forward,Vector3.up);
		Vector3 randomizedDir = Quaternion.AngleAxis(Random.Range(-rotationAngle, rotationAngle), Vector3.up) * transform.forward;
		Debug.Log(randomizedDir);
		targetPosition = transform.position + randomizedDir * frontRadius;//new Vector3(Random.Range(transform.position.x, transform.position.x + frontRadius), 0, Random.Range(-transform.position.z, transform.position.z + frontRadius));
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		if (hit.transform.tag == "Agent" || hit.transform.tag == "Obstacle"){
            transform.Rotate(Vector3.up, -90);
			currentWaypoint = 0;
			genCompRandomPos();
			seeker.StartPath (transform.position,targetPosition);
			return;
		}
	}

	public void FixedUpdate () {

        gameSpeed = hub.gameSpeed;

        if (!puttingFireOut)
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
            transform.rotation = rot;
            dir *= speed * gameSpeed * Time.fixedDeltaTime;
            controller.SimpleMove(dir);
            //Check if we are close enough to the next waypoint
            //If we are, proceed to follow the next waypoint
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            {
                currentWaypoint++;
                return;
            }
        }

	}

    private Transform FindChild(string name)
    {
        Transform[] trans = GetComponentsInChildren<Transform>();

        foreach (Transform t in trans)
        {
            if (t.gameObject.name.Equals(name))
                return t;
        }
        return null;
    }
}
