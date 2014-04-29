using UnityEngine;
using System.Collections;

public class DayNightCicle : MonoBehaviour {

    private float speed;

    private Hub hub;
    private float gameSpeed;

	// Use this for initialization
	void Start () {
        speed = 0.5f;
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(speed * Time.deltaTime * gameSpeed, 0, 0);
	}
}
