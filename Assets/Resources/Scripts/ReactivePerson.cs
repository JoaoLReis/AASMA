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
    public bool readyToMove = false;
    /********************************/
    
    public bool detectedFire = false;

    private ReactivePersonMove move;

    public bool fireSensor(GameObject bOnFire)
    {
        reactToFire(bOnFire);
        return true;
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

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Agent") || hit.gameObject.transform.tag == "Obstacle")
        {
            if (!collided && readyToMove)
            {
                collided = true;
                readyToMove = false;
                Invoke("recalculate", 0.5f / gameSpeed);
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

            Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 1.5f * Time.fixedDeltaTime * gameSpeed, 360);
            transform.rotation = Quaternion.LookRotation(newdir);

            if (transform.rotation == rot)
            {
                detectedFire = false;
                move.recalculate();
            }
        }
    }

    
}