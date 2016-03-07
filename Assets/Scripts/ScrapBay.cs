using UnityEngine;
using System.Collections;

public class ScrapBay : Bay {

	void Start(){
		ships = new GameObject[100];
		for(int i=0; i<ships.Length; i++){
			ships[i] = (GameObject)Instantiate(shipPrefab, transform.position, transform.rotation);
			ships[i].layer = gameObject.layer;
			//GameObject.FindGameObjectWithTag("HUD").transform.Find("Ability" + (i+1)).GetComponent<FighterHUD>().setFighter(ships[i].GetComponent<Fighter>());
			ships[i].SetActive(false);
		}
		StartCoroutine("SpawnManager");
	}

	IEnumerator SpawnManager(){
		while(true){
			if(Input.GetKeyDown(KeyCode.Alpha2)){
				GameObject[] scraps = GameObject.FindGameObjectsWithTag("Scrap");
				foreach(GameObject scrap in scraps){
					if(Vector3.Distance(scrap.transform.position, transform.position) < 20f){
						spawnScrapper(scrap);
						yield return new WaitForSeconds(0.1f);
					}
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void spawnScrapper(GameObject scrap){
		if(scrap == null)
			return;
		GameObject scrapper = sendShip();
		scrapper.GetComponent<ScrapCollector>().setBay(gameObject);
		scrapper.GetComponent<ScrapCollector>().setScrap(scrap);
		scrap.GetComponent<Scrap>().setScrapper(scrapper);
	}
}
