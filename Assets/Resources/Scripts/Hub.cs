using UnityEngine;
using System.Collections;

public class Hub : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Agent")
            other.GetComponent<ReactiveAgent>().refillWater();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
