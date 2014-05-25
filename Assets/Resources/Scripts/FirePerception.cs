using UnityEngine;
using System.Collections;

public class FirePerception : MonoBehaviour
{

    public bool beingTakenCareOf = false;
    private PerceptionInterface assignedAgent;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter" && !beingTakenCareOf)
        {
            if (other.GetComponent<PerceptionInterface>().fireSensor(transform.parent.gameObject))
            {
                assignedAgent = other.GetComponent<PerceptionInterface>();
                beingTakenCareOf = true;
            }

        }
        else if (other.tag == "Builder" && !beingTakenCareOf)
        {
            other.GetComponent<Builder>().fireSensor(transform.parent.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "FireFighter" && beingTakenCareOf)
        {
            if (other.transform.position == assignedAgent.transform.position)
            {
                Debug.Log("hahahah");
                beingTakenCareOf = false;
            }
        }
    }
}