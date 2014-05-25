using UnityEngine;
using System.Collections;

public class FireStats : MonoBehaviour {

    private int health;
    private int maxhealth;
    private float damage;
    private Hub hub;
    private int gameSpeed;

    // Use this for initialization
    void Start()
    {
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        if(transform.gameObject.name == "ImprovedFlame(Clone)")
        {
            health = 50;
            maxhealth = 50;
            damage = 0.25f;
        }
        else if (transform.gameObject.name == "GreaterFlame(Clone)")
        {
            health = 100;
            maxhealth = 100;
            damage = 0.5f;
        }
        else
        {
            health = 5;
            maxhealth = 5;
            damage = 0.25f;
        }
        FindChild("FlameBody").transform.Rotate(Vector3.right, 60);
        StartCoroutine("DamageBuilding");
	}

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.color = new Color(1f,0f,0f);
        GUI.HorizontalScrollbar(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), 0, health, 0, maxhealth);
        GUI.color = Color.white;
        GUI.Label(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), " " + health + "/" + maxhealth);
    }

    public void decreaseHealth(int amount)
    {
        health -= amount;
        if (health < 1)
        {
            transform.parent.GetComponent<BuildingScript>().setFireState(BuildingScript.Fires.NONE);
            Debug.LogWarning("FIRE DEAD");
            hub.incfiresPutOut();
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageBuilding()
    {
        while (true)
        {
            transform.parent.GetComponent<BuildingScript>().DecreaseHealth(1);
            yield return new WaitForSeconds(1.0f/(damage * gameSpeed));

        }
    }
	// Update is called once per frame
	void Update () {
        gameSpeed = hub.gameSpeed;

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

    public int getHealth()
    {
        return health;
    }
}
