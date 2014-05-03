﻿using UnityEngine;
using System.Collections;

public class FireStats : MonoBehaviour {

    private int health;
    private int maxhealth;
    private int damage;
    private Hub hub;
    private int gameSpeed;

    // Use this for initialization
    void Start()
    {
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        if(transform.gameObject.name == "ImprovedFlame(Clone)")
        {
            health = 10;
            maxhealth = 10;
            damage = 2;
        }
        else if (transform.gameObject.name == "GreaterFlame(Clone)")
        {
            health = 20;
            maxhealth = 20;
            damage = 4;
        }
        else
        {
            health = 5;
            maxhealth = 5;
            damage = 1;
        }
        FindChild("FlameBody").transform.Rotate(Vector3.right, 60);
        StartCoroutine("DamageBuilding");
	}

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), health + "/" + maxhealth);
    }

    public void decreaseHealth(int amount)
    {
        health -= amount;
        if (health < 1)
        {
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
}
