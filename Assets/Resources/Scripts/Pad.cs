using UnityEngine;
using System.Collections;

public class Pad : MonoBehaviour {

    Hub hub;

	// Use this for initialization
	void Start () 
    {
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            if(hub.purifiant > 0)
            {
                if(other.GetComponent<PerceptionInterface>().refillWater(transform.position))
                    hub.purifiant--;
            }          
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
            if (hub.purifiant > 0)
            {
                if (other.GetComponent<PerceptionInterface>().refillWater(transform.position))
                    hub.purifiant--;
            }     
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
