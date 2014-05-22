using UnityEngine;
using System.Collections;

public class BuildAreaScript : MonoBehaviour {

	public GameObject building;

    public bool repairing = false;
	public float maxCompletion = 10;
    public float completed;
    public bool isBuilt = false;

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
        if(!isBuilt && completed == maxCompletion)
        {
            Destroy(building);
            transform.gameObject.GetComponent<Renderer>().enabled = false;
            building = Instantiate(Resources.Load("Prefab/building"), new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation) as GameObject;
            building.transform.parent = transform;
            building.transform.Rotate(building.transform.right, -90, Space.World);
            isBuilt = true;
        }
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
                isBuilt = true;
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
                    repairing = false;
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
        repairing = false;
        transform.gameObject.GetComponent<Renderer>().enabled = true;
        Destroy(building);
        isBuilt = false;
        GameObject.FindWithTag("Hub").GetComponent<Hub>().buildingDestroyed();
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
            Builder builder = other.GetComponent<Builder>();
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
                            other.GetComponent<Builder>().buildSensor(transform.gameObject);
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
            Builder builder = other.GetComponent<Builder>();
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
                            other.GetComponent<Builder>().buildSensor(transform.gameObject);
                        }
                    }
                }
            }
        }
	}
}
