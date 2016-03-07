using UnityEngine;
using System.Collections;

public class Bullet : Projectile {

	public float speed;
	internal new void FixedUpdate () {
		base.FixedUpdate();
		transform.position += transform.up * speed;
	}
	public static void spawnBullet(GameObject owner){
		spawnProjectile(owner, "prefabs/Bullet");
	}
	public static void spawnPlasmaBeam(GameObject owner){
		spawnProjectile(owner, "prefabs/PlasmaBeam");
	}
	static void spawnProjectile(GameObject owner, string projectile){
		GameObject newBullet = (GameObject)Instantiate(Resources.Load<GameObject>(projectile), owner.transform.position, owner.transform.rotation);
		newBullet.layer = owner.layer;
		newBullet.GetComponent<Projectile>().setOwner(owner);
	}
}
