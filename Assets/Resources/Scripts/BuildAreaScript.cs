using UnityEngine;
using System.Collections;

public class BuildAreaScript : MonoBehaviour {

	public GameObject building;

    public bool repairing = false;
	public float maxCompletion = 10;
	private float completed;

	// Use this for initialization
	void Start () {
		completed = 0;
        building = Instantiate(Resources.Load("Prefab/building_placeholder"), new Vector3(transform.position.x, transform.position.y-32.0f, transform.position.z), transform.rotation) as GameObject; 
		building.transform.parent = transform;
        building.transform.Rotate(building.transform.right, -90, Space.World);
        building.GetComponentInChildren<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.position;  
	}

	public bool buildArea(int value)
	{
        if(!repairing)
        {
            building.GetComponentInChildren<Renderer>().enabled = true;
            if (completed == maxCompletion)
            {
                Destroy(building);
                transform.gameObject.GetComponent<Renderer>().enabled = false;
                building = Instantiate(Resources.Load("Prefab/building"), new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
                building.transform.parent = transform;
                building.transform.Rotate(building.transform.right, -90, Space.World);
                return true;
            }
            else
            {
                completed++;
                building.transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y - 32.0f, transform.position.z), new Vector3(transform.position.x, transform.position.y, transform.position.z), completed / maxCompletion);
                return false;
            }
        }
        else
        {
            building.GetComponentInChildren<Renderer>().enabled = true;
            BuildingScript scrpt = building.GetComponent<BuildingScript>();
            if (scrpt != null)
            {
                if (!scrpt.inNeedOfRepair())
                {
                    return true;
                }
                else
                {
                    scrpt.repair(10);
                    return false;
                }
            }
            else return true;
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
	
	public GameObject getBuildArea() 
    {
		return building;
	}

    public void RebuildableArea()
    {
        transform.gameObject.GetComponent<Renderer>().enabled = true;
        Destroy(building);
        completed = 0;
        building = Instantiate(Resources.Load("Prefab/building_placeholder")) as GameObject;
        building.transform.parent = transform;
        building.transform.position = new Vector3(transform.position.x, transform.position.y - 32.0f, transform.position.z);
        building.GetComponentInChildren<Renderer>().enabled = false;
    }

	void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Builder")
		{
            ReactiveBuilder builder = other.GetComponent<ReactiveBuilder>();
            if (!builder.preparingToBuild && !builder.preparingToRefill)
            {
                if (completed != maxCompletion)
                {
                    builder.buildSensor(transform.gameObject);
                }
                else
                {
                    BuildingScript scrpt = building.GetComponent<BuildingScript>();
                    if (scrpt != null)
                    {
                        if (scrpt.inNeedOfRepair())
                        {
                            repairing = true;
                            other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
                        }
                    }
                }
            }
		}
	}

	void OnTriggerStay(Collider other)
	{
        if (other.tag == "Builder")
        {
            ReactiveBuilder builder = other.GetComponent<ReactiveBuilder>();
            if (!builder.preparingToBuild && !builder.preparingToRefill)
            {
                if (completed != maxCompletion)
                {
                    builder.buildSensor(transform.gameObject);
                }
                else
                {
                    BuildingScript scrpt = building.GetComponent<BuildingScript>();
                    if (scrpt != null)
                    {
                        if (scrpt.inNeedOfRepair())
                        {
                            repairing = true;
                            other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
                        }
                    }
                }
            }
        }
	}
}
