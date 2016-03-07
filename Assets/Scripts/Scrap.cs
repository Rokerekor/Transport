using UnityEngine;
using System.Collections;

public class Scrap : MonoBehaviour {
	GameObject scrapper;
	public void setScrapper(GameObject val){
		scrapper = val;
	}
	public GameObject getScrapper(){
		return scrapper;
	}
}
