using UnityEngine;
using System.Collections;

public class Bay : MonoBehaviour {
	public GameObject shipPrefab;
	internal GameObject[] ships;

	const float OFFSET = 0.25f;

	internal GameObject sendShip(){
		foreach(GameObject ship in ships){
			if(ship != null && !ship.activeSelf){
				ship.SetActive(true);
				ship.transform.position = transform.position + transform.right * Random.Range(-OFFSET, OFFSET);
				ship.transform.rotation = transform.rotation;

				return ship;
			}
		}
		return null;
	}
	internal void returnShip(){
		foreach(GameObject ship in ships){
			if(ship != null && ship.activeSelf){
				Fighter newFighter = ship.GetComponent<Fighter>();
				newFighter.commandEnterBay();
				return;
			}
		}
	}

	internal int activeFighters(){
		int active = 0;
		foreach(GameObject ship in ships){
			if(ship != null && ship.activeSelf){
				active++;
			}
		}
		return active;
	}
	public GameObject[] getShips(){
		return ships;
	}
}
