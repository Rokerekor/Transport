using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Targeted : MonoBehaviour {
	List<Projectile> incoming;

	GameObject target;
	Crosshair crosshair;

	float incomingDamage = 0;

	void Update () {
		if(target == null){
			Crosshair.targetDestroyed(this);
			return;
		}
		transform.position = target.transform.position;
		transform.Rotate(new Vector3(0,0,-2f));
	}
	public void setTarget(GameObject target){
		this.target = target;
		transform.localScale *= 1.5f * target.GetComponent<SpriteRenderer>().bounds.extents.magnitude / GetComponent<SpriteRenderer>().bounds.extents.magnitude;
	}
	public GameObject getTarget(){
		return target;
	}
	public void setCrosshair(Crosshair crosshair){
		this.crosshair = crosshair;
	}
	public void addIncomingDamage(float damage){
		incomingDamage += damage;
	}
	public bool dealtWith(){
		return incomingDamage >= target.GetComponent<Hull>().getHealth();
	}
}
