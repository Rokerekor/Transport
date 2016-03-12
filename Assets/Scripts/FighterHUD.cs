using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FighterHUD : MonoBehaviour {
	
	public GameObject bayObject;
	public GameObject bar;

	public Color red;
	public Color yellow;
	public Color lightRed;
	public Color lightYellow;
	public Color grey;

	Bay bay;


	void Awake(){
		bay = bayObject.GetComponent<SpawnFriendlies>();
	}
	void Start(){
		createBars();
	}
	void createBars(){
		Debug.Log(bay);
		GameObject[] fighters = bay.getShips();
		for(int i=0;i<fighters.Length;i++){
			GameObject newBar = (GameObject)Instantiate(bar, transform.position - (transform.up * ( 5  + i * 25)), transform.rotation);
			newBar.transform.parent = transform;
			StartCoroutine(gui(newBar, fighters[i]));
		}
	}
	IEnumerator gui(GameObject bar, GameObject go){
		Hull fighter =  go.GetComponent<Hull>();
	
		Image barImage = bar.transform.Find("Bar").Find("Image").GetComponent<Image>();
		Image backgroundImage = bar.transform.Find("Background").GetComponent<Image>();
		Color green = barImage.color;

		while(true){
			if(fighter == null){
				barImage.color = Color.black;
				backgroundImage.color = Color.black;
				yield return new WaitForEndOfFrame();
				continue;
			}
			if(!fighter.gameObject.activeSelf){
				barImage.color = grey;
				yield return new WaitForEndOfFrame();
				continue;
			}
			float health = fighter.getHealth() / (float)fighter.getMaxHealth();
			if(health > 0.9f){
				barImage.color = green;
				backgroundImage.color = green;
			}else{
				barImage.color = Color.Lerp(red, green, health);
				backgroundImage.color = Color.Lerp(lightRed, green, health);
			}
			bar.transform.Find("Bar").localScale = bar.transform.Find("Bar").localScale.setY(health);

			yield return new WaitForEndOfFrame();	
		}
	}
}
