using UnityEngine;
using System.Collections;

public class ShipHud : MonoBehaviour {
	RectTransform rt;
	GameObject player;
	void Awake () {
		rt = GetComponent<RectTransform>();
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update () {
		if(player == null)
			return;
		rt.anchorMax = Camera.main.WorldToViewportPoint(player.transform.position + transform.right * 2);
		rt.anchorMin = Camera.main.WorldToViewportPoint(player.transform.position + transform.right * 2);
	}
}
