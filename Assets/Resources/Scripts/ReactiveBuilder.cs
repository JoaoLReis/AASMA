using UnityEngine;
using System.Collections;

public class ReactiveBuilder : MonoBehaviour, ReactiveInterface {

    /****GENERAL VARIABLES****/
    public int maxBuildingMaterials = 30;
	private int currentBuildingMaterials = 25;

    /******* FOR Building ******/
    private Transform barrelEnd;
    private GameObject buildingJetprefab;
    private GameObject buildingJet;
    private float buildJetLifeTime = 1;

    private bool building = false;
	public bool preparingToBuild = false;
    private GameObject freeBuildArea;
    /************************************/

	private ReactiveBuilderMove move;

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
		buildingJetprefab = (GameObject)Resources.Load("Prefab/Build Jet");

        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
		buildJetLifeTime = buildingJetprefab.particleSystem.startLifetime;
		move = GetComponent<ReactiveBuilderMove>();
    }

    /******GUI FUNCTIONS*****/

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), currentBuildingMaterials + "/" + maxBuildingMaterials);
    }

    public void AddjustCurrentBuildingMaterials(int adj)
    {
		currentBuildingMaterials += adj;
		if (currentBuildingMaterials < 0)
			currentBuildingMaterials = 0;

		if (currentBuildingMaterials > maxBuildingMaterials)
			currentBuildingMaterials = maxBuildingMaterials;

		if (maxBuildingMaterials < 1)
			maxBuildingMaterials = 1;

        //waterBarLength = (Screen.width / 6) * (currentWater /(float)MaxWater);
    }

    private void decreaseBuildingMaterials(int amount)
    {
		if (currentBuildingMaterials - amount < 0)
		{
			currentBuildingMaterials = 0;
            preparingToBuild = false;
            Destroy(buildingJet);
			building = false;
            freeBuildArea = null;
		}
		else
			currentBuildingMaterials -= amount;
    }

    public void refillBuildingMaterials()
    {
		currentBuildingMaterials = maxBuildingMaterials;
    }

    private IEnumerator buildArea(int amount)
    {
		while (building)
        {
            if(freeBuildArea.GetComponent<BuildAreaScript>().buildArea(1) == true)
			{
				preparingToBuild = false;
				Destroy(buildingJet);
                building = false;
                freeBuildArea = null;
			}
            else
            {
                decreaseBuildingMaterials(1);
                yield return new WaitForSeconds(1.0f / gameSpeed);
            }
        }
    }

    public void buildSensor(GameObject fBuildArea)
    {
		if (buildingJet == null && currentBuildingMaterials > 0 && building == false)
        {
			preparingToBuild = true;
			freeBuildArea = fBuildArea;
        }
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
        if (preparingToBuild && freeBuildArea != null)
        {
			Vector3 dir = (freeBuildArea.transform.position - transform.position).normalized;
            dir.y = 0f;

            Quaternion rot = transform.rotation;
            rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

            Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 1.5f * Time.fixedDeltaTime * gameSpeed, 360);
            transform.rotation = Quaternion.LookRotation(newdir);
			
            if (transform.rotation == rot && !building)
            {
				building = true;
				barrelEnd.LookAt(freeBuildArea.transform);
				buildingJet = (GameObject)Instantiate(buildingJetprefab, barrelEnd.position, barrelEnd.rotation);
				buildingJet.particleSystem.startSpeed = (freeBuildArea.transform.position - barrelEnd.transform.position).magnitude * gameSpeed;
				buildingJet.particleSystem.startLifetime = buildJetLifeTime / gameSpeed;
				StartCoroutine("buildArea", 1);
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
