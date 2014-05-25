using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReactiveFireFighter : PerceptionInterface {

    /****GENERAL VARIABLES****/
    public int MaxWater = 30;
    private int currentWater = 30;

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
    //public bool gettingWater = false;
    public bool participant = false;
    public List<ReactiveFireFighter> fireParticipants;
    public bool helping = false;
    public bool purificant = false;
    public Texture fireTex, leaderTex, waterTex, sleepTex;

    /************ FOR NIGHTTIME *************/
    private int _numFiresPutOut = 0;
    public bool _resting = false;
    /****************************************/

    void Start()
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
        Rect rt = new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20);
        GUI.Box(rt, "   " + currentWater + "/" + MaxWater);
        if (hub.getNightTime())
            GUI.DrawTexture(rt, sleepTex);
        else
        {
            GUI.DrawTexture(rt, fireTex);
        }
    }

    public override void updateNodes(Vector3 pos)
    {

    }

    public override void refillPurificant()
    {
        purificant = true;
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

    void decreaseWater(int amount)
    {
        if (currentWater - amount < 0)
            currentWater = 0;
        else
            currentWater -= amount;
    }

    public override bool refillWater(Vector3 position)
    {
        if (!puttingOutFire && !preparingToPutOutFire && !detectIfRefill() && ((currentWater < (0.10f * (float)MaxWater)))) //|| gettingWater))
        {
            preparingToRefill = true;
            refillPosition = position;
            //gettingWater = false;
            return true;
        }
        else return false;
    }

    IEnumerator decreaseFireHealth(int amount)
    {
        if (leader)
        {
            while (fire != null)
            {
                if (currentWater > 0)
                {
                    fire.GetComponent<FireStats>().decreaseHealth(1);
                    decreaseWater(1);
                }
                else
                {
                    if (fireParticipants.Count > 0)
                    {
                        Destroy(waterJet);
                    }
                    else
                    {
                        Destroy(waterJet);
                        goGetWater();
                        break;
                    }
                    if (move.nightTime)
                    {
                        break;
                    }
                }
                if (fire != null)
                    yield return new WaitForSeconds(1.0f / gameSpeed);

            }
        }
        else
        {
            while (fire != null && currentWater > 0 )
            {
                fire.GetComponent<FireStats>().decreaseHealth(1);
                decreaseWater(1);
                if (fire != null)
                    yield return new WaitForSeconds(1.0f / gameSpeed);
            }
        }

        Destroy(waterJet);
        if (leader)
        {
            leader = false;
            //_numFiresPutOut += 1;
            Transform hat = transform.FindChild("Hat") as Transform;
            hat.gameObject.SetActive(false);
            Debug.Log("setting hat off");
            fireParticipants.Clear();
        }
        else
        {
            helping = false;
        }
        //Hack to fix agents blocking.
        preparingToPutOutFire = false;
        puttingOutFire = false;
        if (currentWater == 0)
        {
            goGetWater();
        }
    }

    public void notify()
    {
        //TODO
    }

    public override bool fireSensor(GameObject bOnFire)
    {
        Debug.Log("recebi fire sensor");
        if (waterJet == null && puttingOutFire == false && !move.nightTime)
        {
            Debug.Log("atender");
            return attendFire(bOnFire);
        }
        else return false;
    }

    //Only receives attendFire if the fire isnt already assigned to a leader
    bool attendFire(GameObject bOnFire)
    {
        Debug.Log("within attend fire");
        if (currentWater >= 0.1 * MaxWater)
        {
            Debug.Log("water");
            Debug.Log(currentWater);

                preparingToPutOutFire = true;
                leader = true;
                Debug.Log(leader);
                Transform hat = transform.FindChild("Hat") as Transform;
                hat.gameObject.SetActive(true);
                fire = bOnFire.GetComponent<BuildingScript>().getFire();
                return true;
        }
        return false;
    }

    public override void electLeader()
    {
        Transform hat = transform.FindChild("Hat") as Transform;
        hat.gameObject.SetActive(true);
    }

    void helpWithFire(GameObject fireToAttend)
    {
        preparingToPutOutFire = true;
        helping = true;
        fire = fireToAttend;
    }

    void goGetWater()
    {
        //gettingWater = true;
        helping = false;
        //Hack to fix agents blocking.
        preparingToPutOutFire = false;
        puttingOutFire = false;
    }

    public override void recalculateRight()
    {
        collided = false;
        move.recalculateRight();
    }

    public override void recalculate()
    {
        collided = false;
        move.recalculate();
    }

    public bool detectIfRefill()
    {
        return (preparingToRefill || moveToRefill || reffiling);
    }

    public void attendMovementTowardsHelpingFire()
    {
        if (preparingToPutOutFire)
        {
            Vector3 dir = (fire.transform.position - transform.position).normalized;
            dir.y = 0f;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

            Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 2.5f * Time.deltaTime * gameSpeed, 360);
            transform.rotation = Quaternion.LookRotation(newdir);
            if (transform.rotation == rot && !puttingOutFire)
            {
                barrelEnd.LookAt(fire.transform);
                waterJet = (GameObject)Instantiate(waterJetprefab, barrelEnd.position, barrelEnd.rotation);
                waterJet.particleSystem.startSpeed = (fire.transform.position - barrelEnd.transform.position).magnitude * gameSpeed;
                waterJet.particleSystem.startLifetime = waterJetLifeTime / gameSpeed;
                StartCoroutine("decreaseFireHealth", 1);
                preparingToPutOutFire = false;
                puttingOutFire = true;
                rotatingToFire = false;
            }
        }
    }

    private void attendMovementTowardsFire()
    {
        if(fire == null)
        {
            preparingToPutOutFire = false;
            movingTowardsFire = false;
            rotatingToFire = false;
            leader = false;
        }
        else if (preparingToPutOutFire)
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
            if (fire == null)
            {
                preparingToPutOutFire = false;
                movingTowardsFire = false;
                rotatingToFire = false;
                puttingOutFire = false;
                leader = false;
            }
            /*move.restartPathToPosition(fire.transform.position + fire.transform.forward * 5.5f);
            preparingToPutOutFire = false;
            movingTowardsFire = true;*/
        }
        else if (movingTowardsFire)
        {
            Vector3 tmp = fire.transform.position;
            tmp.y = transform.position.y;
            tmp = tmp + (fire.transform.forward * 5.5f);
            transform.position = Vector3.MoveTowards(transform.position, tmp, 3.5f * Time.fixedDeltaTime * gameSpeed);
            if ((tmp - transform.position).magnitude < 1f )
            {
                movingTowardsFire = false;
                rotatingToFire = true;
            }
            if (fire == null)
            {
                preparingToPutOutFire = false;
                movingTowardsFire = false;
                rotatingToFire = false;
                puttingOutFire = false;
                leader = false;
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
            if (fire == null)
            {
                preparingToPutOutFire = false;
                movingTowardsFire = false;
                rotatingToFire = false;
                puttingOutFire = false;
                leader = false;
            }
        }             
    }

    public void Update()
    {
        gameSpeed = hub.gameSpeed;
        if (!_resting)
        {
            if (!hub.getNightTime())
            {
                if (!detectIfRefill())
                {
                    if (collided)
                    {
                        transform.Rotate(transform.up, 100 * Time.deltaTime * gameSpeed);
                    }
                    if (fire != null)
                    {
                        if (!helping)
                            attendMovementTowardsFire();
                        else attendMovementTowardsHelpingFire();
                    }
                    else
                    {
                        preparingToPutOutFire = false;
                        movingTowardsFire = false;
                        rotatingToFire = false;
                        leader = false;
                        Transform hat = transform.FindChild("Hat") as Transform;
                        hat.gameObject.SetActive(false);
                    }
                }
                else
                {
                    preparingToPutOutFire = false;
                    movingTowardsFire = false;
                    rotatingToFire = false;
                    puttingOutFire = false;
                    if (waterJet != null)
                        Destroy(waterJet);
                }
            }
            else
            {

            }

        }
    }

    Transform FindChild(string name)
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

    public override void setCollided(bool v)
    {
        collided = v;
    }

    public override bool getCollided()
    {
        return collided;
    }

    public override void setReadyToMove(bool v)
    {
        readyToMove = v;
    }

    public override bool getReadyToMove()
    {
        return readyToMove;
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

    //Removes the figther with less water and orders him to get water.
    ReactiveFireFighter sendFigtherWithLessWaterToGetWater()
    {
        ReactiveFireFighter figtherWithLessWater = fireParticipants[0];
        foreach (ReactiveFireFighter scrpt in fireParticipants)
        {
            if (scrpt.currentWater < figtherWithLessWater.currentWater)
            {
                figtherWithLessWater = scrpt;
            }
        }
        if (figtherWithLessWater.currentWater >= ((float)figtherWithLessWater.MaxWater / 1.2f))
            return null;
        figtherWithLessWater.goGetWater();
        fireParticipants.Remove(figtherWithLessWater);
        return figtherWithLessWater;
    }

    bool needtoGetWater(int extraWater)
    {
        int totalWater = currentWater + extraWater;
        foreach (ReactiveFireFighter scrpt in fireParticipants)
        {
            totalWater = totalWater + scrpt.currentWater;
        }
        if (fire != null && (totalWater - fire.GetComponent<FireStats>().getHealth() < 0))
        {
            return true;
        }
        return false;
    }

    bool needHelp()
    {
        float numberOfAgents = fireParticipants.Count+1;
        if (fire == null)
            return false;
        float fireStrength = fire.GetComponent<FireStats>().getHealth();
        float fireToBePutByAgents = fireStrength / numberOfAgents;
       // Debug.Log("NEED HELP?????");
       // Debug.Log(fireToBePutByAgents);
        if (currentWater < fireToBePutByAgents)
            return true;
        foreach (ReactiveFireFighter scrpt in fireParticipants)
        {
            //Debug.Log("Listing:");
            //Debug.Log(scrpt.currentWater);
            if(scrpt.currentWater < fireToBePutByAgents)
            {
                Debug.Log(fireToBePutByAgents);
                return true;
            }    
        }
        return false;
    }
    //fireParticipants.Add(scrpt);
    void checkFireRefill(ReactiveFireFighter newParticipant)
    {
        //DO WE NEED WATER?
        if (needtoGetWater(newParticipant.currentWater))
        {
            fireParticipants.Add(newParticipant);
            //Debug.Log("Need to help!");
            //GRAB ONE AND ORDER HIM TO GET WATER
            ReactiveFireFighter scrpt = sendFigtherWithLessWaterToGetWater();
            if (scrpt == null)
            {
                newParticipant.helpWithFire(fire);
                return;
            }
            else if (scrpt.helping != newParticipant.helping)
            {
                newParticipant.helpWithFire(fire);
                return;
            }
            fireParticipants.Remove(scrpt);
            scrpt.goGetWater();
        }
        else
        {
            if (needHelp())
            {
                //Debug.Log("Theres no need to get water");
                fireParticipants.Add(newParticipant);
                newParticipant.helpWithFire(fire);
            }
        }
    }

    //Function invoked by leaderscript.
    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter" && leader)
        {
            ReactiveFireFighter scrpt = other.GetComponent<ReactiveFireFighter>();
            if (!fireParticipants.Contains(scrpt) && !move.nightTime)
            {               
                checkFireRefill(scrpt);
            }
        }
        else if (other.tag == "Hub")
        {
            if (purificant)
            {
                hub.purifiant++;
                purificant = false;
            }       
        }
    }

    //Function invoked by leaderscript.
    public override void OnTriggerStay(Collider other)
    {
        if (other.tag == "FireFighter" && leader)
        {
            ReactiveFireFighter scrpt = other.GetComponent<ReactiveFireFighter>();
            if (!fireParticipants.Contains(scrpt) && !move.nightTime)
            {
                checkFireRefill(scrpt);
            }
        }
        else if (other.tag == "Hub")
        {
            if(purificant)
            {
                hub.purifiant++;
                purificant = false;
            }
        }
    }

    //Function invoked by leaderscript.
    public override void OnTriggerExit(Collider other)
    {
        if (other.tag == "FireFighter" && leader)
        {
            ReactiveFireFighter scrpt = other.GetComponent<ReactiveFireFighter>();
            if (fireParticipants.Contains(scrpt) && !scrpt.helping)
            {
                fireParticipants.Remove(scrpt);
            }
        }
    }

    public override void isNightTime(bool night)
    {
        if (move == null)
            move = GetComponent<ReactiveFireFighterMove>();
        move.nightTime = night;
    }

    public override void setResting(bool resting)
    {
        _resting = resting;
    }

    public override int numFiresPutOut()
    {
        return 0;
    }

    public override void reset()
    {
        puttingOutFire = false;
        preparingToPutOutFire = false;
        movingTowardsFire = false;
        rotatingToFire = false;
        collided = false;
        readyToMove = false;
        preparingToRefill = false;
        moveToRefill = false;
        reffiling = false;
        helping = false;
        leader = false;
        participant = false;
        _resting = false;
        _numFiresPutOut = 0;
        if (waterJet != null)
            Destroy(waterJet);
        Transform hat = transform.FindChild("Hat") as Transform;
        hat.gameObject.SetActive(false);
    }
}
