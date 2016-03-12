using UnityEngine;
using System.Collections;

public class SpawnFriendlies : Bay {

	public GameObject commandMenu;

	void Start(){
		ships = new GameObject[10];
		for(int i=0; i<ships.Length; i++){
			ships[i] = (GameObject)Instantiate(shipPrefab, transform.position, transform.rotation);
			ships[i].layer = gameObject.layer;
			ships[i].SetActive(false);
			ships[i].GetComponent<Fighter>().setFriendly(true);
		}
		StartCoroutine(SpawnManager());
		StartCoroutine(CommandManager());
	}
	IEnumerator CommandManager(){
		CommandMenu cm = commandMenu.GetComponent<CommandMenu>();
		while(true){
			GameObject currentOrder = cm.getCurrentOrder();
			if(currentOrder != null){
				giveOrder(currentOrder);
				yield return new WaitUntil(() => currentOrder == null);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	IEnumerator SpawnManager(){
		while(true){
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				GameObject ship = sendShip();
				if(ship != null){
					Fighter fighter = ship.GetComponent<Fighter>();
					fighter.setBay(gameObject);
					fighter.commandLeaveBay();
					fighter.setHome(transform.parent.gameObject);
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}
	void giveOrder(GameObject order){
		foreach(GameObject ship in ships){
			if(ship != null && ship.activeSelf){
				Fighter fighter = ship.GetComponent<Fighter>();
				fighter.commandOrder(order);
			}
		}
	}
	void spawnFriendly(){
		foreach(GameObject ship in ships){
			if(ship != null && !ship.activeSelf){
				ship.SetActive(true);
				ship.transform.position = transform.position;
				ship.transform.rotation = transform.rotation;
				return;
			}
		}
	}
}
	