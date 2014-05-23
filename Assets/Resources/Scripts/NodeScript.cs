using UnityEngine;
using System.Collections;

public class NodeScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter")
        {
            //Debug.Log("Node Covered");
            other.GetComponent<PerceptionInterface>().updateNodes(new Vector3(transform.position.x, 0.0899f, transform.position.z));
        }
        else if (other.tag == "Builder")
        {
            other.GetComponent<Builder>().updateNodes(new Vector3(transform.position.x, 0.0899f, transform.position.z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //hack to force ontriggerstay on each frame //testing purposes only
        transform.position = transform.position;
    }
}
