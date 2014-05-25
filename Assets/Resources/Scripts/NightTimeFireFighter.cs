﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class NightTimeFireFighter : MonoBehaviour {

    private int _numFires = 0;
    private int _totalBuildings = 0;
    private int _buildingsDestroyed = 0;
    public bool _leader = false;
    private PerceptionInterface agent;
    private Vector3 endPos;
    private bool movingToPos = false;
    private bool rotatingToFront = false;
    private bool _preparingToTalk = false;
    private bool _talking = false;
    private bool _talkWithBuilder = false;
    private GameObject _target;
    private List<GameObject> _firefighters;

    /*********** FOR GLOBAL GAME SPEED ********/
    private Hub hub;
    private int gameSpeed = 1;
    /*****************************************/


    // Use this for initialization
    void Start() {
        agent = gameObject.GetComponent<PerceptionInterface>();
        hub = GameObject.FindWithTag("Hub").GetComponent<Hub>();
        _firefighters = new List<GameObject>();
        gameSpeed = hub.gameSpeed;
        StartCoroutine(test(2f));
	}

    public void startNightTimeBehaviour()
    {
        agent.setResting(true);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void electLeader()
    {
        _leader = true;
        agent.electLeader();
    }

    public void startCommunicating(List<GameObject> others, int buildingsDestroyed, int totalBuildings)
    {
        _totalBuildings = totalBuildings;
        _buildingsDestroyed = buildingsDestroyed;
        _firefighters.AddRange(others);
        talkWithEveryone();
    }

    public void talkWithEveryone()
    {
        if (_firefighters.Count == 0)
        {
            //StreamWriter file2 = new StreamWriter("Report.txt", true);
            //file2.WriteLine("\nNumber Fires put out: " + _numFires);
            //file2.Close();
            _talking = false;
            Debug.LogWarning("NUM FIRES: " + _numFires);
            if (_numFires == 0)
            {
                hub.spawnFireFighters(2);
                return;
            }
            Debug.LogWarning(_totalBuildings);
            if (_buildingsDestroyed >= 0.50 * _totalBuildings)
            {
                hub.spawnFireFighters(5);
                return;
            }
            if (_buildingsDestroyed >= 0.40 * _totalBuildings)
            {
                hub.spawnFireFighters(4);
                return;
            }
            if (_buildingsDestroyed >= 0.20 * _totalBuildings)
            {
                hub.spawnFireFighters(2);
                return;
            }
            if (_buildingsDestroyed >= 0.05 * _totalBuildings)
            {
                hub.spawnFireFighters(1);
                return;
            }
            return;
        }
        _target = _firefighters.First();
        _firefighters.Remove(_target);
        Invoke("preparedToTalk", 1.0f / gameSpeed);
    }

    public void setEndPos(Vector3 end)
    {
        endPos = end;
        movingToPos = true;
    }

    public void reset()
    {
        _numFires = 0;
        _buildingsDestroyed = 0;
        _leader = false;
        _firefighters.Clear();
        movingToPos = false;
        _preparingToTalk = false;
        _talking = false;
        rotatingToFront = false;
        GetComponent<Rigidbody>().isKinematic = false;
        agent.reset();
    }

    IEnumerator test(float waitTime)
    {
        while (true)
        {
            if (movingToPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPos, 10 * Time.deltaTime * gameSpeed);
                transform.position = endPos;
                if (transform.position == endPos)
                {
                    movingToPos = false;
                    rotatingToFront = true;
                }
            }
            if (rotatingToFront)
            {
                Vector3 dir;
                if (_leader)
                {
                    dir = -Vector3.forward;

                }
                else
                {
                    dir = Vector3.forward;
                }
                dir.y = 0f;

                Quaternion rot = transform.rotation;
                rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

                Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 3.0f * Time.deltaTime * gameSpeed, 360);
                transform.rotation = Quaternion.LookRotation(newdir);

                transform.rotation = rot;
                if (transform.rotation == rot)
                    rotatingToFront = false;

            }
            if (_leader)
            {
                if (_preparingToTalk)
                {
                    Vector3 dir = (_target.transform.position - transform.position).normalized;
                    dir.y = 0f;

                    Quaternion rot = transform.rotation;
                    rot.SetLookRotation(dir, new Vector3(0f, 1f, 0f));

                    Vector3 newdir = Vector3.RotateTowards(transform.forward, dir, 2.0f * Time.deltaTime * gameSpeed, 360);
                    transform.rotation = Quaternion.LookRotation(newdir);

                    if (transform.rotation == rot)
                    {
                        _preparingToTalk = false;
                        _talking = true;
                    }
                }
                if (_talking)
                {
                    _numFires += _target.GetComponent<PerceptionInterface>().numFiresPutOut();
                    _talking = false;
                    talkWithEveryone();
                }
            }
            if (hub.getNightTime())
            {
                if (!_talking)
                {
                    yield return new WaitForSeconds(0.005f);
                }
                yield return new WaitForSeconds(0.005f);
            }
            else yield return new WaitForSeconds(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameSpeed = hub.gameSpeed;
    }

    public void preparedToTalk()
    {
        _preparingToTalk = true;
    }
}
