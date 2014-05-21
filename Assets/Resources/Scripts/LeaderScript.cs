using UnityEngine;
using System.Collections;

public class LeaderScript : MonoBehaviour {

    PerceptionInterface scrpt;

	// Use this for initialization
	void Start () {
        scrpt = transform.parent.GetComponent<PerceptionInterface>();
	}

    void OnTriggerStay(Collider other)
    {
        scrpt.OnTriggerStay(other);
    }

    void OnTriggerEnter(Collider other)
    {
        if (scrpt == null)
            scrpt = transform.parent.GetComponent<PerceptionInterface>();
        scrpt.OnTriggerEnter(other);
    }
}
