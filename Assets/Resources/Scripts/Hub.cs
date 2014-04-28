using UnityEngine;
using System.Collections;

public class Hub : MonoBehaviour {

    public int gameSpeed = 1;
	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
            other.GetComponent<ReactiveFireFighter>().refillWater();
        else if(other.tag == "Builder")
            other.GetComponent<ReactiveBuilder>().refillBuildingMaterials();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
