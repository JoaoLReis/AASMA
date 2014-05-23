using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {

    public enum Fires : int {NONE, SIMPLE, IMPROVED, GREATER};

	public int maxHealth = 200;

	private GameObject fireEffect = null;
    private GameObject prefabSimple;
    private GameObject prefabImproved;
    private GameObject prefabGreater;
    private Fires fireState = 0;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/

	private int curHealth = 200;

	void Start()
	{
        prefabSimple = Resources.Load("Prefab/Flame") as GameObject;
        prefabImproved = Resources.Load("Prefab/ImprovedFlame") as GameObject;
        prefabGreater = Resources.Load("Prefab/GreaterFlame") as GameObject;
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        int randomizer = Random.Range(30/gameSpeed, 300/gameSpeed);
        Invoke("generateFires", randomizer);
        hub._buildingsCreated++;
	}

    public GameObject getFire()
    {
        return fireEffect;
    }

    private bool IsMouseOver()
    {
        return Event.current.type == EventType.Repaint &&
        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
    }

	void OnGUI()
	{		
		Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
        GUI.color = Color.cyan;
        GUI.HorizontalScrollbar(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), 0, curHealth, 0, maxHealth);
        GUI.color = Color.white;
        GUI.Label(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), " " + curHealth + "/" + maxHealth);
	}
	
	void Update()
	{
        gameSpeed = hub.gameSpeed;
	}

    public bool inNeedOfRepair()
    {
        return (curHealth < maxHealth && fireEffect == null);
    }

    public void repair(int amount)
    {  
        if (curHealth <= maxHealth)
        {
            if (curHealth + amount > maxHealth)
                curHealth = maxHealth;
            else
                curHealth += amount;
        }
    }

	public void DecreaseHealth(int amount)
	{
        curHealth -= amount;
        if (curHealth <= 0)
        {
            transform.parent.GetComponent<BuildAreaScript>().RebuildableArea();
            fireState = Fires.NONE;
        }
	}
	
	public void putOutFire()
	{
		fireEffect.SetActive(false);
	}

	void OnMouseDown() {
        FirePerception scrpt;
        bool btcof;
        switch(fireState)
        {
            case  Fires.NONE:
                Destroy(fireEffect);
                fireEffect = (GameObject)Instantiate(prefabSimple, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                fireEffect.transform.parent = transform;
                fireEffect.transform.Translate(3.75f * Vector3.forward);
                fireState = Fires.SIMPLE;
                break;
            case Fires.SIMPLE:
                scrpt = fireEffect.GetComponent<FirePerception>();
                if (!scrpt.beingTakenCareOf)
                {
                    btcof = scrpt.beingTakenCareOf;
                    Destroy(fireEffect);
                    fireEffect = (GameObject)Instantiate(prefabImproved, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                    fireEffect.transform.parent = transform;
                    fireEffect.transform.Translate(3.75f * Vector3.forward);
                    scrpt = fireEffect.GetComponent<FirePerception>();
                    scrpt.beingTakenCareOf = btcof;
                    fireState = Fires.IMPROVED;
                }
                break;
            case Fires.IMPROVED:
                scrpt = fireEffect.GetComponent<FirePerception>();
                if (!scrpt.beingTakenCareOf)
                {
                    btcof = scrpt.beingTakenCareOf;
                    Destroy(fireEffect);
                    fireEffect = (GameObject)Instantiate(prefabGreater, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                    fireEffect.transform.parent = transform;
                    fireEffect.transform.Translate(3.75f * Vector3.forward);
                    scrpt = fireEffect.GetComponent<FirePerception>();
                    scrpt.beingTakenCareOf = btcof;
                }
                break;
            case Fires.GREATER:
                break;
            default:
                break;
        }
    }

    private void generateFires()
    {
        int randomizer = Random.Range(30 / gameSpeed, 300 / gameSpeed);
        if (hub.getNightTime())
        {
            randomizer = Random.Range(30 / gameSpeed, 300 / gameSpeed);
            Invoke("generateFires", randomizer);
            return;
        }
        FirePerception scrpt;
        bool btcof;
        switch (fireState)
        {
            case Fires.NONE:
                randomizer = Random.Range(30 / gameSpeed, 300 / gameSpeed);
                Destroy(fireEffect);
                fireEffect = (GameObject)Instantiate(prefabSimple, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                fireEffect.transform.parent = transform; 
                fireEffect.GetComponent<FirePerception>().beingTakenCareOf = false;
                fireState = Fires.SIMPLE;
                break;
            case Fires.SIMPLE:
                randomizer = Random.Range(60 / gameSpeed, 450 / gameSpeed);
                scrpt = fireEffect.GetComponent<FirePerception>();
                if (!scrpt.beingTakenCareOf)
                {
                    btcof = scrpt.beingTakenCareOf;
                    Destroy(fireEffect);
                    fireEffect = (GameObject)Instantiate(prefabImproved, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                    fireEffect.transform.parent = transform;
                    scrpt = fireEffect.GetComponent<FirePerception>();
                    scrpt.beingTakenCareOf = btcof;
                    fireState = Fires.IMPROVED;
                }
                break;
            case Fires.IMPROVED:
                randomizer = Random.Range(150 / gameSpeed, 600 / gameSpeed);
                scrpt = fireEffect.GetComponent<FirePerception>();
                if (!scrpt.beingTakenCareOf)
                {
                    btcof = scrpt.beingTakenCareOf;
                    Destroy(fireEffect);
                    fireEffect = (GameObject)Instantiate(prefabGreater, new Vector3(transform.position.x, transform.position.y + 9.0f, transform.position.z), transform.rotation);
                    fireEffect.transform.parent = transform;
                    scrpt = fireEffect.GetComponent<FirePerception>();
                    scrpt.beingTakenCareOf = btcof;
                }
                break;
            case Fires.GREATER:
                return;
            default:
                break;
        }
        fireEffect.transform.parent = transform;
        fireEffect.transform.Translate(3.75f * Vector3.forward);

        fireState++;
        Invoke("generateFires", randomizer);
    }

	public void startFire()
	{
		fireEffect.SetActive(true);
	}

	public bool onFire()
	{
		return fireEffect.activeSelf;
	}

	public bool destroyed()
	{
		if(curHealth == 0)
			return true;
		else return false;
	}

    public void setFireState(Fires state)
    {
        fireState = state;
    }
}
