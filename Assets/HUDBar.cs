using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDBar : MonoBehaviour {


	public enum colours {green, red, yellow, grey}
	public Color green;
	public Color red;
	public Color yellow;
	public Color lightRed;
	public Color lightYellow;
	public Color grey;

	Image foreground;
	Image background;

	void Awake(){
		foreground = transform.Find("foreground").GetComponent<Image>();
		background = transform.Find("background").GetComponent<Image>();
	}

	public void setColor(Color color){
			foreground.color = color;
	}

}
