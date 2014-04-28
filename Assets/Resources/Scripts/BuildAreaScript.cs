using UnityEngine;
using System.Collections;

public class BuildAreaScript : MonoBehaviour {

	public GameObject building;

	public float maxCompletion = 10;
	private float completed;

	// Use this for initialization
	void Start () {
		completed = 0;
		building = Instantiate(Resources.Load("Prefab/placeHolder")) as GameObject; 
		building.transform.parent = transform;
		building.transform.position = new Vector3(transform.position.x, transform.position.y-10.0f, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.position;  
	}

	public bool buildArea(int value)
	{
        if (completed == maxCompletion)
        {
            Destroy(building);
            building = Instantiate(Resources.Load("Prefab/Obstacle")) as GameObject;
            building.transform.parent = transform;
            building.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            return true;
        }
        else
        {
            completed++;
            building.transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y - 10.0f, transform.position.z), new Vector3(transform.position.x, transform.position.y, transform.position.z), completed / maxCompletion);  
            return false;
        }
	}

	public void startBuilding()
	{
		building.SetActive(true);
	}

	public bool built()
	{
		return building.activeSelf;
	}
	
	public GameObject getBuildArea() {
		return building;
	}

	void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Builder" && completed != maxCompletion)
		{
			other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
		}
	}

	void OnTriggerStay(Collider other)
	{
        if (other.tag == "Builder" && completed != maxCompletion)
		{
			other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
		}
	}
}
