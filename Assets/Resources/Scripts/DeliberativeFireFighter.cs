using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public partial class DeliberativeFireFighter : PerceptionInterface
{

    /****GENERAL VARIABLES****/
    public int MaxWater = 30;
    public int currentWater = 30;

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

    private DeliberativeFireFighterMove move;

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
    public bool gettingWater = false;
    public bool participant = false;
    public List<DeliberativeFireFighter> fireParticipants;
    public bool helping = false;
    public Texture fireTex, leaderTex, waterTex, helpingTex, needHelpTex, sleepTex;
    /****************************************/
    public enum STATE {DEFAULT, GET_WATER, GET_WATER_AND_RETURN, GO_HELP_A_FIRE, RECRUIT_A_HELPER, SLEEP}
    public Vector3 placeTogo;
    public STATE objective;
    public int sentHelp = 0;
    /****************************************/

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
        move = GetComponent<DeliberativeFireFighterMove>();
        fireParticipants = new List<DeliberativeFireFighter>();
    }

    /******GUI FUNCTIONS*****/

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rt = new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20);
        GUI.Box(rt, "   " + currentWater + "/" + MaxWater);
        if (leader)
            GUI.DrawTexture(rt, leaderTex);
        else if (objective == STATE.GET_WATER)//objective == STATE.GET_WATER)
            GUI.DrawTexture(rt, waterTex);
        else if (objective == STATE.RECRUIT_A_HELPER)
            GUI.DrawTexture(rt, needHelpTex);
        else if (objective == STATE.DEFAULT)
            GUI.DrawTexture(rt, fireTex);
        else if (objective == STATE.GO_HELP_A_FIRE)
            GUI.DrawTexture(rt, helpingTex);
        else if (objective == STATE.SLEEP)
            GUI.DrawTexture(rt, sleepTex);
    }

    public bool AddjustCurrentWater(int adj)
    {
        currentWater += adj;
        if (currentWater < 0)
        {
            currentWater = 0;
            goGetWater();
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
        {
            currentWater = 0;
            goGetWater();
        }    
        else
            currentWater -= amount;
    }

    public override bool refillWater(Vector3 position)
    {
        if (!puttingOutFire && !preparingToPutOutFire && !detectIfRefill() && ((currentWater < (0.10f * (float)MaxWater)) || gettingWater))
        {
            preparingToRefill = true;
            refillPosition = position;
            gettingWater = false;
            if(objective == STATE.GET_WATER)
                objective = STATE.DEFAULT;
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
                }
                if (fire != null)
                    yield return new WaitForSeconds(1.0f / gameSpeed);

            }
        }
        else
        {
            while (fire != null && currentWater > 0 && !gettingWater)
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
            _numFiresPutOut += 1;
            //Transform hat = transform.FindChild("Hat") as Transform;
            //hat.gameObject.SetActive(false);
            fireParticipants.Clear();
            sentHelp = 0;
        }
        else
        {
            helping = false;
            setState(STATE.DEFAULT, Vector3.zero);
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

    void helpWithFire(GameObject fireToAttend)
    {
        preparingToPutOutFire = true;
        helping = true;
        setState(STATE.GO_HELP_A_FIRE, Vector3.zero);
        fire = fireToAttend;
    }

    void goGetWater()
    {
        gettingWater = true;
        helping = false;
        //Hack to fix agents blocking.
        preparingToPutOutFire = false;
        puttingOutFire = false;
        setState(STATE.GET_WATER, Vector3.zero);
    }

    public void setState(STATE st, Vector3 fire)
    {
        if (st == STATE.GET_WATER)
        {
            objective = STATE.GET_WATER;
            placeTogo = new Vector3(-19.57f, 0.0899f, 23.337f);
        }
        else if (st == STATE.GO_HELP_A_FIRE)
        {
            objective = STATE.GO_HELP_A_FIRE;
        }
        else if (st == STATE.RECRUIT_A_HELPER)
        {
            objective = STATE.RECRUIT_A_HELPER;
            placeTogo = fire;
        }
        else if (st == STATE.SLEEP)
        {
            objective = STATE.SLEEP;
            placeTogo = new Vector3(-19.57f, 0.0899f, 23.337f);
        }
        else objective = STATE.DEFAULT;
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
        if (fire == null)
        {
            preparingToPutOutFire = false;
            movingTowardsFire = false;
            rotatingToFire = false;
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

    //Removes the figther with less water and orders him to get water.
    DeliberativeFireFighter sendFigtherWithLessWaterToGetWater()
    {
        DeliberativeFireFighter figtherWithLessWater = fireParticipants[0];
        foreach (DeliberativeFireFighter scrpt in fireParticipants)
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
        foreach (DeliberativeFireFighter scrpt in fireParticipants)
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
        float numberOfAgents = fireParticipants.Count + 1;
        if (fire == null)
            return false;
        float fireStrength = fire.GetComponent<FireStats>().getHealth();
        float fireToBePutByAgents = fireStrength / numberOfAgents;
        if (currentWater < fireToBePutByAgents)
            return true;
        foreach (DeliberativeFireFighter scrpt in fireParticipants)
        {
            if (scrpt.currentWater < fireToBePutByAgents)
            {
                return true;
            }
        }
        return false;
    }
    //fireParticipants.Add(scrpt);
    public void Update()
    {
        gameSpeed = hub.gameSpeed;
        if (!_resting)
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
                    {
                        attendMovementTowardsFire();
                    }
                    else attendMovementTowardsHelpingFire();
                }
            }
        }
    }

    /********************ATENDING A FIRE************************/
    //Only receives attendFire if the fire isnt already assigned to a leader
    void attendFire(GameObject bOnFire)
    {
        fire = bOnFire.GetComponent<BuildingScript>().getFire();
        if(currentWater >= 0.1 * MaxWater)
        {
            preparingToPutOutFire = true;
            leader = true;
            //Transform hat = transform.FindChild("Hat") as Transform;
            //hat.gameObject.SetActive(true);
        }
        else setState(STATE.RECRUIT_A_HELPER, fire.transform.position);
    }

    public override bool fireSensor(GameObject bOnFire)
    {
        if (waterJet == null && puttingOutFire == false && !move.nightTime)
        {
            attendFire(bOnFire);
            return true;
        }
        else return false;
    }
    /***************************************************************/

    void redistributeWater(DeliberativeFireFighter figther)
    {
        int valueToFill = (MaxWater - currentWater);
        if(valueToFill >= figther.currentWater)
        {
            currentWater += figther.currentWater;
            figther.currentWater -= valueToFill;
        }
        else if(valueToFill < figther.currentWater)
        {
            currentWater += valueToFill;
            figther.currentWater-= valueToFill;
        }
        foreach(DeliberativeFireFighter scrpt in fireParticipants)
        {
            valueToFill = scrpt.MaxWater - scrpt.currentWater;
            if(valueToFill >= figther.currentWater)
            {
                scrpt.currentWater += figther.currentWater;
                figther.currentWater -= valueToFill;
            }
            else if(valueToFill < figther.currentWater)
            {
                scrpt.currentWater += valueToFill;
                figther.currentWater-= valueToFill;
            }
        }
    }

    void checkFireRefill(DeliberativeFireFighter newParticipant)
    {
        //DO WE NEED WATER?
        if (needtoGetWater(newParticipant.currentWater))
        {
            fireParticipants.Add(newParticipant);
            //GRAB ONE AND ORDER HIM TO GET WATER
            DeliberativeFireFighter scrpt = sendFigtherWithLessWaterToGetWater();
            if (scrpt == null)
            {
                if (fire != null)
                {
                    if(needHelp() && sentHelp < 1)
                    {
                        redistributeWater(newParticipant);
                        if(newParticipant.currentWater < 0.1 * newParticipant.MaxWater)
                            newParticipant.goGetWater();
                        else
                        {
                            sentHelp++;
                            newParticipant.setState(STATE.RECRUIT_A_HELPER, fire.transform.position);
                        }
                    }
                    else
                    {
                        redistributeWater(newParticipant);
                        if (newParticipant.currentWater < 0.1 * newParticipant.MaxWater)
                            newParticipant.goGetWater();
                        else newParticipant.helpWithFire(fire);
                    }
                }
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
                fireParticipants.Add(newParticipant);
                newParticipant.helpWithFire(fire);
            }
        }
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

    //Function invoked by leaderscript.
    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FireFighter" && leader)
        {
            DeliberativeFireFighter scrpt = other.GetComponent<DeliberativeFireFighter>();
            if (!fireParticipants.Contains(scrpt) && !move.nightTime)
            {
                checkFireRefill(scrpt);
            }
        }
    }

    //Function invoked by leaderscript.
    public override void OnTriggerStay(Collider other)
    {
        if (other.tag == "FireFighter" && leader)
        {
            DeliberativeFireFighter scrpt = other.GetComponent<DeliberativeFireFighter>();
            if (!fireParticipants.Contains(scrpt) && !move.nightTime)
            {
                checkFireRefill(scrpt);
            }
        }
    }

    public override void isNightTime(bool night)
    {
        if(move == null)
            move = GetComponent<DeliberativeFireFighterMove>();
        move.nightTime = night;
        if (night)
        {
            setState(STATE.SLEEP, Vector3.zero);
        }
    }

    public override void setResting(bool resting)
    {
        _resting = resting;

    }

    public override int numFiresPutOut()
    {
        return _numFiresPutOut;
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
        placeTogo = Vector3.zero;
        objective = STATE.DEFAULT;
        if (waterJet != null)
            Destroy(waterJet);
    }
}
