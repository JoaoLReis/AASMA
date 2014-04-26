using UnityEngine;
using System.Collections;

public class FirePerception : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
        {
			other.GetComponent<ReactiveFireFighter>().fireSensor(transform.parent.gameObject);
		}
        if (other.tag == "Person")
        {
            other.GetComponent<ReactivePerson>().fireSensor(transform.parent.gameObject);
        }
	}

	/*void OnTriggerStay(Collider other) {
		if(other.tag == "Agent") {
			other.GetComponent<ReactiveAgent>().attendFire(transform.parent.gameObject);
		}
	}*/
}
