using UnityEngine;
using System.Collections;

public class FireStats : MonoBehaviour {

    public int health = 5;
    
    // Use this for initialization
    void Start()
    {
        FindChild("FlameBody").transform.Rotate(Vector3.right, 60);
        StartCoroutine("DamageBuilding");
	}

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), health + "/" + "5");
    }

    public void decreaseHealth(int amount)
    {
        health -= amount;
    }

    private IEnumerator DamageBuilding()
    {
        while (true)
        {
            transform.parent.GetComponent<BuildingScript>().DecreaseHealth(1);
            yield return new WaitForSeconds(1);

        }
    }
	// Update is called once per frame
	void Update () {
	    if(health < 0)
        {
            Destroy(gameObject);
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
