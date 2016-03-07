using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityGUI : MonoBehaviour {

	public Color red;
	public Color yellow;
	public Color lightRed;
	public Color lightYellow;
	public Color grey;
	Color green;

	float cooldown = 0;

	Image barImage;
	Image backgroundImage;
	void Awake(){
		barImage = transform.Find("Bar").Find("Image").GetComponent<Image>();
		backgroundImage = transform.Find("Background").GetComponent<Image>();
		green = barImage.color;
	}
	public void abilityUsed(float cooldown){
		this.cooldown = cooldown;
		StartCoroutine("gui");
	}
	IEnumerator gui(){
		float time = 0;
		barImage.color = red;
		while(time < cooldown){
			time += Time.deltaTime;
			transform.Find("Bar").localScale = transform.Find("Bar").localScale.setY(time/cooldown);

			barImage.color = Color.Lerp(red, yellow, time/cooldown);
			backgroundImage.color = Color.Lerp(lightRed, lightYellow, time/cooldown);
			yield return new WaitForFixedUpdate();
		}
		transform.Find("Bar").localScale = Vector3.one;
		barImage.color = green;
	}
}
