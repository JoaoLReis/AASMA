using UnityEngine;
using System.Collections;

public class FirePerception : MonoBehaviour {

    public bool beingTakenCareOf = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter" && !beingTakenCareOf)
        {
            if (other.GetComponent<PerceptionInterface>().fireSensor(transform.parent.gameObject))
                beingTakenCareOf = true;
		}
        else if (other.tag == "Person" && !beingTakenCareOf)
        {
            other.GetComponent<ReactivePerson>().fireSensor(transform.parent.gameObject);
        }
        else if (other.tag == "Builder" && !beingTakenCareOf)
        {
            other.GetComponent<Builder>().fireSensor(transform.parent.gameObject);
        }
	}
}
