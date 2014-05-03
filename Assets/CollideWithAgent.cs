using UnityEngine;
using System.Collections;

public class CollideWithAgent : MonoBehaviour {


    private ReactiveFireFighter fireFighter;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

    private int numCollisions = 0;

	// Use this for initialization
	void Start () {

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        fireFighter = transform.parent.GetComponent<ReactiveFireFighter>();
	}

    private void recalculate()
    {
        fireFighter.recalculate();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.isTrigger)
            return;
        //Debug.LogWarning("this: " + transform.position.z + "Collided: " + hit.name);
        if (hit.gameObject.layer == LayerMask.NameToLayer("Agent") || hit.transform.tag == "Obstacle")
        {
            fireFighter.collided = true;
            fireFighter.readyToMove = false;
            numCollisions++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;
        numCollisions--;
        if(numCollisions <= 0)
            recalculate();
    }

	// Update is called once per frame
    void Update()
    {
        gameSpeed = hub.gameSpeed;	
	}
}
