using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]
public class Missile : Projectile {
	public int straightTimer;

	public float maxVel = 8f;
	float vel = 4f;
	float accel = 0.07f;
	float maxRot = 6f;
	float rotAccel = 2.5f;
	float rotVel = 0f;

	bool locked = false;
	GameObject target;
	void Update(){
		if(locked) return;
		if(target != null) return;
		findTarget();
	}
	public void lockOn(){
		target = Crosshair.giveNearestAvailableTarget(gameObject, damage);
	}
	protected override void FixedUpdate(){
		base.FixedUpdate();
		move();
		rotate();
	}
	void findTarget(){
		LayerMask mask = new LayerMask();
		if(gameObject.layer == 9){
			mask = 1 << 8;
		}else if(gameObject.layer == 8){
			mask = 1 << 9;
		}

		Collider2D col = null;
		int i=0;
		while(col == null && i < 10){
			col = Physics2D.OverlapCircle(transform.position, i, mask);
			if(col != null){
				target = col.gameObject;
			}
			i++;
		}

	}
	void move(){
		vel+=accel;
		if(vel > maxVel){
			vel = maxVel;
		}
		this.transform.GetComponent<Rigidbody2D>().velocity = transform.up*vel;
	}
	void rotate(){
		if(straightTimer > 0 || target == null){
			straightTimer--;
		}else{
			Vector3 direction = this.transform.position - target.transform.position;
			float difference = transform.rotation.eulerAngles.z - Quaternion.LookRotation(transform.forward, - direction).eulerAngles.z;
			if((difference > 0 && difference < 180) || difference < -180){
				rotVel-=rotAccel;
			}else{
				rotVel+=rotAccel;
			}
			if(rotVel > maxRot){
				rotVel = maxRot;
			}else if(rotVel < -maxRot){
				rotVel = -maxRot;
			}
			transform.Rotate(0,0,rotVel);
		}
	}
}
