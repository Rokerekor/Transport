using UnityEngine;
using System.Collections;

public class FighterBay : Bay {
	const int IDLECOUNT = 4;
	void Awake(){
		ships = new GameObject[10];
	}
	void Start(){
		for(int i=0; i<ships.Length; i++){
			ships[i] = (GameObject)Instantiate(shipPrefab, transform.position, transform.rotation);

			Fighter newFighter = ships[i].GetComponent<Fighter>();
			newFighter.setHome(transform.parent.gameObject);
			newFighter.setBay(gameObject);
			newFighter.transform.parent = transform;
			ships[i].SetActive(false);
			ships[i].GetComponent<Fighter>().setFriendly(false);

		}
		StartCoroutine(SpawnManager());
	}

	IEnumerator SpawnManager(){
		while(true){
			GameObject enemy = gameObject.findNearestEnemy(5f);
			bool spawnFighter = false;
			int active = activeFighters();
			if(enemy != null){
				spawnFighter = true;
			}else{
				if(active < IDLECOUNT){
					spawnFighter = true;
				}else if(active > IDLECOUNT){
					returnShip();
				}
			}
			if(spawnFighter){
				GameObject ship = sendShip();
				if(ship != null){
					Fighter newFighter = ship.GetComponent<Fighter>();
					newFighter.commandLeaveBay();
					yield return new WaitForSeconds(0.75f);
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}

}
