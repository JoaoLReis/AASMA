using UnityEngine;
using System.Collections;

using Pathfinding;

public class ReactiveAgent : MonoBehaviour {

	/****GENERAL VARIABLES****/
	public int MaxWater = 30;
	private int currentWater = 15;
	//private float waterBarLength = (Screen.width / 6);

	/******GUI FUNCTIONS*****/
	void OnGUI()
	{		
		Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Box(new Rect(targetPos.x, Screen.height- targetPos.y, 60, 20), currentWater + "/" + MaxWater);
	}

	//Temporary function.
	void Update()
	{
		AddjustCurrentWater(0);
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
	/************************/

	/*PATHFINDING VARIABLES AND FUNCTIONS**/
	//Objective position.
	private Vector3 targetPosition;
	//Int Radius that the unit will be able to see around.
	public int visionRadius;
	//Forward radius wich the unit will randomly try to move.
	public float frontRadius;
	//RotationAngle.
	public float rotationAngle = 70;
	//The AI's speed per second
	public float speed = 700;
	
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
		seeker.StartPath (transform.position,targetPosition);
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

	public void attendFire(GameObject bOnFire) {
		bOnFire.GetComponent<BuildingScript>().putOutFire();
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		if (hit.transform.tag == "Agent" || hit.transform.tag == "Obstacle"){
			currentWaypoint = 0;
			genCompRandomPos();
			seeker.StartPath (transform.position,targetPosition);
			return;
		}
	}

	public void FixedUpdate () {
		if (path == null) {
			//We have no path to move after yet
			return;
		}
		if (currentWaypoint >= path.vectorPath.Count) {
			Debug.Log ("End Of Path Reached");
			currentWaypoint = 0;
			genRandomPos();
			seeker.StartPath (transform.position,targetPosition);
			return;
		}
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir.y = 0f;
		Quaternion rot = transform.rotation;
		rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));
		transform.rotation = rot;
		dir *= speed * Time.fixedDeltaTime;
		controller.SimpleMove (dir);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
