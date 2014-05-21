using UnityEngine;
using System.Collections;

public class Pad : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            other.GetComponent<PerceptionInterface>().refillWater(transform.position);
        }
        else if (other.tag == "Builder")
        {
            other.GetComponent<Builder>().refillBuildingMaterials(transform.position);
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            other.GetComponent<PerceptionInterface>().refillWater(transform.position);
        }
        else if (other.tag == "Builder")
        {
            other.GetComponent<Builder>().refillBuildingMaterials(transform.position);
        }
    }

	// Update is called once per frame
	void Update () {
        //hack to force ontriggerstay on each frame //testing purposes only
        transform.position = transform.position;
	}
}
