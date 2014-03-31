using UnityEngine;
using System.Collections;

using Pathfinding;

public class ReactiveAgent : MonoBehaviour {

	public Vector3 targetPosition;
	
	private Seeker seeker;
	private CharacterController controller;
	
	//The calculated path
	public Path path;
	
	//The AI's speed per second
	public float speed = 700;
	
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
		//So now we can omit the callback parameter
		seeker.StartPath (transform.position,targetPosition);
	}

	public void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	
	public void genRandomPos(){
		targetPosition = new Vector3(Random.Range(-50.0f, 50.0f), 0, Random.Range(-50.0f, 50.0f));
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
		dir *= speed * Time.fixedDeltaTime;
		Quaternion rot = transform.rotation;
		rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));
		transform.rotation = rot;
		controller.SimpleMove (dir);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}
}
