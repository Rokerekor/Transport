using UnityEngine;
using System.Collections;

public class FighterShooting : MonoBehaviour {

	public float cooldown;
	public GameObject bullet;

	Fighter fighter;

	void Awake () {
		fighter =  GetComponent<Fighter>();
	}
	void Start(){
		StartCoroutine("Shooting");
	}
	IEnumerator Shooting(){
		//yield return new WaitForSeconds(1f);
		for(;;){
			if(fighter.canShoot()){
				Bullet.spawnBullet(gameObject);
				yield return new WaitForSeconds(cooldown);
			}
			yield return null;
		}
	}
}
