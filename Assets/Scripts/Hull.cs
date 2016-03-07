	using UnityEngine;
using System.Collections;

public class Hull : MonoBehaviour {
	public int maxHealth;
	public AudioClip[] playOnDeath;
	public GameObject[] spawnOnHit;
	public GameObject[] spawnOnDeath;
	public bool damageSmoke = false;

	string gibFolder;

	internal bool targetable = true;
	internal bool invulnerable = false;
	internal bool stunned = false;
	internal int health;


	internal int defaultLayer;

	internal AudioSource audioSource;

	protected virtual void Start () {
		defaultLayer = gameObject.layer;
		health = maxHealth;
		if(damageSmoke){
			GameObject newSmoke = Instantiate(Resources.Load("DamageSmoke", typeof(GameObject)),transform.position, new Quaternion(0,0,0,0)) as GameObject;
			newSmoke.transform.parent = transform;
		}
		if(playOnDeath.Length > 0){
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.spatialBlend = 1f;
			audioSource.maxDistance = 30f;
		}
	}
	public void heal(int val){
		GameObject newParticles = (GameObject)Instantiate(Resources.Load("HealParticles", typeof(GameObject)),transform.position,new Quaternion(0,0,0,0));
		newParticles.transform.parent = transform;
		newParticles.GetComponent<ParticleSystem>().Emit(val);
		healthChange(val);
	}
	protected virtual void healthChange(int val){
		health += val;
		if(health > maxHealth){
			health = maxHealth;
		}else if(health <= 0){
			health = 0;
			death();
		}
	}
	public virtual bool receiveDamage(GameObject attacker, int damage, Vector3 pos, Vector3 dir){
		if(invulnerable){return false;}
		if(spawnOnHit.Length > 0){
			foreach(GameObject spawn in spawnOnHit){
				Instantiate(spawn,pos,Quaternion.LookRotation(dir));
			}
		}
		healthChange(-damage);
		return true;
	}
	protected virtual void death(){
		if(playOnDeath.Length > 0){
			audioSource.clip = playOnDeath[Random.Range(0,playOnDeath.Length)];
			audioSource.Play();
		}
		if(gibFolder != null){
			Sprite[] gibs = Resources.LoadAll<Sprite>(gibFolder);
			if(gibs.Length > 0){
				foreach(Sprite sprite in gibs){
					GameObject newGib = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/gib"), transform.position + new Vector3(Random.Range(-0.05f,0.05f), Random.Range(-0.05f,0.05f), 0), Extensions.ZeroQuaternion());
					newGib.transform.localScale = transform.localScale;
					newGib.GetComponent<SpriteRenderer>().sprite = sprite;
					newGib.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-3f,3f), Random.Range(-3f,3f)));
					newGib.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-3f, 3f));
				}
			}
		}
		if(spawnOnDeath.Length > 0){
			foreach(GameObject spawn in spawnOnDeath){
				Instantiate(spawn,transform.position,new Quaternion(0,0,0,0));
			}
		}
		Destroy(gameObject);
	}

	public void removeStun(){
		stunned = false;
	}
	public virtual void applyStun(float time){
		stunned = true;
		Invoke("removeStun", time);
	}
	public bool isTargetable(){return targetable;}
	public bool isInvulnerable(){return invulnerable;}
	public bool isStunned(){return stunned;}
	public bool isFullHealth(){
		return (health == maxHealth);
	}
	public int getHealth(){return health;}
	public int getMaxHealth(){return maxHealth;}

	public void setGibFolder(string val){gibFolder = val;}

	public void layerIgnore(){
		gameObject.layer = 2; // Set to ignore raycast layer
	}
	public void layerIsolate(){
		gameObject.layer = 10; // Set to test layer
	}
	public void layerDefault(){
		gameObject.layer = defaultLayer; // Revert to original layer
	}
}
