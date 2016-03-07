using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public abstract class Projectile : MonoBehaviour {


	public GameObject[] spawnOnHit;

	GameObject owner;
	public int damage;
	public bool plasma;
	int life = 200;
	protected virtual void FixedUpdate(){
		life--;
		if(life <= 0){
			hit();
		}
	}
	public void OnTriggerEnter2D(Collider2D col){
		Hull hull = col.GetComponent<Hull>();
		if(hull == null) return;
		if(gameObject.layer == col.gameObject.layer) return;
		if(hull.receiveDamage(gameObject, damage, transform.position, transform.up)){
			if(plasma) return;
			hit();
		}
	}
	internal void hit(){
		if(spawnOnHit.Length > 0){
			foreach(GameObject spawn in spawnOnHit){
				Instantiate(spawn, transform.position, transform.rotation);
			}
		}
		Destroy(gameObject);
	}
	public void setOwner(GameObject owner){
		this.owner = owner;
	}
}
