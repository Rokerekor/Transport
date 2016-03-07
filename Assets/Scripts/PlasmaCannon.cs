using UnityEngine;
using System.Collections;

public class PlasmaCannon : MonoBehaviour {

	public float cooldown;
	public GameObject bullet;

	Fighter fighter;


	void Start(){
		StartCoroutine("Shooting");
	}

	IEnumerator Shooting(){
		//yield return new WaitForSeconds(1f);
		for(;;){
			if(Input.GetKeyDown(KeyCode.Space)){
				Bullet.spawnPlasmaBeam(gameObject);
				yield return new WaitForSeconds(cooldown);
			}
			yield return null;
		}
	}
}
