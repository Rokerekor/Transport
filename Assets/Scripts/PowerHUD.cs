using UnityEngine;
using System.Collections;

public class PowerHUD : MonoBehaviour {
	Generator generator = null;
	void Awake(){
		generator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Generator>();
	}

	void Update(){
		transform.Find("Bar").localScale = transform.Find("Bar").localScale.setY(generator.powerPercent());
	}
}
