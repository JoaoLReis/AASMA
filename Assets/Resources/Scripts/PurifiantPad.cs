using UnityEngine;
using System.Collections;

public class PurifiantPad : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            other.GetComponent<PerceptionInterface>().refillPurificant();
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            other.GetComponent<PerceptionInterface>().refillPurificant();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //hack to force ontriggerstay on each frame //testing purposes only
        transform.position = transform.position;
    }
}
