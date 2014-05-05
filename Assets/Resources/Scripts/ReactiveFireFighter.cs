using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactiveFireFighter : MonoBehaviour, ReactiveInterface {

    /****GENERAL VARIABLES****/
    public int MaxWater = 30;
    private int currentWater = 15;

    /******* FOR PUTTING OUT FIRES ******/
    private Transform barrelEnd;
    private GameObject waterJetprefab;
    private GameObject waterJet;
    private float waterJetLifeTime = 1;

    /****** FOR DEALING WITH FIRES ******/
    public bool puttingOutFire = false;
    private GameObject fire;
    public bool preparingToPutOutFire = false;
    public bool movingTowardsFire = false;
    public bool rotatingToFire = false;
    /************************************/

    private ReactiveFireFighterMove move;

    /********** FOR Colliding *******/
    public bool collided = false;
    public bool readyToMove = false;
    /********************************/

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

    /*************** REFILL ******************/
    public bool preparingToRefill = false;
    public bool moveToRefill = false;
    public bool reffiling = false;
    public Vector3 refillPosition;
    /*****************************************/

    /*************** FIRE LEADER *************/
    public bool leader = false;
    public bool participant = false;
    public List<ReactiveFireFighter> fireParticipants;

    public void Start()
    {
        //Initialize some objects
        barrelEnd = FindChild("BarrelEnd");
        waterJetprefab = (GameObject)Resources.Load("Prefab/Water Jet");

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        waterJetLifeTime = waterJetprefab.particleSystem.startLifetime;
        move = GetComponent<ReactiveFireFighterMove>();
        fireParticipants = new List<ReactiveFireFighter>();
    }

    /******GUI FUNCTIONS*****/

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), currentWater + "/" + MaxWater);
    }

    public bool AddjustCurrentWater(int adj)
    {
        currentWater += adj;
        if (currentWater < 0)
        {
            currentWater = 0;
            return true;
        }

        if (currentWater > MaxWater)
        {
            currentWater = MaxWater;
            return true;
        }

        if (MaxWater < 1)
            MaxWater = 1;

        return false;
        //waterBarLength = (Screen.width / 6) * (currentWater /(float)MaxWater);
    }

    private void decreaseWater(int amount)
    {
        if (currentWater - amount < 0)
            currentWater = 0;
        else
            currentWater -= amount;
    }

    public bool refillWater(Vector3 position)
    {
        if (currentWater < (0.10f * (float)MaxWater) && !puttingOutFire && !preparingToPutOutFire && !detectIfRefill())
        {
            preparingToRefill = true;
            refillPosition = position;
            return true;
        }
        else return false;
    }

    private IEnumerator decreaseFireHealth(int amount)
    {
        while (fire != null && currentWater > 0)
        {
            fire.GetComponent<FireStats>().decreaseHealth(1);
            decreaseWater(1);
            if(fire != null)
                yield return new WaitForSeconds(1.0f / gameSpeed);
        }
        Destroy(waterJet);
        if (leader)
        {
            foreach (ReactiveFireFighter ff in fireParticipants)
            {
                ff.notify();
            }
        }
        puttingOutFire = false;
    }

    public void notify()
    {
        //TODO
    }

    public bool fireSensor(GameObject bOnFire)
    {
        if (waterJet == null && currentWater > 0 && puttingOutFire == false)
        {
            attendFire(bOnFire);
            return true;
        }
        else return false;
    }

    private void attendFire(GameObject bOnFire)
    {
        preparingToPutOutFire = true;
        fire = bOnFire.GetComponent<BuildingScript>().getFire();
    }

    public void recalculateRight()
    {
        collided = false;
        move.recalculateRight();
    }
    
    public void recalculate()
    {
        collided = false;
        move.recalculate();
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Agent") || hit.transform.tag == "Obstacle")
        {
            if (!collided && readyToMove)
            {
                
                foreach (ContactPoint var in hit.contacts)
                {
                    Vector3 relativePosition = transform.InverseTransformPoint(var.point);

                    if (relativePosition.z < -0.3f)
                        return;
                }
                
                collided = true;
                readyToMove = false;
                Invoke("recalculate", 1.2f / gameSpeed);
            }
            return;
        }
    }

    public bool detectIfRefill()
    {
        return (preparingToRefill || moveToRefill || reffiling);
    }

    public void Update()
    {
        gameSpeed = hub.gameSpeed;
        if (!detectIfRefill())
        {
            if (collided)
            {
                transform.Rotate(transform.up, 100 * Time.deltaTime * gameSpeed);
            }
            if (fire != null)
            {
                if (preparingToPutOutFire)
                {
                    Vector3 dir = ((fire.transform.position + fire.transform.forward * 5.5f) - transform.position).normalized;
                    dir.y = 0f;

                    Quaternion rot = transform.rotation;
                    rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

                    Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 2.5f * Time.deltaTime * gameSpeed, 360);
                    transform.rotation = Quaternion.LookRotation(newdir);
                    if (transform.rotation == rot)
                    {
                        preparingToPutOutFire = false;
                        movingTowardsFire = true;
                    }
                    /*move.restartPathToPosition(fire.transform.position + fire.transform.forward * 5.5f);
                    preparingToPutOutFire = false;
                    movingTowardsFire = true;*/
                }
                else if(movingTowardsFire)
                {
                    Vector3 tmp = fire.transform.position;
                    tmp.y = transform.position.y;
                    tmp = tmp + (fire.transform.forward * 5.5f);
                    transform.position = Vector3.MoveTowards(transform.position, tmp, 3.5f * Time.fixedDeltaTime * gameSpeed);
                    if ((tmp - transform.position).magnitude < 1f)
                    {
                        movingTowardsFire = false;
                        rotatingToFire = true;
                    }
                }
                else if (rotatingToFire)
                {
                    Vector3 dir = (fire.transform.position - transform.position).normalized;
                    dir.y = 0f;

                    Quaternion rot = transform.rotation;
                    rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

                    Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 2.5f * Time.deltaTime * gameSpeed, 360);

                    transform.rotation = Quaternion.LookRotation(newdir);

                    if (transform.rotation == rot)
                    {
                        barrelEnd.LookAt(fire.transform);
                        waterJet = (GameObject)Instantiate(waterJetprefab, barrelEnd.position, barrelEnd.rotation);
                        waterJet.particleSystem.startSpeed = (fire.transform.position - barrelEnd.transform.position).magnitude * gameSpeed;
                        waterJet.particleSystem.startLifetime = waterJetLifeTime / gameSpeed;
                        StartCoroutine("decreaseFireHealth", 1);
                        puttingOutFire = true;
                        rotatingToFire = false;
                    }
                }             
            }
        }
    }

    private Transform FindChild(string name)
    {
        Transform[] trans = GetComponentsInChildren<Transform>();

        foreach (Transform t in trans)
        {
            if (t.gameObject.name.Equals(name))
                return t;
        }
        return null;
    }

    /*----------Reactive Interface functions-----------*/

    public void setCollided(bool v)
    {
        collided = v;
    }

    public bool getCollided()
    {
        return collided;
    }

    public void setReadyToMove(bool v)
    {
        readyToMove = v;
    }

    public bool getReadyToMove()
    {
        return readyToMove;
    }
}
