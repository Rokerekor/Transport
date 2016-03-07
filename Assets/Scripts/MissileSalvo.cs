using UnityEngine;
using System.Collections;

public class MissileSalvo : Weapon {
	public GameObject missile;
	public int missileCount;
	public float delay;


	protected override void activate(){
		StartCoroutine("spawnMissiles");
	}
	IEnumerator spawnMissiles(){ 
		int spawned = 0;
		while(spawned < missileCount){
			GameObject newMissile = (GameObject)Instantiate(missile, transform.position, transform.rotation);
			newMissile.GetComponent<Missile>().lockOn();
			newMissile.layer = 8;
			spawned++;
			yield return new WaitForSeconds(delay);
		}
	}
}
