using UnityEngine;
using System.Collections;

public class ScrapBay : Bay {

	public HUDBar hudbar;
	bool active;

	void Start(){
		ships = new GameObject[100];
		for(int i=0; i<ships.Length; i++){
			ships[i] = (GameObject)Instantiate(shipPrefab, transform.position, transform.rotation);
			ships[i].layer = gameObject.layer;
			//GameObject.FindGameObjectWithTag("HUD").transform.Find("Ability" + (i+1)).GetComponent<FighterHUD>().setFighter(ships[i].GetComponent<Fighter>());
			ships[i].SetActive(false);
		}

	}
	void Update(){
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			active = !active;
			if(active){
				StartCoroutine(SpawnManager());
				hudbar.setColor(hudbar.green);
			}else{
				StopCoroutine(SpawnManager());
				hudbar.setColor(hudbar.red);
			}
		}
	}
	IEnumerator SpawnManager(){
		while(true){
			if(active){
				GameObject[] scraps = GameObject.FindGameObjectsWithTag("Scrap");
				foreach(GameObject scrap in scraps){
					if(Vector3.Distance(scrap.transform.position, transform.position) < 20f){
						if(scrap.GetComponent<Scrap>().getScrapper() == null){
							spawnScrapper(scrap);
							yield return new WaitForSeconds(0.1f);
						}
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
