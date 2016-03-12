using UnityEngine;
using System.Collections;

public class MissileSalvo : Weapon {
	public GameObject missile;
	public int missileCount;
	public float delay;
	public float width;


	protected override void activate(){
		foreach(Transform child in transform){
			StartCoroutine(spawnMissiles(child.gameObject));
		}

	}
	IEnumerator spawnMissiles(GameObject launch){ 
		int spawned = 0;
		float inc =  width / missileCount;
		float start = width / 2;
		while(spawned < missileCount){
			GameObject newMissile = (GameObject)Instantiate(missile, launch.transform.position + (width / 2 - inc * spawned) * transform.right, launch.transform.rotation);
			newMissile.GetComponent<Missile>().lockOn();
			newMissile.layer = 8;
			spawned++;
			yield return new WaitForSeconds(delay);
		}
	}
}
