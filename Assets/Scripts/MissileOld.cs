using UnityEngine;
using System.Collections;

public class MissileOld : MonoBehaviour {
	public ParticleSystem explosion;
	GameObject target;
	public int straightTimer = 25;
	public int damage = 10;

	int	life = 200;
	float maxVel = 12f;
	float vel = 4f;
	float accel = 0.07f;
	float maxRot = 6f;
	float rotAccel = 2.5f;
	float rotVel = 0f;
	bool hostile = false;
	void Start () {

	}
	
	void Update () {
		lifeTime();
		move();
		rotate();
		getTarget();
	}
	void lifeTime(){
		life--;
		if(life<0){
			Destroy(this.gameObject);
			ParticleSystem newExplosion = (ParticleSystem)Instantiate(explosion,this.transform.position,new Quaternion(0,0,0,0));
		}
	}
	void getTarget(){
		if(hostile){
			target = GameObject.FindGameObjectWithTag("Player");
		}else{
			//if(GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Targeting>().target != null){
				//target = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Targeting>().target;
			//}else{
				GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
				GameObject[] asteroid = GameObject.FindGameObjectsWithTag("Asteroid");
				float distance = -1;
				int t = 0;
				for(int i=0;i<enemy.Length;i++){
					float testDistance = Vector3.Distance(enemy[i].transform.position,this.transform.position);
					if(distance == -1 || testDistance < distance){
						distance = testDistance;
						t = i;
					}
					target = enemy[t];
				}
				for(int i=0;i<asteroid.Length;i++){
					float testDistance = Vector3.Distance(asteroid[i].transform.position,this.transform.position);
					if(distance == -1 || testDistance < distance){
						distance = testDistance;
						t = i;
					}
					target = asteroid[t];
				}
			//}
		}
	}
	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.tag.Equals("Enemy")){
			collider.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity*5);
			Destroy(this.gameObject);
			ParticleSystem newExplosion = (ParticleSystem)Instantiate(explosion,this.transform.position,new Quaternion(0,0,0,0));
		}
		if(hostile){
			if(collider.tag.Equals("Player")){
				collider.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity*5);
				Destroy(this.gameObject);
				ParticleSystem newExplosion = (ParticleSystem)Instantiate(explosion,this.transform.position,new Quaternion(0,0,0,0));
			}
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
	public void setTarget(GameObject t){
		target = t;
	}
	public void cancelStraight(){
		straightTimer = 0;
	}
	public void setHostile(bool val){hostile = val;}
	public void setMaxVel(float val){maxVel = val;}
	public void setMaxRot(float val){maxRot = val;}
	public int getDamage(){return damage;}
	public void setDamage(int val){damage = val;}
}
