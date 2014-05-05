using UnityEngine;
using System.Collections;

public class FirePerception : MonoBehaviour {

    public bool beingTakenCareOf = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter" && !beingTakenCareOf)
        {
            if (other.GetComponent<ReactiveFireFighter>().fireSensor(transform.parent.gameObject))
                beingTakenCareOf = true;
		}
        if (other.tag == "Person" && !beingTakenCareOf)
        {
            if (other.GetComponent<ReactivePerson>().fireSensor(transform.parent.gameObject))
                beingTakenCareOf = true;
        }
	}
}
