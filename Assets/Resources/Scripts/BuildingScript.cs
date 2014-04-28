using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {

	public int maxHealth = 200;

	private GameObject fireEffect = null;
	public GameObject prefab;

	private int curHealth = 200;

	void Start()
	{
        int randomizer = Random.Range(30, 300);
        Invoke("generateFires", randomizer);
	}

    public GameObject getFire()
    {
        return fireEffect;
    }

	void OnGUI()
	{		
		Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Box(new Rect(targetPos.x, Screen.height- targetPos.y, 60, 20), curHealth + "/" + maxHealth);
	}
	
	void Update()
	{
	}

	public void DecreaseHealth(int amount)
	{
		curHealth -= amount;
	}
	
	public void putOutFire()
	{
		fireEffect.SetActive(false);
	}

	void OnMouseDown() {
        fireEffect = (GameObject)Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        fireEffect.transform.parent = transform;
        fireEffect.transform.Translate(3.75f * Vector3.forward);
    }

    private void generateFires()
    {
        int randomizer = Random.Range(30, 150);
        fireEffect = (GameObject)Instantiate(prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        fireEffect.transform.parent = transform;
        fireEffect.transform.Translate(3.75f * Vector3.forward);
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
}
