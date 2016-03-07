using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FighterHUD : MonoBehaviour {

	public Color red;
	public Color yellow;
	public Color lightRed;
	public Color lightYellow;
	public Color grey;
	Color green;

	Hull fighter;

	Image barImage;
	Image backgroundImage;
	void Awake(){
		barImage = transform.Find("Bar").Find("Image").GetComponent<Image>();
		backgroundImage = transform.Find("Background").GetComponent<Image>();
		green = barImage.color;
	}
	void Start(){
		StartCoroutine("gui");
	}
	IEnumerator gui(){
		while(true){
			if(fighter == null || !fighter.gameObject.activeSelf){
				barImage.color = grey;
				yield return new WaitForEndOfFrame();
				continue;
			}

			float health = fighter.getHealth() / (float)fighter.getMaxHealth();
			transform.Find("Bar").localScale = transform.Find("Bar").localScale.setY(health);
			barImage.color = Color.Lerp(red, yellow, health);
			backgroundImage.color = Color.Lerp(lightRed, green, health);
			yield return new WaitForEndOfFrame();
		}
	}
	public void setFighter(Hull fighter){
		this.fighter = fighter;
	}
}
