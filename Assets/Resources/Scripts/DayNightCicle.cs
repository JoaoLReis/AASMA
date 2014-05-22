using UnityEngine;
using System.Collections;

public class DayNightCicle : MonoBehaviour {

    private float speed;

    private Hub hub;
    private float gameSpeed;

    private bool daytime = false;

	// Use this for initialization
	void Start () {
        speed = 0.25f;
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
	}
	
	// Update is called once per frame
    void Update()
    {
        gameSpeed = hub.gameSpeed;
        transform.Rotate(speed * Time.deltaTime * gameSpeed, 0, 0);
        //Debug.Log(transform.rotation.eulerAngles);
        if (Vector3.Dot(Vector3.up, transform.forward) > 0)
        {
            if (!daytime)
            {
                speed *= 4f;
                hub.isNightTime(true);
                daytime = true;
            }
        }
        else
        {
            if (daytime)
            {
                speed /= 4f;
                hub.isNightTime(false);
                daytime = false;
            }
        }
	}
}
