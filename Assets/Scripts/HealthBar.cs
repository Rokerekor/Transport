using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	Hull hull;
	void Update () {
		if(hull == null)
			return;
		transform.localScale = Vector3.one * hull.getHealth() / hull.getMaxHealth();
	}
}
