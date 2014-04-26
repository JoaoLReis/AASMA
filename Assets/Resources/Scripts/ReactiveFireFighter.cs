using UnityEngine;
using System.Collections;

public class ReactiveFireFighter : MonoBehaviour {

    /****GENERAL VARIABLES****/
    public int MaxWater = 30;
    private int currentWater = 15;

    /******* FOR PUTTING OUT FIRES ******/
    private Transform barrelEnd;
    private GameObject waterJetprefab;
    private GameObject waterJet;
    private float waterJetLifeTime = 1;

    private bool puttingOutFire = false;
    private GameObject fire;
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

    public void Start()
    {
        //Initialize some objects
        barrelEnd = FindChild("BarrelEnd");
        waterJetprefab = (GameObject)Resources.Load("Prefab/Water Jet");

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        waterJetLifeTime = waterJetprefab.particleSystem.startLifetime;
        move = GetComponent<ReactiveFireFighterMove>();

    }

    /******GUI FUNCTIONS*****/

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), currentWater + "/" + MaxWater);
    }

    public void AddjustCurrentWater(int adj)
    {
        currentWater += adj;
        if (currentWater < 0)
            currentWater = 0;

        if (currentWater > MaxWater)
            currentWater = MaxWater;

        if (MaxWater < 1)
            MaxWater = 1;

        //waterBarLength = (Screen.width / 6) * (currentWater /(float)MaxWater);
    }

    private void decreaseWater(int amount)
    {
        if (currentWater - amount < 0)
            currentWater = 0;
        else
            currentWater -= amount;
    }

    public void refillWater()
    {
        currentWater = MaxWater;
    }

    private IEnumerator decreaseFireHealth(int amount)
    {
        while (fire != null)
        {
            fire.GetComponent<FireStats>().decreaseHealth(1);
            decreaseWater(1);
            yield return new WaitForSeconds(1.0f / gameSpeed);
        }
        preparingToPutOutFire = false;
        Destroy(waterJet);
        puttingOutFire = false;
    }

    public void fireSensor(GameObject bOnFire)
    {
        if (waterJet == null && currentWater > 0)
        {
            attendFire(bOnFire);
        }
    }

    private void attendFire(GameObject bOnFire)
    {
        preparingToPutOutFire = true;
        fire = bOnFire.GetComponent<BuildingScript>().getFire();
    }

    public bool preparingToPutOutFire = false;


   

    private void recalculate()
    {
        collided = false;
        move.recalculate();
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("Agent") || hit.gameObject.transform.tag == "Obstacle")
        {
            if (!collided)
            {
                collided = true;
                readyToMove = false;
                Invoke("recalculate", 0.5f / gameSpeed);
            }
            return;
        }
    }

    public void Update()
    {

        gameSpeed = hub.gameSpeed;

        if (collided)
        {
            transform.Rotate(transform.up, 100 * Time.fixedDeltaTime * gameSpeed);
        }
        if (preparingToPutOutFire && fire != null)
        {
            Vector3 dir = (fire.transform.position - transform.position).normalized;
            dir.y = 0f;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

            Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 1.5f * Time.fixedDeltaTime * gameSpeed, 360);
            transform.rotation = Quaternion.LookRotation(newdir);

            if (transform.rotation == rot && !puttingOutFire)
            {
                puttingOutFire = true;
                barrelEnd.LookAt(fire.transform);
                waterJet = (GameObject)Instantiate(waterJetprefab, barrelEnd.position, barrelEnd.rotation);
                waterJet.particleSystem.startSpeed = (fire.transform.position - barrelEnd.transform.position).magnitude * gameSpeed;
                waterJet.particleSystem.startLifetime = waterJetLifeTime / gameSpeed;
                StartCoroutine("decreaseFireHealth", 1);
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
}
