using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class DayNightCicle : MonoBehaviour {

    private float speed;
    public float daySpeed = 0.15f;
    public float nigthSpeed = 1;

    private Hub hub;
    private float gameSpeed;

    private bool daytime = false;

	// Use this for initialization
	void Start () {
        speed = 0.25f;
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        gameSpeed = hub.gameSpeed;
        if (!File.Exists("Report.txt"))
        {
            Debug.Log("Creating File");
            File.Create("Report.txt");
        }
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
                speed = nigthSpeed;
                hub.isNightTime(true);
                daytime = true;
            }
        }
        else
        {
            if (daytime)
            {
                speed = daySpeed;
                hub.isNightTime(false);
                daytime = false;
            }
        }
	}
}
