﻿using UnityEngine;
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
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.position;  
	}

	public bool buildArea(int value)
	{
        if(!repairing)
        {
            if (completed == maxCompletion)
            {
                Destroy(building);
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
            BuildingScript scrpt = building.GetComponent<BuildingScript>();
            if (scrpt != null)
            {
                if (!scrpt.inNeedOfRepair())
                {
                    return true;
                }
                else
                {
                    scrpt.repair(1);
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
        Destroy(building);
        completed = 0;
        building = Instantiate(Resources.Load("Prefab/building_placeholder")) as GameObject;
        building.transform.parent = transform;
        building.transform.position = new Vector3(transform.position.x, transform.position.y - 32.0f, transform.position.z);
    }

	void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Builder")
		{
            
            if (completed != maxCompletion)
            {
                other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
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

	void OnTriggerStay(Collider other)
	{
        if (other.tag == "Builder")
        {
            if (completed != maxCompletion)
            {
                other.GetComponent<ReactiveBuilder>().buildSensor(transform.gameObject);
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
