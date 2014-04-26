using UnityEngine;
using System.Collections;

using Pathfinding;

public class ReactivePerson : MonoBehaviour
{

    /****GENERAL VARIABLES****/

    private GameObject fire;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

    /********** FOR Colliding *******/
    public bool collided = false;
    /********************************/
    
    public bool detectedFire = false;

    private ReactivePersonMove move;

    public void fireSensor(GameObject bOnFire)
    {
        reactToFire(bOnFire);
    }

    private void reactToFire(GameObject bOnFire)
    {
        detectedFire = true;
        fire = bOnFire.GetComponent<BuildingScript>().getFire();
    }

    void Start()
    {
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        move = GetComponent<ReactivePersonMove>();
    }

    private void recalculate()
    {
        collided = false;
        move.recalculate();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Agent") || hit.transform.tag == "Obstacle")
        {
            if (!collided)
            {
                collided = true;
                Invoke("recalculate", 1 / gameSpeed);
            }
            return;
        }
    }

    void Update()
    {
        if (collided)
        {
            transform.Rotate(transform.up, 100 * Time.fixedDeltaTime * gameSpeed);
        }
        if (detectedFire && fire != null)
        {
            Vector3 dir = (transform.position - fire.transform.position).normalized;
            dir.y = 0f;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 100 * Time.deltaTime * gameSpeed);
            if (transform.rotation == rot)
            {
                detectedFire = false;
                move.recalculate();
            }
        }
    }

    
}