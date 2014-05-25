using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Hub : MonoBehaviour
{

    private GameObject _firefighterprefab;
    private GameObject _builderprefab;
    
    public int _numMaxAgents = 16;
    private int _actualNumFF = 8;
    private int _actualNumBuilders = 5;

    public int purifiant;

    public int gameSpeed = 1;
    private bool nightTime = false;
    public bool notAllParked = true;

    private int _totalFires = 0;
    private int _totalBuildings = 0;
    private int _buildingsCreated = 0;
    private int _buildingsDestroyed = 0;

    private List<GameObject> _parkedfireFighters;
    private List<GameObject> _fireFighters;

    private GameObject _fireFighterLeader;
    private int _fireFighterindex = 0;
    private int _builderindex = 0;
    private int _numberDay = 0;

    // Use this for initialization
    void Start()
    {
        purifiant = 0;
        _parkedfireFighters = new List<GameObject>();
        _fireFighters = new List<GameObject>();
        GameObject[] fireFighters = GameObject.FindGameObjectsWithTag("FireFighter");
        foreach (GameObject i in fireFighters)
        {
            _fireFighters.Add(i);
        }
        _firefighterprefab = Resources.Load("Prefab/Del_Fireman") as GameObject;
        _builderprefab = Resources.Load("Prefab/Builder") as GameObject;
    }

    void OnGUI()
    {
        Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
        Rect rt = new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20);
        GUI.Box(rt, "  P:" + purifiant);
    }

    public bool getNightTime() 
    { 
        return nightTime; 
    }

    public void isNightTime(bool night)
    {
        nightTime = night;
        if (night)
        {
            StreamWriter file2 = new StreamWriter("Report.txt", true);
            file2.WriteLine("Night" + _numberDay + ":" + "\nNumber Buildings destroyed: " + _buildingsDestroyed + "\nNumber Buildings created: " + _buildingsCreated + "\nTotal Number of Buildings: " + _totalBuildings + "\nNumber of FireMan: " + _actualNumFF + "\nNumber of fires put out: " + _totalFires);
            file2.Close();
            BoxCollider box = GetComponent<BoxCollider>();
            foreach (GameObject i in _fireFighters)
            {
                i.GetComponent<PerceptionInterface>().isNightTime(night);
                float iposx = i.transform.position.x;
                float iposz = i.transform.position.z;
                float rightLimit = (transform.position.x + box.size.x * transform.localScale.x / 2.0f);
                float leftLimit = (transform.position.x - box.size.x * transform.localScale.x / 2.0f);
                float botLimit = (transform.position.z + box.center.z - box.size.z * transform.localScale.z / 2.0f);
                float upLimit = (transform.position.z + box.center.z + box.size.z * transform.localScale.z / 2.0f);

                //Debug.LogWarning("iposx: " + iposx);
               // Debug.LogWarning("iposz: " + iposz);
               // Debug.LogWarning("rightLimit: " + rightLimit);
               // Debug.LogWarning("leftLimit: " + leftLimit);
               // Debug.LogWarning("botLimit: " + botLimit);
               // Debug.LogWarning("upLimit: " + upLimit);
                if ((iposx < rightLimit) && (iposx > leftLimit) && (i.transform.position.z > botLimit) && (i.transform.position.z < upLimit))
                {
                    parkFireFighter(i);
                }
            }

        }
        else
        {
            StreamWriter file2 = new StreamWriter("Report.txt", true);
            file2.WriteLine("Day" + ++_numberDay);
            file2.Close();
            foreach (GameObject i in _fireFighters)
            {
                i.GetComponent<PerceptionInterface>().isNightTime(night);
            }
            releaseAll();
        }
    }

    public void buildingDestroyed()
    {
        _buildingsDestroyed++;
        _totalBuildings -= 1 ;
    }

    public void buildingCreated()
    {
        _buildingsCreated++;
        _totalBuildings += 1;
    }

    public void incfiresPutOut()
    {
        _totalFires++;
    }

    private Vector3 getFFNextPos()
    {
        if (_fireFighterindex < 5)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 1) * 4, transform.position.y - 6.73f, transform.position.z - 12);
        if (_fireFighterindex < 9)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 5) * 4, transform.position.y - 6.73f, transform.position.z - 14);
        if (_fireFighterindex < 13)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 9) * 4, transform.position.y - 6.73f, transform.position.z - 16);
        if (_fireFighterindex < 17)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 13) * 4, transform.position.y - 6.73f, transform.position.z - 18);
        if (_fireFighterindex < 21)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 17) * 4, transform.position.y - 6.73f, transform.position.z - 20);
        if (_fireFighterindex < 25)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 21) * 4, transform.position.y - 6.73f, transform.position.z - 22);
        if (_fireFighterindex < 29)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 25) * 4, transform.position.y - 6.73f, transform.position.z - 24);
        if (_fireFighterindex < 33)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 29) * 4, transform.position.y - 6.73f, transform.position.z - 26);
        if (_fireFighterindex < 37)
            return new Vector3(transform.position.x + 12 - (_fireFighterindex - 33) * 4, transform.position.y - 6.73f, transform.position.z - 28);

        return new Vector3(transform.position.x + 12 - (_fireFighterindex - 37) * 4, transform.position.y - 6.73f, transform.position.z - 30);
    }

    private void spawn()
    {
        GameObject ff = Instantiate(_firefighterprefab, getFFNextPos(), Quaternion.identity) as GameObject;
        ff.GetComponent<PerceptionInterface>().isNightTime(true);
        ff.GetComponent<PerceptionInterface>().setResting(true);
        _fireFighters.Add(ff);
        _parkedfireFighters.Add(ff);
        _buildingsDestroyed = 0;
        _fireFighterindex++;
        _actualNumFF++;
    }

    private void prepareSpawn(int amount)
    {
        if (_fireFighterindex + amount < _numMaxAgents)
        {
            for (int i = 0; i < amount; i++)
            {
                Invoke("spawn", (i + 0.5f)/gameSpeed);
            }
        }
        else
        {
            int k = _numMaxAgents - _fireFighterindex;
            for (int i = 0; i < k; i++)
            {
                Invoke("spawn", (i + 0.5f) / gameSpeed);
            }
        }
    }

    public void spawnFireFighters(int amount)
    {
        StartCoroutine("prepareSpawn", amount);
    }

    private void parkFireFighter(GameObject firefighter)
    {
        if (_fireFighterindex == 0)
        {
            _fireFighterLeader = firefighter;
            firefighter.GetComponent<NightTimeFireFighter>().electLeader();
            firefighter.GetComponent<NightTimeFireFighter>().startNightTimeBehaviour();
            firefighter.GetComponent<NightTimeFireFighter>().setEndPos(new Vector3(transform.position.x + 6, transform.position.y - 6.73f, transform.position.z - 10));
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
            _fireFighterLeader.GetComponent<NightTimeFireFighter>().startCommunicating(_parkedfireFighters, _buildingsDestroyed, _totalBuildings);
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
                StartCoroutine("checkInsiders");
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

    public IEnumerator checkInsiders()
    {
        while (true)
        {
            if (nightTime && notAllParked)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                foreach (GameObject i in _fireFighters)
                {
                    if (_parkedfireFighters.Contains(i))
                        continue;
                    i.GetComponent<PerceptionInterface>().isNightTime(nightTime);
                    float iposx = i.transform.position.x;
                    float iposz = i.transform.position.z;
                    float rightLimit = (transform.position.x + box.size.x * transform.localScale.x / 2.0f);
                    float leftLimit = (transform.position.x - box.size.x * transform.localScale.x / 2.0f);
                    float botLimit = (transform.position.z + box.center.z - box.size.z * transform.localScale.z / 2.0f);
                    float upLimit = (transform.position.z + box.center.z + box.size.z * transform.localScale.z / 2.0f);

                    //Debug.LogWarning("iposx: " + iposx);
                    //Debug.LogWarning("iposz: " + iposz);
                    //Debug.LogWarning("rightLimit: " + rightLimit);
                    //Debug.LogWarning("leftLimit: " + leftLimit);
                    //Debug.LogWarning("botLimit: " + botLimit);
                    //Debug.LogWarning("upLimit: " + upLimit);
                    if ((i.transform.position.x < rightLimit) && (i.transform.position.x > leftLimit) && (iposz > botLimit) && (iposz < upLimit))
                    {
                        parkFireFighter(i);
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(1.5f / gameSpeed);
            }
            yield return null;
        }
    }

    void releaseBuilders()
    {

    }

    void releaseAll()
    {
        releaseFF();
        releaseBuilders();
        notAllParked = true;
        _fireFighterindex = 0;
        _parkedfireFighters.Clear();
        _buildingsCreated = 0;
        _buildingsDestroyed = 0;
        _builderindex = 0;
        _totalFires = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
