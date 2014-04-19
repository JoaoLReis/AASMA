using UnityEngine;
using System.Collections;

public class FirePerception : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        if (other.tag == "Agent")
        {
            Debug.Log("TriggerEnterAgent");
			other.GetComponent<ReactiveAgent>().fireSensor(transform.parent.gameObject);
		}
	}

	/*void OnTriggerStay(Collider other) {
		if(other.tag == "Agent") {
			other.GetComponent<ReactiveAgent>().attendFire(transform.parent.gameObject);
		}
	}*/
}
