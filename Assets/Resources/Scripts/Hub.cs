using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hub : MonoBehaviour
{

    private GameObject _firefighterprefab;
    private GameObject _builderprefab;

    private int _numMaxAgents = 16;
    private int _actualNumFF = 4;
    private int _actualNumBuilders = 5;

    public int gameSpeed = 1;
    private bool nightTime = false;
    private bool notAllParked = true;

    public int _buildingsDestroyed;

    private List<GameObject> _parkedfireFighters;
    private List<GameObject> _fireFighters;

    private GameObject _fireFighterLeader;
    private int _fireFighterindex = 0;
    private int _builderindex = 0;
    private int[] _fireFighterPark;
    private int[] _builderPark;

    // Use this for initialization
    void Start()
    {
        _parkedfireFighters = new List<GameObject>();
        _fireFighters = new List<GameObject>();
        GameObject[] fireFighters = GameObject.FindGameObjectsWithTag("FireFighter");
        foreach (GameObject i in fireFighters)
        {
            _fireFighters.Add(i);
        }
        _firefighterprefab = Resources.Load("Prefab/Del_Fireman") as GameObject;
        _builderprefab = Resources.Load("Prefab/Builder") as GameObject;
        _fireFighterPark = new int[_numMaxAgents];
        _builderPark = new int[_numMaxAgents];
        for (int i = 0; i < _numMaxAgents; i++)
        {
            _fireFighterPark[i] = 0;
            _builderPark[i] = 0;
        }
    }

    public void isNightTime(bool night)
    {
        nightTime = night;
        foreach (GameObject i in _fireFighters)
        {
            i.GetComponent<PerceptionInterface>().isNightTime(night);
        }
    }

    public void buildingDestroyed()
    {
        _buildingsDestroyed++;
    }

    private Vector3 getFFNextPos()
    {
        if (_fireFighterindex < 5)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 1) * 4, transform.position.y - 6.73f, transform.position.z - 16);
        if (_fireFighterindex < 9)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 5) * 4, transform.position.y - 6.73f, transform.position.z - 18);
        if (_fireFighterindex < 13)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 9) * 4, transform.position.y - 6.73f, transform.position.z - 20);

        return new Vector3(transform.position.x + 12 - (_fireFighterindex - 13) * 4, transform.position.y - 6.73f, transform.position.z - 22);

    }

    private void spawn()
    {
        GameObject ff = Instantiate(_firefighterprefab, getFFNextPos(), Quaternion.identity) as GameObject;
        ff.GetComponent<PerceptionInterface>().isNightTime(true);
        ff.GetComponent<PerceptionInterface>().setResting(true);
        _fireFighters.Add(ff);
        _parkedfireFighters.Add(ff);
        _fireFighterindex++;
        _actualNumFF++;
    }

    public void spawnFireFighters(int amount)
    {
        if (_fireFighterindex + amount < _fireFighterPark.Length)
        {
            for (int i = 0; i < amount; i++)
            {
                Invoke("spawn", i + 1);
            }
        }
        else
        {
            int k = _fireFighterPark.Length - _fireFighterindex;
            for (int i = 0; i < k; i++)
            {
                Invoke("spawn", i + 1);
            }
        }
    }

    private void parkFireFighter(GameObject firefighter)
    {
        if (_fireFighterindex == 0)
        {
            _fireFighterLeader = firefighter;
            firefighter.GetComponent<NightTimeFireFighter>().electLeader();
            firefighter.GetComponent<NightTimeFireFighter>().startNightTimeBehaviour();
            firefighter.GetComponent<NightTimeFireFighter>().setEndPos(new Vector3(transform.position.x + 6, transform.position.y - 6.73f, transform.position.z - 12));
        }
        else
        {
            firefighter.GetComponent<NightTimeFireFighter>().startNightTimeBehaviour();
            firefighter.GetComponent<NightTimeFireFighter>().setEndPos(getFFNextPos());
        }
        _fireFighterindex++;
        _parkedfireFighters.Add(firefighter);
        if (_fireFighterindex == _actualNumFF)
        {
            notAllParked = false;
            _fireFighterLeader.GetComponent<NightTimeFireFighter>().startCommunicating(_parkedfireFighters, _buildingsDestroyed);
        }
    }

    private void parkBuilder(GameObject builder)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (nightTime && notAllParked)
        {
            if (other.tag == "FireFighter")
            {
                parkFireFighter(other.gameObject);
            }
            if (other.tag == "Builder")
            {
                parkBuilder(other.gameObject);
            }
        }
    }

    void releaseFF()
    {
        foreach (GameObject i in _fireFighters)
        {
            i.GetComponent<NightTimeFireFighter>().reset();
        }
    }

    void releaseBuilders()
    {

    }

    void releaseAll()
    {
        releaseFF();
        releaseBuilders();
    }

    // Update is called once per frame
    void Update()
    {
        if (!nightTime)
        {
            releaseAll();
        }


    }
}
