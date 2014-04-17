using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {

	public int maxHealth = 200;

	private GameObject fireEffect;
	public GameObject prefab;

	private int curHealh = 200;

	void Start()
	{
		Object obj = Instantiate(prefab, new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
		fireEffect = (GameObject) obj;
		fireEffect.SetActive(false);
	}

	void Update()
	{
	}

	public void putOutFire()
	{
		fireEffect.SetActive(false);
	}

	void OnMouseDown() {
		fireEffect.SetActive(!fireEffect.activeSelf);
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
		if(curHealh == 0)
			return true;
		else return false;
	}

	public void turn2Wreck()
	{

	}
}
