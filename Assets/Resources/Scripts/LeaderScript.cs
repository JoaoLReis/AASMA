using UnityEngine;
using System.Collections;

public class LeaderScript : MonoBehaviour {

    ReactiveFireFighter scrpt;

	// Use this for initialization
	void Start () {
        scrpt = transform.parent.GetComponent<ReactiveFireFighter>();
	}

    void OnTriggerStay(Collider other)
    {
        scrpt.OnTriggerStay(other);
    }

    void OnTriggerEnter(Collider other)
    {
        scrpt.OnTriggerEnter(other);
    }
}
