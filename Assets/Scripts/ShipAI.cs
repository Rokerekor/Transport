using UnityEngine;
using System.Collections;

public class ShipAI : MonoBehaviour {
	
	public float movementSpeed;
	public float rotationSpeed;

	internal enum spriteLayer : int {enterShip = 9, def = 11}

	internal float speedModifier = 1;
	internal GameObject home;
	internal GameObject bay;
	internal Rigidbody2D rb;
	internal Hull hull;

	internal bool taskInProgress;

	protected virtual void Awake () {
		hull = GetComponent<Hull>();
		rb = GetComponent<Rigidbody2D>();
	}
	protected virtual void  Start(){
		StartCoroutine(pushAway());
	}
	IEnumerator pushAway(){
		while(true){
			yield return new WaitForFixedUpdate();
			Collider2D col = Physics2D.OverlapCircle(transform.position, 0.5f);
			if(col == null){
				continue;
			}
			if(col.GetComponent<ShipAI>() != null){
				rb.AddForce((transform.position - col.transform.position).normalized);
			}

		}
	}
	protected virtual IEnumerator goTo(GameObject target, float dist){
		float distance = Mathf.Infinity;
		while(distance > dist){
			distance = Vector3.Distance(transform.position, target.transform.position);

			if(distance > 2){
				speedModifier = 1;
			}else{
				speedModifier = distance/2;
			}
			moveTowards(target.transform.position);
			yield return new WaitForFixedUpdate();
		}
		taskInProgress = false;
	}

	protected virtual IEnumerator goToPos(Vector3 pos, float dist){
		float distance = Mathf.Infinity;
		while(distance > dist){
			distance = Vector3.Distance(transform.position, pos);
			moveTowards(pos);
			yield return new WaitForFixedUpdate();
		}
		taskInProgress = false;
	}
	protected virtual IEnumerator goToTargetPos(GameObject target, Vector3 pos, float dist){
		float distance = Mathf.Infinity;
		while(distance > dist){
			if(target == null){
				break;
			}
			distance = Vector3.Distance(transform.position, target.transform.position + pos);
			moveTowards(target.transform.position + pos);
			yield return new WaitForFixedUpdate();
		}
		taskInProgress = false;
	}
	protected virtual IEnumerator enterBay(GameObject bay){
		while(true){
			speedModifier = 1;
			Vector3 targetPos = bay.transform.position;
			float pDot = Vector3.Dot(transform.position - bay.transform.position, bay.transform.up);
			float prDot = Vector3.Dot(transform.position - bay.transform.position, bay.transform.right);
			float aDot = Vector3.Dot(transform.up, -bay.transform.up);
			float distance = Vector3.Distance(transform.position, bay.transform.position);
			if(pDot < 2f && aDot < 0.5f){
				if(prDot > 0){
					targetPos += bay.transform.transform.up * 3f + bay.transform.right * 1.5f;
				}else{
					targetPos += bay.transform.transform.up * 3f  - bay.transform.right * 1.5f;
				}
			}else{
				if(pDot < 0 && distance < 0.5f){
					gameObject.SetActive(false);
				}else if(distance < 1f){
					setSpriteLayer((int)spriteLayer.enterShip);
				}
				targetPos = bay.transform.position + bay.transform.up * (pDot-1f);
			}

			moveTowards(targetPos);
			yield return new WaitForFixedUpdate();
		}
	}

	void moveTowards(Vector3 pos){
		Vector3 direction = pos - transform.position;
		float difference = (Vector3.Dot(direction.normalized, transform.right));

		if(difference > 0){
			rb.AddTorque(-rotationSpeed);
		}else{
			rb.AddTorque(rotationSpeed);
		}
		rb.AddForce(transform.up*movementSpeed*speedModifier);
	}
	protected virtual void setSpriteLayer(int layer){
		GetComponent<SpriteRenderer>().sortingOrder = layer;
		ParticleSystemRenderer[] ps = GetComponentsInChildren<ParticleSystemRenderer>();
		foreach(ParticleSystemRenderer p in ps){
			p.sortingOrder = layer;
		}
	}
	public void setBay(GameObject val){bay = val;}

}
